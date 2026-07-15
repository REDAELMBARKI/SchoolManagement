namespace SchoolManagement.Domain.Interfaces.Queries.Common
{
    public interface IQuery<T> where T : class
    {
      
            Task<List<T>> GetAllAsync();
            Task<T?> GetByIdAsync(int id);
            Task<bool> ExistsAsync(int id);

    }
}