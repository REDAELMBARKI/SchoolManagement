using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class RoleFactory : Factory<Role>
{
    public RoleFactory(AppDbContext context) : base(context)
    {
    }

    protected override Task<Role> Make()
    {
        var roles = new (string Name, string Slug)[]
        {
            ("Admin", "admin"),
            ("Teacher", "teacher"),
            ("Student", "student"),
            ("Parent", "parent"),
            ("Staff", "staff")
        };

        var selected = faker.PickRandom(roles);
        return Task.FromResult(Role.Create(selected.Name, selected.Slug));
    }
}
