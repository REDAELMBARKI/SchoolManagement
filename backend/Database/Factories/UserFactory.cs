using SchoolManagement.Models;

namespace SchoolManagement.Database.Factories ; 
public abstract class UserFactory<TUser> : Factory<TUser> where TUser : User 
{
    protected override TUser Make()
    {
        TUser User = Create() ;
        User.FirstName    = faker.Name.FirstName();
        User.LastName     = faker.Name.LastName();
        User.Email        = faker.Internet.Email();
        User.Phone  = faker.Phone.PhoneNumber();
        return User ;

    }

    protected abstract TUser Create();
}