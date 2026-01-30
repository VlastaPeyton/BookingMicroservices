using System.Text.Json;
using BuildingBlocks.EventStore.Domain;
using MongoDB.Driver;

namespace BuildingBlocks.EventStore.Repositories.Snapshots
{
    public class SnapshotRepository<TAggregate, TId> : ISnapshotRepository<TAggregate, TId>
        where TAggregate : AggregateRootEventSource<TId>
        where TId : IPersistableId 
    {
        private readonly IMongoCollection<Snapshot<TAggregate>> _collection; // "Tabela" tj kolekcija u MongoDb
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _collectionName; 

        public SnapshotRepository(IMongoDatabase mongoDb)
        {
            _collectionName = $"{typeof(TAggregate).Name}_snapshots";
            _collection = mongoDb.GetCollection<Snapshot<TAggregate>>(_collectionName);
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            CreateIndex();
        }

        // Za brze pretrazivanje kroz MongoDb "tabelu" tj kolekciju sa snasphosts
        private void CreateIndex()
        {
            var indexKeys = Builders<Snapshot<TAggregate>>.IndexKeys.Ascending(x => x.AggregateId);
            var indexUniqueOptions = new CreateIndexOptions { Unique = true }; // Atomicnost + Db Race Conditions
            var indexModel = new CreateIndexModel<Snapshot<TAggregate>>(indexKeys, indexUniqueOptions);
            _collection.Indexes.CreateOne(indexModel);
        }

        public async Task<Snapshot<TAggregate>?> GetSnapshotAsync(TId id, CancellationToken ct)
        {
            var filter = Builders<Snapshot<TAggregate>>.Filter.Eq(s => s.AggregateId, id?.PersistToString());

            var snapshot = await _collection.Find(filter)
                                            .SortByDescending(x => x.Version) // uzmi poslednji (najnoviji) snapshot ako imam vise snapshots po agregatu
                                            .FirstOrDefaultAsync(ct);

            return snapshot;
        }

        public async Task SaveSnapshotAsync(TId id, TAggregate aggregate, int version, CancellationToken ct)
        {
            var snapshot = new Snapshot<TAggregate>
            {
                AggregateId = id.ToString()!,
                AggregateType = typeof(TAggregate).Name,
                Aggregate = aggregate,
                Version = version,
                CreatedAt = DateTime.UtcNow,
            };

            var filter = Builders<Snapshot<TAggregate>>.Filter.Eq(s => s.AggregateId, id?.PersistToString());

            // Imam samo 1 snapshot po agregatu i zato ReplaceOneAsync
            await _collection.ReplaceOneAsync(filter,
                                              snapshot,
                                              new ReplaceOptions { IsUpsert = true }, // Pri prvom unosu, kreira kolekciju
                                              ct);
        }
    }
}
