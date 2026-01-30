


using MongoDB.Driver;

namespace BuildingBlocks.EventStore.Checkpoints
{
    public class CheckpointStore : ICheckpointStore
    {   
        private readonly IMongoCollection<Checkpoint> _collection; // "Tabela" tj kolekcija u MongoDb
        public CheckpointStore(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<Checkpoint>("Checkpoints");
            CreateIndex();
        }

        // Za brze pretrazivanje kroz MongoDb "tabelu" tj kolekciju sa checkpoints
        private void CreateIndex()
        {
            var indexKeys = Builders<Checkpoint>.IndexKeys.Ascending(x => x.CheckpointName);
            var indexUniqueOptions = new CreateIndexOptions { Unique = true }; // Atomicnost + Db Race Conditions
            var indexModel = new CreateIndexModel<Checkpoint>(indexKeys, indexUniqueOptions);
            _collection.Indexes.CreateOne(indexModel);
        }

        public async Task<ulong?> GetCheckpointAsync(string checkpointName, CancellationToken ct)
        {
            var filter = Builders<Checkpoint>.Filter.Eq(c => c.CheckpointName, checkpointName);
            var checkpoint = await _collection.Find(filter).FirstOrDefaultAsync(ct);

            return checkpoint?.Position;
        }

        public async Task SaveCheckpointAsync(string checkpointName, ulong position, CancellationToken ct)
        {
            var filter = Builders<Checkpoint>.Filter.Eq(c => c.CheckpointName, checkpointName);

            var update = Builders<Checkpoint>.Update.Set(c => c.Position, position).Set(c => c.UpdatedAt, DateTime.UtcNow);

            await _collection.UpdateOneAsync(filter, 
                                            update, 
                                            new UpdateOptions { IsUpsert = true }, // Kreiram kolekciji prvi prvom unosu 
                                            ct);
        }
    }
}
