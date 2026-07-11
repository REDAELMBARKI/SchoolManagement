
using SchoolManagement.Backend.Data.Factories;
using SchoolManagement.Backend.Entities;
using SchoolManagement.Backend.Data ;

namespace  SchoolManagement.Backend.Data.Seeders ; 

public class GroupSeeder : Seeder
{
    private readonly   GroupFactory _factory;

    public GroupSeeder(AppDbContext context) : base(context)
    {
            _factory = new  GroupFactory(context);
    } 

    public override async Task RunAsync()
    {
       List<Group> groups = await _factory.MakeMany(10);
       await Context.Groups.AddRangeAsync(groups);
       await Context.SaveChangesAsync() ;
    }
}