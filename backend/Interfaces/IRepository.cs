namespace SchoolManagement.Backend.Interfaces;
public interface IReadRepository<T> where T : class
{
    Task<List<T>>  GetAllAsync();
    Task<T?>       GetOneAsync(int id);
    Task<bool>     ExistsAsync(int id);
}

// WRITE ONLY
public interface IWriteRepository<T> where T : class
{
    Task<T>        AddAsync(T entity);
    Task<T?>       UpdateAsync(int id, T entity);
    Task           DeleteAsync(int id);
}


public interface IBulkActionsRepository<T> where T : class
{
    Task<List<T>>  AddManyAsync(IEnumerable<T> entities);
    Task           UpdateManyAsync(IEnumerable<T> entities);
    Task           DeleteManyAsync(IEnumerable<int> ids);
}

