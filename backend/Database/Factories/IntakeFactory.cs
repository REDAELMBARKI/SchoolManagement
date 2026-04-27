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
            FirstName = faker.Name.FirstName(),
            LastName = faker.Name.LastName(),
            Email = faker.Internet.Email(),
            Phone = faker.Phone.PhoneNumber(),
            DateOfBirth = faker.Date.Past(30, DateTime.Now.AddYears(-18)), 
            IntakeDate = faker.Date.Past(),
            Status = faker.PickRandom<IntakeStatus>(),
            FollowUpDate = faker.Date.Future(),
            Notes = faker.Lorem.Sentence(),
            GenderId = Context.Genders.OrderBy(g => Guid.NewGuid()).Select(g => g.Id).First(),
            LeadSourceId = Context.LeadSources.OrderBy(l => Guid.NewGuid()).Select(l => l.Id).First(),
            SchoolProgramId = Context.SchoolPrograms.OrderBy(sp => Guid.NewGuid()).Select(sp => sp.Id).First(),
            BranchId = Context.Branches.OrderBy(b => Guid.NewGuid()).Select(b => b.Id).First(),
            CommercialAgentId = Context.CommercialAgents.OrderBy(ca => Guid.NewGuid()).Select(ca => ca.Id).First()
        };
    }
}