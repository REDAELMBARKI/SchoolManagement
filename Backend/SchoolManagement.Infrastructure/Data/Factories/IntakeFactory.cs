using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Entities;
using SchoolManagement.Backend.Data ;

using SchoolManagement.Backend.Utils;
using SchoolManagement.Backend.Repositories;
using Slugify;
namespace SchoolManagement.Backend.Data.Factories;

public class IntakeFactory : Factory<Intake>
{

    private readonly IntakeRepository _repo;
    public IntakeFactory(AppDbContext context , IntakeRepository repo) : base(context)
    {
        _repo = repo;
    }
    protected override async Task<Intake> Make()
    {
        var genderIds = Context.Genders.Select(g => g.Id).ToList();
        var leadSourceIds = Context.LeadSources.Select(l => l.Id).ToList();
        var subjectIds = Context.Subjects.Select(s => s.Id).ToList();
        var branchIds = Context.Branches.Select(b => b.Id).ToList();
        var agentIds = Context.CommercialAgents.Select(ca => ca.Id).ToList();
        var studentIds = Context.Students.Select(s => s.Id).ToList();
        string firstName = faker.Name.FirstName();
        string lastName = faker.Name.LastName();
        decimal totalFees = faker.Finance.Amount(500, 5000);
        decimal amountPaid = faker.Finance.Amount(0, totalFees);
        
        return new Intake
        {   
            FirstName = firstName,
            LastName = lastName,
            Slug =  this.GenerateSlug(firstName , lastName),
            Email = faker.Random.Bool() ? faker.Internet.Email() : null,
            Phone = faker.Phone.PhoneNumber("###-####"),
            DateOfBirth = DateOnly.FromDateTime(faker.Date.Past(30, DateTime.Now.AddYears(-18))), 
            IntakeDate = faker.Date.Past(),
            Status = faker.PickRandom<IntakeStatus>(),
            FollowUpDate = faker.Date.Future(),
            Notes = faker.Lorem.Sentence(),
            GenderId = faker.PickRandom(genderIds),
            LeadSourceId = faker.PickRandom(leadSourceIds),
            SubjectId = faker.PickRandom(subjectIds),
            BranchId = faker.PickRandom(branchIds),
            CommercialAgentId = faker.PickRandom(agentIds),
            IsIndependent = faker.Random.Bool(),
            TotalFees = totalFees,
            AmountPaid = amountPaid
        };
    }
}