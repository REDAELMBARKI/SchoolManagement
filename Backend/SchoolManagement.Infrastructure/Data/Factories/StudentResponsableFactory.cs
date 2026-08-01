using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Infrastructure.Data;
using Slugify;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class StudentResponsableFactory : Factory<StudentResponsable>
{
    public StudentResponsableFactory(AppDbContext context) : base(context)
    {
    }

    protected override async Task<StudentResponsable> Make()
    {
        var genders = await Context.Genders.Select(g => g.Id).ToListAsync();
        var branches = await Context.Branches.Select(b => b.Id).ToListAsync();

        var firstName = faker.Name.FirstName();
        var lastName = faker.Name.LastName();
        var email = faker.Random.Bool() ? faker.Internet.Email(firstName, lastName) : null;
        var phone = faker.Phone.PhoneNumber();
        var genderId = genders.Any() ? faker.PickRandom(genders) : (Guid?)null;
        var relationship = faker.PickRandom<RelationshipType>();
        var branchId = branches.Any() ? faker.PickRandom(branches) : Guid.Empty;

        return StudentResponsable.Register(
            firstName: firstName,
            lastName: lastName,
            slug: new SlugHelper().GenerateSlug($"{firstName} {lastName}"),
            genderId: genderId,
            email: email,
            phone: phone,
            relationship: relationship,
            branchId: branchId
        );
    }
}
