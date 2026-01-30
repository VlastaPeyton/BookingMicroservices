
namespace BuildingBlocks.Core.Domain
{   
    // Audit u Sql Server 
    public interface IEntity
    {
        DateTime CreatedAt { get; set; }
        string? CreatedBy { get; set; }
        DateTime? LastModifiedAt { get; set; }
        string? LastModifiedBy { get; set; }
        bool IsDeleted { get; set; }

        byte[] RowVersion {  get; set; }
    }
    
    public interface IEntity<TId> : IEntity
    {
        TId Id { get; }
    }
}
