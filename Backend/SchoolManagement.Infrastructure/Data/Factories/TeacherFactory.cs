using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;
using Slugify;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class TeacherFactory : Factory<Teacher>
{
    public TeacherFactory(AppDbContext context) : base(context)
    {
    }

    protected override async Task<Teacher> Make()
    {
        var genders = await Context.Genders.Select(g => g.Id).ToListAsync();
        var branches = await Context.Branches.Select(b => b.Id).ToListAsync();

        var firstName = faker.Name.FirstName();
        var lastName = faker.Name.LastName();
        var email = faker.Random.Bool() ? faker.Internet.Email(firstName, lastName) : null;
        var phone = faker.Phone.PhoneNumber();
        var dateOfBirth = faker.Random.Bool() 
            ? DateOnly.FromDateTime(faker.Date.Past(30, DateTime.Now.AddYears(-18))) 
            : (DateOnly?)null;
        var hireDate = faker.Date.Past(10);
        var salary = faker.Finance.Amount(3000, 15000);
        var branchId = branches.Any() ? faker.PickRandom(branches) : Guid.Empty;
        var genderId = genders.Any() ? faker.PickRandom(genders) : (Guid?)null;
        var specializations = new[] { "Mathematics", "English", "Physics", "Chemistry", "Biology", "History" };
        var specialization = faker.PickRandom(specializations);

        return Teacher.Register(
            firstName: firstName,
            lastName: lastName,
            slug: new SlugHelper().GenerateSlug($"{firstName} {lastName}"),
            genderId: genderId,
            email: email,
            phone: phone,
            dateOfBirth: dateOfBirth,
            hireDate: hireDate,
            salary: salary,
            branchId: branchId,
            specialization: specialization
        );
    }
}
