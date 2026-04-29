using System;

namespace SchoolManagement.Backend.Models;

public class Gender : BaseEntity
{
    public string Slug {get;set;} = string.Empty ;

   public string Name {get;set;}  = string.Empty;

}
