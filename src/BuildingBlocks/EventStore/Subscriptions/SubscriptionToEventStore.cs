using System.Text.Json;
using BuildingBlocks.Core.Events;
using BuildingBlocks.EventStore.Checkpoints;
using BuildingBlocks.EventStore.EventTypeMappers;
using EventStore.Client;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.EventStore.Subscriptions
{   
    // Pozadinski servis je uvek pretplacen na sve streams u EventStore jer DomainEventHandler ima CanHandle metodu koja zna koji event moze da handluje, a koji ne
    // Mogu imati razlicite instance ovog servisa i zato subscriptionId mi treba, a svaka instanca bira na koje streams ce se pretplatiti
    public class SubscriptionToEventStore : BackgroundService
    {
        private readonly EventStoreClient _eventStore;
        private readonly IEnumerable<IDomainEventHandlerES> _eventHandlers; // Svi custom klase koje implementiraj DomainEvenHandler (: IEventHandler)
        private readonly ICheckpointStore _checkpointStore;
        private readonly IEventTypeMapper _eventTypeMapper;
        private readonly string _subscriptionId;
        private readonly string[]? _streamNames;  // null = pretplata na svaki stream 

        public SubscriptionToEventStore(EventStoreClient eventStore,
                                        IEnumerable<IDomainEventHandlerES> eventHandlers,
                                        ICheckpointStore checkpointStore,
                                        IEventTypeMapper eventTypeMapper,
                                        string subscriptionId = "default-subscription",
                                        params string[]? streamNames)
        {
            _eventStore = eventStore;
            _eventHandlers = eventHandlers;
            _checkpointStore = checkpointStore;
            _eventTypeMapper = eventTypeMapper;
            _subscriptionId = subscriptionId;
            _streamNames = streamNames?.Length > 0 ? streamNames : null; 
            // _subscriptionId + streamName = CheckpointName u Checkpoint.cs
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {   // Pokrene se na startu aplikacije i non stop radi zbog SubscribeToAllAsync/SubscribeToStreamAsync koje pokrecu OnEventAppearedAll/OnEventAppearedStream
            try
            {
                if (_streamNames is null)
                {   
                    // Subscriber instance pretplacena na sve streams 

                    // Ucitaj poslednji checkpoint
                    var checkpoint = await _checkpointStore.GetCheckpointAsync(_subscriptionId, stoppingToken);
                    var fromPosition = checkpoint.HasValue ? FromAll.After(new Position(checkpoint.Value, checkpoint.Value))
                                                           : FromAll.Start;

                    // Subscribe to $all stream tj na sve streams tj na svaki agregat
                    await _eventStore.SubscribeToAllAsync(fromPosition,
                                                          OnEventAppearedAll,
                                                          subscriptionDropped: OnSubscriptionDropped,
                                                          cancellationToken: stoppingToken);
                }
                else
                {
                    // Pretplata na zeljene streams ili samo na 1 
                    foreach (var streamName in  _streamNames)
                    {
                        var streamCheckpoint = await _checkpointStore.GetCheckpointAsync($"{_subscriptionId}-{streamName}", stoppingToken);
                        var fromPosition = streamCheckpoint.HasValue ? FromStream.After(StreamPosition.FromInt64((long)streamCheckpoint.Value))
                                                                     : FromStream.Start;

                        // Subscribe to desired stream 
                        await _eventStore.SubscribeToStreamAsync(streamName,
                                                                 fromPosition,
                                                                 (sub, e, ct) => OnEventAppearedStream(sub, e, streamName, ct),
                                                                 subscriptionDropped: OnSubscriptionDropped,
                                                                 cancellationToken: stoppingToken);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task OnEventAppearedAll(StreamSubscription subscription, ResolvedEvent resolvedEvent, CancellationToken ct)
        {   // Non stop radi jer je ovo background servis i automatski se poziva u SubscribeToAllAsync
            try
            {
                await ProcessResolvedEvent(resolvedEvent, ct);

                // Sacuvaj checkpoint ako globalna pozicija (OriginalPosition) nije null, jer ona moze biti null za $istem event pa da to preskocim
                if (resolvedEvent.OriginalPosition.HasValue)
                    await _checkpointStore.SaveCheckpointAsync(_subscriptionId,
                                                               resolvedEvent.OriginalPosition.Value.CommitPosition,
                                                               ct);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task OnEventAppearedStream(StreamSubscription subscription, ResolvedEvent resolvedEvent, string streamName, CancellationToken ct)
        {  // Non stop radi jer je ovo background servis i automatski se poziva u SubscribeToStreamAsync
            try
            {
                await ProcessResolvedEvent(resolvedEvent, ct);
                // Nema globalne pozicije
                await _checkpointStore.SaveCheckpointAsync($"{_subscriptionId}-{streamName}",
                                                           resolvedEvent.Event.EventNumber.ToUInt64(),
                                                           ct);

            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task ProcessResolvedEvent(ResolvedEvent resolvedEvent, CancellationToken ct)
        {
            // Preskocim system events
            if (resolvedEvent.Event.EventType.StartsWith("$"))
                return;

            var eventTypeName = resolvedEvent.Event.EventType;
            var eventType = _eventTypeMapper.GetDomainEventType(eventTypeName);

            if (eventType is null)
                return;

            // Deserialize event
            var eventData = JsonSerializer.Deserialize(resolvedEvent.Event.Data.Span, eventType) as IDomainEvent;
            if (eventData is null)
                return;

            // Nadji handlers koji mogu da obrade ovaj event
            var handlers = _eventHandlers.Where(h => h.CanHandle(eventType)).ToList();

            // Obradi event sa svim handlers
            foreach (var handler in handlers)
            {
                try
                {
                    await handler.HandleAsync(eventData, ct);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        private void OnSubscriptionDropped(StreamSubscription subscription, SubscriptionDroppedReason reason, Exception? exception)
        {
            if (reason != SubscriptionDroppedReason.Disposed)
               // Reconnect 
                _ = ReconnectAsync(); // Fire and forget
            
        }

        private async Task ReconnectAsync()
        {
            await Task.Delay(5000);
            await ExecuteAsync(CancellationToken.None);
        }
    }
}
