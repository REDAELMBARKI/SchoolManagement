using System;
using Bogus;
using Bogus.DataSets;
using SchoolManagement.Infrastructure.Data ;
 using SchoolManagement.Infrastructure.Data.Factories; 
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
       
        return new User
        {
            FirstName = firstName,
            LastName = lastName,
            Slug = new SlugHelper().GenerateSlug($"{firstName} {lastName}"),
            Email = faker.Internet.Email(firstName, lastName),
            DateOfBirth = dateOfBirth,
            Password = BCrypt.Net.BCrypt.HashPassword("password"),
            IsActivated = true,
            GenderId = faker.PickRandom(genders)
        };
    }

    public async Task<Opc> MakeOpc()
    { 
        var branchIds = Context.Branches.Select(b => b.Id).ToList();
        User user = await this.Make();
        DateTime creationDate = DateTime.Now;
        
        return new Opc
        {
            // Copy properties from User (Person properties)
            FirstName = user.FirstName,
            LastName = user.LastName,
            Slug = user.Slug,
            Email = user.Email,
            Phone = faker.Phone.PhoneNumber(),
            DateOfBirth = user.DateOfBirth,
            GenderId = user.GenderId,
            
            // Employee properties
            HireDate = faker.Date.Past(5),
            Salary = faker.Finance.Amount(3000, 15000),
            BranchId = faker.PickRandom(branchIds),
            CreatedAt = creationDate,
            UpdatedAt = creationDate
        };
    }
  
    public async Task<CommercialAgent> MakeCA()
    {
        var branchIds = Context.Branches.Select(b => b.Id).ToList();
        User user = await this.Make();
        DateTime creationDate = DateTime.Now;

        return new CommercialAgent
        {
            // Copy properties from User (Person properties)
            FirstName = user.FirstName,
            LastName = user.LastName,
            Slug = user.Slug,
            Email = user.Email,
            Phone = faker.Phone.PhoneNumber(),
            DateOfBirth = user.DateOfBirth,
            GenderId = user.GenderId,
            
            // Employee properties
            HireDate = faker.Date.Past(5),
            Salary = faker.Finance.Amount(3000, 15000),
            BranchId = faker.PickRandom(branchIds),
            CreatedAt = creationDate,
            UpdatedAt = creationDate
        };
    }
}
