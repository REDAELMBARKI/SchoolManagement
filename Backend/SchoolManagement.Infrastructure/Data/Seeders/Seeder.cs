using Bogus;
using SchoolManagement.Backend.Data.Factories;
using SchoolManagement.Backend.Data ;
namespace SchoolManagement.Backend.Data.Seeders ; 

public abstract class Seeder
{

    private readonly AppDbContext _context;
    private static readonly Faker _faker =  new Faker() ;
    public Seeder(AppDbContext context)
    {
        _context = context;
    }

    public abstract Task RunAsync(); 
    protected AppDbContext Context => _context ;
    protected Faker Faker => _faker ;
}