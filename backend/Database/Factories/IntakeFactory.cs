using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Utils;
namespace SchoolManagement.Backend.Database.Factories;

public class IntakeFactory : Factory<Intake>
{
    public IntakeFactory(AppDbContext context) : base(context)
    {
    }
    protected override Intake Make()
    {
        var genderIds = Context.Genders.Select(g => g.Id).ToList();
        var leadSourceIds = Context.LeadSources.Select(l => l.Id).ToList();
        var programIds = Context.SchoolPrograms.Select(sp => sp.Id).ToList();
        var branchIds = Context.Branches.Select(b => b.Id).ToList();
        var agentIds = Context.CommercialAgents.Select(ca => ca.Id).ToList();
        string firstName = faker.Name.FirstName();
        string lastName = faker.Name.LastName();
        decimal totalFees = faker.Finance.Amount(500, 5000);
        decimal amountPaid = faker.Finance.Amount(0, totalFees);
        
        return new Intake
        {   
            FirstName = firstName,
            LastName = lastName,
            Slug = CustomSluger.Slug(firstName, lastName),
            Email = faker.Internet.Email(),
            Phone = faker.Phone.PhoneNumber(),
            DateOfBirth = DateOnly.FromDateTime(faker.Date.Past(30, DateTime.Now.AddYears(-18))), 
            IntakeDate = faker.Date.Past(),
            Status = faker.PickRandom<IntakeStatus>(),
            FollowUpDate = faker.Date.Future(),
            Notes = faker.Lorem.Sentence(),
            GenderId = faker.PickRandom(genderIds),
            LeadSourceId = faker.PickRandom(leadSourceIds),
            SchoolProgramId = faker.PickRandom(programIds),
            BranchId = faker.PickRandom(branchIds),
            CommercialAgentId = faker.PickRandom(agentIds),
            IsIndependent = faker.Random.Bool(),
            TotalFees = totalFees,
            AmountPaid = amountPaid
        };
    }
}