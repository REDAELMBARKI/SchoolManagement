using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Models;
namespace SchoolManagement.Backend.Database.Factories;

public class IntakeFactory : Factory<Intake>
{
    public IntakeFactory(AppDbContext context) : base(context)
    {
    }
    protected override Intake Make()
    {
        return new Intake
        {   
             FirstName = faker.Name.FirstName() ,
             LastName = faker.Name.LastName(),
             Email = faker.Internet.Email()   , 
             IntakeDate = faker.Date.Past(),
             Phone = faker.Phone.PhoneNumber(),
             GenderId = Context.Genders.OrderBy(g => Guid.NewGuid()).Select(g => g.Id).First(),
             LeadSourceId = Context.LeadSources.OrderBy(l => Guid.NewGuid()).Select(l => l.Id).First(),
        };
    }
}