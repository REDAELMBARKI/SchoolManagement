
using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend.Models;

namespace  SchoolManagement.Backend.Database.Seeders ; 

public class GroupSeeder : Seeder
{
    private readonly   GroupFactory _factory;

    public GroupSeeder(AppDbContext context) : base(context)
    {
            _factory = new   GroupFactory(context);

    } 

    public override async Task RunAsync()
    {
       List<Group> groups = _factory.MakeMany(10);
       await Context.Groups.AddRangeAsync(groups);
       await Context.SaveChangesAsync() ;
    }
}