
using SchoolManagement.Infrastructure.Data.Factories;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;

namespace  SchoolManagement.Infrastructure.Data.Seeders ; 

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