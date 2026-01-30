using System.Text;
using System.Text.Json;
using BuildingBlocks.Core.Events;
using BuildingBlocks.EventStore.Domain;
using BuildingBlocks.EventStore.EventTypeMappers;
using BuildingBlocks.EventStore.Repositories.Snapshots;
using EventStore.Client;

namespace BuildingBlocks.EventStore.Repositories.EvetnStore
{
    public class EventStoreRepository<TAggregate, TId> : IEventStoreRepository<TAggregate, TId>
        where TAggregate : AggregateRootEventSource<TId>, new()
        where TId : IPersistableId
    {
        private readonly EventStoreClient _eventStore;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IEventTypeMapper _eventTypeMapper; 
        private readonly ISnapshotRepository<TAggregate, TId> _snapshotRepository;

        public EventStoreRepository(EventStoreClient eventStoreClient, 
                                   IEventTypeMapper eventTypeMapper,
                                   ISnapshotRepository<TAggregate, TId> snapshotRepository)
        {
            _eventStore = eventStoreClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // pozeljno veoma 
            };
            _eventTypeMapper = eventTypeMapper;
            _snapshotRepository = snapshotRepository;
        }

        private string GetStreamName(TId id)
        {
            return $"{typeof(TAggregate).Name}-{id.PersistToString()}"; // typeof(TAggregate) je tip konkretnog aggregata
        }

        private EventData ToEventData(IDomainEvent domainEvent, string? userId, string? correlationId)
        {
            // Serializuj kompletan domain event
            var eventJson = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(),_jsonOptions);
            var eventBytes = Encoding.UTF8.GetBytes(eventJson);

            // Metadata
            var metadata = new
            {
                UserId = userId,
                CorrelationId = correlationId ?? Guid.NewGuid().ToString(), // Distributed tracing
                Timestamp = DateTime.UtcNow,
                // Čuvaj domain event ID u metadata za lakše pretraživanje
                DomainEventId = domainEvent is DomainEvent de ? de.EventId : Guid.Empty
            };
            var metadataJson = JsonSerializer.Serialize(metadata, _jsonOptions);
            var metadataBytes = Encoding.UTF8.GetBytes(metadataJson);

            // EventData je tip kada upisujemo u EventStoreDb
            return new EventData(eventId: Uuid.NewUuid(),
                                type: domainEvent.GetType().Name, // custom domain event name
                                data: eventBytes,
                                metadata: metadataBytes,
                                contentType: "application/json");
        }

        private IDomainEvent DeserializeEvent(ResolvedEvent resolvedEvent)
        {   // ResolvedEvent je tip event kada citamo iz EventStoreDb

            var domainEventTypeName = resolvedEvent.Event.EventType;

            var domainEventType = _eventTypeMapper.GetDomainEventType(domainEventTypeName);

            var domainEventJson = Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span);

            var domainEvent = (IDomainEvent)JsonSerializer.Deserialize(domainEventJson, domainEventType, _jsonOptions)!;

            return domainEvent;
        }

        public async Task<TAggregate?> GetStreamByIdAsync(TId id, CancellationToken ct)
        {
            // Pokusaj ucitati snapshot ako on postoji za zeljeni stream
            var snapshot = await _snapshotRepository.GetSnapshotAsync(id, ct);

            TAggregate aggregate;
            StreamPosition startPosition;

            if (snapshot != null)
            {
                // Ucitaj iz snapshot
                aggregate = snapshot.Aggregate;
                startPosition = StreamPosition.FromInt64(snapshot.Version + 1);
            }
            else
            {
                // Kreiraj novi agregat jer snapshot ne postoji
                aggregate = new TAggregate();
                startPosition = StreamPosition.Start;
            }

            // Ucitaj (replay) events samo posle snapshot ako on postoji. Ako ne postoji, replay ceo aggregate od pocetka.
            var streamName = GetStreamName(id);
            var stream = _eventStore.ReadStreamAsync(Direction.Forwards,
                                                     streamName,
                                                     startPosition, 
                                                     cancellationToken: ct);

            // Primeni events
            var domainEvents = new List<IDomainEvent>();
            await foreach (var resolvedEvent in stream.WithCancellation(ct))
            {   
                // Ne zanima me system event koje EventStore ubaci automatski
                if (resolvedEvent.Event.EventType.StartsWith("$"))
                    continue;

                var domainEvent = DeserializeEvent(resolvedEvent);
                domainEvents.Add(domainEvent);
            }

            if (domainEvents.Any())
                aggregate.LoadFromHistory(domainEvents); // Replay onoliko events u zavisnosti ima li snapshot ili ako nema onda od pocetka replay

            return aggregate;
        }

        public async Task DeleteStreamAsync(TId id, CancellationToken ct)
        {
            var streamName = GetStreamName(id);

            try
            {
                await _eventStore.TombstoneAsync(streamName,
                                                 StreamState.Any,
                                                 cancellationToken: ct);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveUncommittedEventsAsync(TAggregate aggregate, string? userId, string? correlationId, CancellationToken ct)
        {
            var uncommittedEvents = aggregate.ClearDomainEvents(); // kopira _uncommittedEvents iz agregat u varijablu, pa izbrise iz agregata

            if (!uncommittedEvents.Any())
                return;

            var streamName = GetStreamName(aggregate.Id);

            var eventDataList = uncommittedEvents.Select(domainEvent => ToEventData(domainEvent, userId, correlationId)).ToList();

            try
            {
                bool streamExists = await StreamExistAsync(aggregate.Id, ct);

                if (streamExists) 
                {
                    await _eventStore.AppendToStreamAsync(streamName,
                                                        StreamRevision.FromInt64(aggregate.Version),
                                                        eventDataList,
                                                        cancellationToken: ct);
                }
                else
                {
                    await _eventStore.AppendToStreamAsync(streamName,
                                                        StreamState.NoStream,
                                                        eventDataList,
                                                        cancellationToken: ct);
                }
                // AppendToStreamAsync automatski svakom event iz eventDataList dodeli streamRevision
            }
            catch (WrongExpectedVersionException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }

            // Napravi snapshot nakog uspesnog save 
            if (aggregate.Version % 100 == 0)
                await _snapshotRepository.SaveSnapshotAsync(aggregate.Id, aggregate, aggregate.Version, ct);

        }

        public async Task<bool> StreamExistAsync(TId id, CancellationToken ct)
        {
            var streamName = GetStreamName(id);

            try
            {
                var result = await _eventStore.ReadStreamAsync(Direction.Forwards,
                                                               streamName,
                                                               StreamPosition.Start,
                                                               maxCount: 1,
                                                               cancellationToken: ct).ToListAsync(ct);

                return result.Any();
            }
            catch (StreamNotFoundException)
            {
                return false;
            }
        }
    }
}
