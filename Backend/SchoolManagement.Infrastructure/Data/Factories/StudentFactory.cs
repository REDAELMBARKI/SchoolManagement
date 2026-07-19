using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;
using Slugify;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class StudentFactory : Factory<Student>
{
    public StudentFactory(AppDbContext context) : base(context)
    {
    }

    protected override async Task<Student> Make()
    {
        var genders = await Context.Genders.Select(g => g.Id).ToListAsync();
        var intakes = await Context.Intakes.Select(i => i.Id).ToListAsync();

        var firstName = faker.Name.FirstName();
        var lastName = faker.Name.LastName();
        var email = faker.Random.Bool() ? faker.Internet.Email(firstName, lastName) : null;
        var phone = faker.Phone.PhoneNumber();
        var dateOfBirth = DateOnly.FromDateTime(faker.Date.Past(30, DateTime.Now.AddYears(-18)));
        var genderId = genders.Any() ? faker.PickRandom(genders) : (Guid?)null;
        var intakeId = intakes.Any() ? faker.PickRandom(intakes) : (Guid?)null;

        return Student.Register(
            firstName: firstName,
            lastName: lastName,
            slug: new SlugHelper().GenerateSlug($"{firstName} {lastName}"),
            genderId: genderId,
            email: email,
            phone: phone,
            dateOfBirth: dateOfBirth,
            intakeId: intakeId
        );
    }
}
