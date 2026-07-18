namespace SchoolManagement.Domain.Interfaces.Queries.Common;

// Keep this simple, just for shared markers if needed
// Each specific query interface will define its own entity and response methods
public interface IQuery
{
}

// Optional: If you want a base for all entity query services
public interface IEntityQuery<TEntity> : IQuery where TEntity : class
{
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<bool> IsExistsAsync(Guid id);
}
