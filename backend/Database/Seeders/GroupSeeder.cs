
using SchoolManagement.Database.Factories;
using SchoolManagement.Models;

namespace  SchoolManagement.Database.Seeders ; 

public class GroupSeeder : Seeder
{
    private readonly   GroupFactory _factory;
    private readonly AppDbContext _context;

    public GroupSeeder(AppDbContext context)
    {
            _factory = new   GroupFactory();
            _context = context ;

    } 

    public override async Task RunAsync()
    {
       List<Group> Groups = _factory.MakeMany(10);
       await _context.Groups.AddRangeAsync(Groups);
       await _context.SaveChangesAsync() ;
    }
}