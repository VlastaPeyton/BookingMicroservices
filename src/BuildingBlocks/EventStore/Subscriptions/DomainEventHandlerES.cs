using BuildingBlocks.Core.Events;
using MongoDB.Driver;

namespace BuildingBlocks.EventStore.Subscriptions
{   
    // Moram imati konkretnu imeplementaciju ove klase kao CustomDomainEventHandlerES i CustomProjectionHandlerES(da napuni ReadModel kolekciju u Mongo) za AggregateRootEventSourcing
    public abstract class DomainEventHandlerES<TEvent> : IDomainEventHandlerES where TEvent : IDomainEvent
    {
        private readonly IMongoCollection<ProcessedEvent> _collection; // "Tabela" tj kolekcija u MonogDb

        public DomainEventHandlerES(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<ProcessedEvent>("ProcessedEvents");

            CreateIndex();
        }

        // Kreiraj Index nad kolekcijom
        private void CreateIndex()
        {
            var indexKeys = Builders<ProcessedEvent>.IndexKeys.Ascending(e => e.EventId);
            var indexUniqueOptions = new CreateIndexOptions { Unique = true }; // Atomicnost + Db Race Conditions
            var indexModel = new CreateIndexModel<ProcessedEvent>(indexKeys, indexUniqueOptions);
            _collection.Indexes.CreateOne(indexModel);
        }

        // Konkretna klasa mora da override ovu metodu jer ce je ovde pozvati u HandleAsync
        protected abstract Task HandleEventAsync(TEvent domainEvent, CancellationToken ct); 

        public bool CanHandle(Type eventType)
        {
            return typeof(TEvent).IsAssignableFrom(eventType);
        }

        public async Task HandleAsync(IDomainEvent domainEvent, CancellationToken ct)
        {
            var eventId = domainEvent.EventId.ToString();

            var processedEvent = new ProcessedEvent
            {
                EventId = eventId,
                EventType = domainEvent.GetType().Name,
                ProcessedAt = DateTime.UtcNow,
                Status = "Pending",
            };

            try
            {
                // Upisi u Mongo + automatski race codnition srpecen
                await _collection.InsertOneAsync(processedEvent, null, ct);
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                // Neko je vec zauzeo event i obradjuje ga
                return;
            }

            try 
            {
                // Obradi event + side logika pomocu konkretne implementacije ove metode u CustomDomainEventHandler klasi gde mogu da npr posaljem integration event u rabbitmq, azurira read model itd.
                await HandleEventAsync((TEvent)domainEvent, ct);

                // Označi kao završen
                var filter = Builders<ProcessedEvent>.Filter.Eq(e => e.EventId, eventId);
                var update = Builders<ProcessedEvent>.Update.Set(e => e.Status, "Completed");
                await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                // Označi kao failed - omogućava retry
                var filter = Builders<ProcessedEvent>.Filter.Eq(e => e.EventId, eventId);
                var update = Builders<ProcessedEvent>.Update.Set(e => e.Status, "Failed")
                                                            .Set(e => e.Error, ex.Message);

                await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);

                throw;
            }
        }
    }
}
