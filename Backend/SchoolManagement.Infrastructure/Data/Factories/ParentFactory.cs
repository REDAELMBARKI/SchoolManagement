using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;
using Slugify;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class ParentFactory : Factory<Parent>
{
    public ParentFactory(AppDbContext context) : base(context)
    {
    }

    protected override async Task<Parent> Make()
    {
        var genders = await Context.Genders.Select(g => g.Id).ToListAsync();

        var firstName = faker.Name.FirstName();
        var lastName = faker.Name.LastName();
        var email = faker.Random.Bool() ? faker.Internet.Email(firstName, lastName) : null;
        var phone = faker.Phone.PhoneNumber();
        var genderId = genders.Any() ? faker.PickRandom(genders) : (Guid?)null;
        var relationship = faker.PickRandom<RelationshipType>();

        return Parent.Register(
            firstName: firstName,
            lastName: lastName,
            slug: new SlugHelper().GenerateSlug($"{firstName} {lastName}"),
            genderId: genderId,
            email: email,
            phone: phone,
            relationship: relationship
        );
    }
}
