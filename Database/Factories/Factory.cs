using Bogus;
using Microsoft.VisualBasic;
namespace SchoolManagement.Database.Factories ; 
public abstract class Factory<T> where T : class 
{
    protected readonly Faker faker = new Faker() ; 
    protected abstract T Make() ; 

    public List<T> MakeMany(int count)
    {
        return Enumerable.Range(0 , count)
               .Select(_ => Make()) 
               .ToList() ;
    } 
     
}