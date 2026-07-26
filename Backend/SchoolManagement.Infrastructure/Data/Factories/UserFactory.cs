using Bogus;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;
using Slugify;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class UserFactory : Factory<DomainUser>
{
    public UserFactory(AppDbContext context) : base(context)
    {
    }

    protected override Task<DomainUser> Make()
    {   
        var genders = Context.Genders.Select(g => g.Id).ToList();
        var firstName = faker.Name.FirstName();
        var lastName = faker.Name.LastName();
       
        return Task.FromResult(DomainUser.Register(
            firstName: firstName,
            lastName: lastName,
            slug: new SlugHelper().GenerateSlug($"{firstName} {lastName}"),
            genderId: faker.PickRandom(genders)
        ));
    }

    public Task<Opc> MakeOpc()
    { 
        var branchIds = Context.Branches.Select(b => b.Id).ToList();
        var firstName = faker.Name.FirstName();
        var lastName = faker.Name.LastName();
        var email = faker.Internet.Email(firstName, lastName);
        var phone = faker.Phone.PhoneNumber();
        var hireDate = faker.Date.Past(5);
        var salary = faker.Finance.Amount(3000, 15000);
        var branchId = faker.PickRandom(branchIds);
        var genderIds = Context.Genders.Select(g => g.Id).ToList();
        var genderId = faker.PickRandom(genderIds);

        return Task.FromResult(Opc.Register(
            firstName: firstName,
            lastName: lastName,
            slug: new SlugHelper().GenerateSlug($"{firstName} {lastName}"),
            genderId: genderId,
            email: email,
            phone: phone,
            dateOfBirth: DateOnly.FromDateTime(faker.Date.Past(30, DateTime.Now.AddYears(-18))),
            hireDate: hireDate,
            salary: salary,
            branchId: branchId
        ));
    }
  
    public Task<CommercialAgent> MakeCA()
    {
        var branchIds = Context.Branches.Select(b => b.Id).ToList();
        var firstName = faker.Name.FirstName();
        var lastName = faker.Name.LastName();
        var email = faker.Internet.Email(firstName, lastName);
        var phone = faker.Phone.PhoneNumber();
        var hireDate = faker.Date.Past(5);
        var salary = faker.Finance.Amount(3000, 15000);
        var branchId = faker.PickRandom(branchIds);
        var genderIds = Context.Genders.Select(g => g.Id).ToList();
        var genderId = faker.PickRandom(genderIds);

        return Task.FromResult(CommercialAgent.Register(
            firstName: firstName,
            lastName: lastName,
            slug: new SlugHelper().GenerateSlug($"{firstName} {lastName}"),
            genderId: genderId,
            email: email,
            phone: phone,
            dateOfBirth: DateOnly.FromDateTime(faker.Date.Past(30, DateTime.Now.AddYears(-18))),
            hireDate: hireDate,
            salary: salary,
            branchId: branchId
        ));
    }
}
