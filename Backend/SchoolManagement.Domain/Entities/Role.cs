using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;


public class Role : AggregateRoot
{
    public string Name {get;set;} = string.Empty ;
    public string Slug {get;set;} = string.Empty ;

    public virtual List<User> Users { get; set; } = new List<User>();

}