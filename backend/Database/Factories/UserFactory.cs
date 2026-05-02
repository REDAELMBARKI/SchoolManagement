using System;
using AutoMapper;
using Bogus;
using Bogus.DataSets;
using SchoolManagement.Backend;
using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend.Database.Seeders;
using SchoolManagement.Backend.Dtos;
using SchoolManagement.Backend.Models;
using Slugify;

namespace SchoolManagement.Backend.Database.Factories;

public class UserFactory  : Factory<UserDto>
{

    protected IMapper _mapper ; 
    public UserFactory(AppDbContext context , IMapper mapper) : base(context)
    {
        _mapper = mapper;
    }

    protected override UserDto Make()
    {   
       var genders =  Context.Genders.Select(g => g.Id).ToList();
       var firstName = faker.Name.FirstName();
       var lastName = faker.Name.LastName();
        return new UserDto
        {
            FirstName = firstName,
            LastName = lastName,
            Slug = new SlugHelper().GenerateSlug($"{firstName} {lastName}"),
            Email = faker.Internet.Email(firstName, lastName),
            Phone = faker.Phone.PhoneNumber(),
            DateOfBirth = faker.Date.Past(30, DateTime.Now.AddYears(-18)),
            Password = BCrypt.Net.BCrypt.HashPassword("password"),
            IsActivated = true,
            GenderId = faker.PickRandom(genders)
        };
    }


    public Opc MakeOpc()
    { 

        
        var branchIds = Context.Branches.Select(b => b.Id).ToList();
        UserDto user = this.Make();
        var opc =  _mapper.Map<Opc>(user);
        DateTime creationDate = DateTime.Now ;
        opc.HireDate = faker.Date.Past(5) ;
        opc.Salary = faker.Finance.Amount(3000, 15000);
        opc.BranchId = faker.PickRandom(branchIds);
        opc.CreatedAt = creationDate ;
        opc.UpdatedAt = creationDate;

        return opc;
    }
  
    public CommercialAgent MakeCa()
    {
        var branchIds = Context.Branches.Select(b => b.Id).ToList();
        var user =  this.Make() ;
        var ca = _mapper.Map<CommercialAgent>(user) ;
        DateTime creationDate = DateTime.Now ;
        ca.CreatedAt = creationDate ;
        ca.UpdatedAt = creationDate;
        ca.HireDate = faker.Date.Past(5) ;
        ca.Salary = faker.Finance.Amount(3000, 15000);
        ca.BranchId = faker.PickRandom(branchIds);
        return ca ;
    }

}
