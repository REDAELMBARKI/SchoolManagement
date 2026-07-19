using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Domain.Utils;
using SchoolManagement.Infrastructure.Repositories;
using Slugify;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class IntakeFactory : Factory<Intake>
{
    private readonly IntakeRepository _repo;
    public IntakeFactory(AppDbContext context, IntakeRepository repo) : base(context)
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
        string firstName = faker.Name.FirstName();
        string lastName = faker.Name.LastName();
        decimal totalFees = faker.Finance.Amount(500, 5000);
        decimal amountPaid = faker.Finance.Amount(0, totalFees);
        string? email = faker.Random.Bool() ? faker.Internet.Email() : null;
        string? phone = faker.Phone.PhoneNumber("###-####");
        DateOnly? dateOfBirth = DateOnly.FromDateTime(faker.Date.Past(30, DateTime.Now.AddYears(-18)));
        DateTime intakeDate = faker.Date.Past();
        IntakeStatus status = faker.PickRandom<IntakeStatus>();
        DateTime? followUpDate = faker.Date.Future();
        string? notes = faker.Lorem.Sentence();
        Guid? genderId = faker.PickRandom(genderIds);
        Guid? leadSourceId = leadSourceIds.Any() ? faker.PickRandom(leadSourceIds) : null;
        Guid subjectId = faker.PickRandom(subjectIds);
        Guid branchId = faker.PickRandom(branchIds);
        Guid? commercialAgentId = agentIds.Any() ? faker.PickRandom(agentIds) : null;
        bool isIndependent = faker.Random.Bool();

        return Intake.Register(
            firstName: firstName,
            lastName: lastName,
            slug: this.GenerateSlug(firstName, lastName),
            genderId: genderId,
            email: email,
            phone: phone,
            dateOfBirth: dateOfBirth,
            intakeDate: intakeDate,
            status: status,
            followUpDate: followUpDate,
            notes: notes,
            commercialAgentId: commercialAgentId,
            leadSourceId: leadSourceId,
            subjectId: subjectId,
            branchId: branchId,
            isIndependent: isIndependent,
            totalFees: totalFees,
            amountPaid: amountPaid
        );
    }
}