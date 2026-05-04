using System;
using Bogus;
using Bogus.DataSets;
using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend.Models;
using Slugify;

namespace SchoolManagement.Backend.Database.Factories;

public class UserFactory : Factory<User>
{
    public UserFactory(AppDbContext context) : base(context)
    {
    }

    protected override User Make()
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
            Phone = faker.Random.Bool() ? faker.Phone.PhoneNumber() : null,
            DateOfBirth = dateOfBirth,
            Password = BCrypt.Net.BCrypt.HashPassword("password"),
            IsActivated = true,
            GenderId = faker.PickRandom(genders)
        };
    }

    public Opc MakeOpc()
    { 
        var branchIds = Context.Branches.Select(b => b.Id).ToList();
        User user = this.Make();
        DateTime creationDate = DateTime.Now;
        
        return new Opc
        {
            // Copy properties from User (Person properties)
            FirstName = user.FirstName,
            LastName = user.LastName,
            Slug = user.Slug,
            Email = user.Email,
            Phone = user.Phone,
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
  
    public CommercialAgent MakeCa()
    {
        var branchIds = Context.Branches.Select(b => b.Id).ToList();
        User user = this.Make();
        DateTime creationDate = DateTime.Now;
        
        return new CommercialAgent
        {
            // Copy properties from User (Person properties)
            FirstName = user.FirstName,
            LastName = user.LastName,
            Slug = user.Slug,
            Email = user.Email,
            Phone = user.Phone,
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
