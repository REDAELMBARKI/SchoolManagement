namespace SchoolManagement.Domain.Interfaces.Queries.Common
{
    public interface IQuery<T> where T : class
    {
      
            Task<List<T>> GetAllAsync();
            Task<T?> GetByIdAsync(Guid id);
            Task<bool> ExistsAsync(Guid id);

    }
}