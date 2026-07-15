namespace SchoolManagement.Domain.Interfaces.Repositories.Common;

public interface IRepository<T> where T : class
{
    Task<T>        AddAsync(T entity);
    Task<T?>       UpdateAsync(int id, T entity);
    Task           DeleteAsync(int id);
}



