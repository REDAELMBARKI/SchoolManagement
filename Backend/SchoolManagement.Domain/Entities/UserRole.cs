using System.ComponentModel;
using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Domain.Entities;

public class UserRole
{
    public int UserId {get;set;}
    public int RoleId {get;set;}

    // nnv 

    public User User {get;set;} = null! ;
    public Role Role {get;set;} = null! ;
}
