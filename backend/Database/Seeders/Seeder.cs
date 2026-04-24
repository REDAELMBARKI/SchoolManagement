using Bogus;
using SchoolManagement.Backend.Database.Factories;

namespace SchoolManagement.Backend.Database.Seeders ; 

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