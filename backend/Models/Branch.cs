using System;

namespace SchoolManagement.Backend.Models;

public class Branch : BaseEntity
{
    public string Name { get; set; }  = string.Empty ;
    public string Slug {get;set;} = string.Empty ;

    public string City { get; set; }   
    public string Address { get; set; } 
    public string? Phone { get; set; }
}