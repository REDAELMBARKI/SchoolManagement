namespace SchoolManagement.Interfaces ;
public interface IRepository<T> where T : class
{
    public List<T> GetList() ;

}