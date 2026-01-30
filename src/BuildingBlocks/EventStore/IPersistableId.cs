

namespace BuildingBlocks.EventStore
{   
    /* Svaki TId koriscen za Aggregate.Id za EventStore mora ovo da nasledi kako bi u MongoDb bio siguran
     da ce da se upise Id tipa string kao jedinstveni. */

    public interface IPersistableId
    {
        string PersistToString();
    }
}
