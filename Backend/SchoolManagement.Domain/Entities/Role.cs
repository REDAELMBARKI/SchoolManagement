using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;


public class Role : BaseEntity
{
    public string Name {get;set;} = string.Empty ;
    public string Slug {get;set;} = string.Empty ;
}