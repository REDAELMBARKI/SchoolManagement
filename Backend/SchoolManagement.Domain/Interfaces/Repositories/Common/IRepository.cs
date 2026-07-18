namespace SchoolManagement.Domain.Interfaces.Repositories.Common;

public interface IRepository<T> where T : class
{
    Task<T>        AddAsync(T entity);
    Task<T>        UpdateAsync(T entity);
    Task           DeleteAsync(Guid id);
    Task<T?>       GetByIdAsync(Guid id);
}
