using SchoolManagement.Domain.Common;
using System;

namespace SchoolManagement.Domain.Entities;

public class Gender : AggregateRoot
{
    public string Slug {get;set;} = string.Empty ;

   public string Name {get;set;}  = string.Empty;

}
