


using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Core.Domain
{
    public abstract class Entity<TId> : IEntity<TId>
    {   
        public TId Id { get; protected set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        public bool IsDeleted { get; set; } = false;

        [Timestamp] // = IsRowVersion() u OnModelCreating => Sql Server automatski prevetns race condition sa ovim
        public byte[] RowVersion { get; set; } 
    }
}
