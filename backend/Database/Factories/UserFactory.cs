using System;
using AutoMapper;
using Bogus;
using Bogus.DataSets;
using SchoolManagement.Backend;
using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend.Dtos;
using SchoolManagement.Backend.Models;

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

        return new UserDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "studentEmail@gmail.com" ,
            Phone = "1234567890",
            GenderId = 1,
            IsActivated = false , 


        };
    }


    public Opc MakeOpc()
    {
        var user = this.Make();
        var opc =  _mapper.Map<Opc>(user);
        opc.HireDate = DateTime.Now.AddYears(-5);
        opc.Salary = 10000;
        return opc;
    }


}
