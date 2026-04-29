using System.ComponentModel;
using System.ComponentModel.DataAnnotations ;
namespace SchoolManagement.Backend.Models ;  

public class UserRole
{
    public int UserId {get;set;}
    public int RoleId {get;set;}

    // nnv 

    public User User {get;set;} = null! ;
    public Role Role {get;set;} = null! ;
}
