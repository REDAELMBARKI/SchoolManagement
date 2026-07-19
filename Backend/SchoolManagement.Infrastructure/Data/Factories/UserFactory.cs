using System;
using Bogus;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Domain.Entities;
using Slugify;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class UserFactory : Factory<User>
{
    public UserFactory(AppDbContext context) : base(context)
    {
    }

    protected override async Task<User> Make()
    {   
        var genders = Context.Genders.Select(g => g.Id).ToList();
        var firstName = faker.Name.FirstName();
        var lastName = faker.Name.LastName();
        var dateOfBirth = DateOnly.FromDateTime(faker.Date.Past(30, DateTime.Now.AddYears(-18)));
        var email = faker.Internet.Email(firstName, lastName);
       
        return User.Register(
            firstName: firstName,
            lastName: lastName,
            slug: new SlugHelper().GenerateSlug($"{firstName} {lastName}"),
            genderId: faker.PickRandom(genders),
            email: email,
            password: BCrypt.Net.BCrypt.HashPassword("password"),
            dateOfBirth: dateOfBirth,
            isActivated: true
        );
    }

    public async Task<Opc> MakeOpc()
    { 
        var branchIds = Context.Branches.Select(b => b.Id).ToList();
        var firstName = faker.Name.FirstName();
        var lastName = faker.Name.LastName();
        var dateOfBirth = DateOnly.FromDateTime(faker.Date.Past(30, DateTime.Now.AddYears(-18)));
        var email = faker.Internet.Email(firstName, lastName);
        var phone = faker.Phone.PhoneNumber();
        var hireDate = faker.Date.Past(5);
        var salary = faker.Finance.Amount(3000, 15000);
        var branchId = faker.PickRandom(branchIds);
        var genderIds = Context.Genders.Select(g => g.Id).ToList();
        var genderId = faker.PickRandom(genderIds);

        return Opc.Register(
            firstName: firstName,
            lastName: lastName,
            slug: new SlugHelper().GenerateSlug($"{firstName} {lastName}"),
            genderId: genderId,
            email: email,
            phone: phone,
            dateOfBirth: dateOfBirth,
            hireDate: hireDate,
            salary: salary,
            branchId: branchId
        );
    }
  
    public async Task<CommercialAgent> MakeCA()
    {
        var branchIds = Context.Branches.Select(b => b.Id).ToList();
        var firstName = faker.Name.FirstName();
        var lastName = faker.Name.LastName();
        var dateOfBirth = DateOnly.FromDateTime(faker.Date.Past(30, DateTime.Now.AddYears(-18)));
        var email = faker.Internet.Email(firstName, lastName);
        var phone = faker.Phone.PhoneNumber();
        var hireDate = faker.Date.Past(5);
        var salary = faker.Finance.Amount(3000, 15000);
        var branchId = faker.PickRandom(branchIds);
        var genderIds = Context.Genders.Select(g => g.Id).ToList();
        var genderId = faker.PickRandom(genderIds);

        return CommercialAgent.Register(
            firstName: firstName,
            lastName: lastName,
            slug: new SlugHelper().GenerateSlug($"{firstName} {lastName}"),
            genderId: genderId,
            email: email,
            phone: phone,
            dateOfBirth: dateOfBirth,
            hireDate: hireDate,
            salary: salary,
            branchId: branchId
        );
    }
}
