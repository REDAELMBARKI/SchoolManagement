using FluentValidation;
using SchoolManagement.Backend.Dtos;
using SchoolManagement.Backend.Interfaces.Repos;
using SchoolManagement.Backend.Models;
namespace SchoolManagement.Backend.Validators;


public class IntakeValidator : AbstractValidator<IntakeRequestDto>
{
    public IntakeValidator(
        IGenderRepository gender_repo,
        IBranchRepository branch_repo,
        ICommercialAgentRepository commercialAgent_repo,
        IOpcRepository opc_repo,
        IAdRepository ad_repo
    )
    {
        // Basic property validations
        RuleFor(i => i.FirstName).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(i => i.LastName).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(i => i.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(i => i.Phone).NotEmpty().MaximumLength(20);
        RuleFor(i => i.IntakeDate).NotEmpty().LessThanOrEqualTo(DateTime.Today);

        // Date validations
        RuleFor(i => i.DateOfBirth)
            .Must(date => date.HasValue && date.Value < DateOnly.FromDateTime(DateTime.Today.AddYears(-5)))
            .When(i => i.DateOfBirth.HasValue)
            .WithMessage("Date of birth must be at least 16 years ago");

        RuleFor(i => i.FollowUpDate)
            .GreaterThan(i => i.IntakeDate)
            .When(i => i.FollowUpDate.HasValue)
            .WithMessage("Follow-up date must be after intake date");

        // Financial validations
        RuleFor(i => i.TotalFees).GreaterThan(0).WithMessage("Total fees must be greater than 0");
        RuleFor(i => i.AmountPaid).GreaterThanOrEqualTo(0).WithMessage("Amount paid must be non-negative");
        RuleFor(i => i.AmountPaid)
            .LessThanOrEqualTo(i => i.TotalFees)
            .WithMessage("Amount paid cannot exceed total fees");

        // LeadSource validation (only when not independent)
        RuleFor(i => i.LeadSource)
        .NotEmpty()
        .SetValidator(new LeadSourceValidator(ad_repo, opc_repo))
        .When(i => i.IsIndependent == false)
        .WithMessage("Lead source is required when intake is not independent");

        // Subject validation - basic check only (no repository available)
        RuleFor(i => i.SubjectId)
        .NotEmpty()
        .GreaterThan(0)
        .WithMessage("Subject must be selected");

        // Gender validation
        RuleFor(i => i.GenderId)
        .MustAsync(async (genderId, ct) =>
        {
            return await gender_repo.ExistsAsync(genderId.Value);
        })
        .When(i => i.GenderId.HasValue)
        .WithMessage("Selected gender does not exist");

        // Branch validation
        RuleFor(i => i.BranchId)
        .NotEmpty()
        .MustAsync(async (branchId, ct) =>
        {
            return await branch_repo.ExistsAsync(branchId);
        })
        .WithMessage("Selected branch does not exist");

        // CommercialAgent validation
        RuleFor(i => i.CommercialAgentId)
        .MustAsync(async (commercialAgentId, ct) =>
        {
            return await commercialAgent_repo.ExistsAsync(commercialAgentId!.Value);
        })
        .When(i => i.CommercialAgentId.HasValue)
        .WithMessage("Selected commercial agent does not exist");
    }
}

public class LeadSourceValidator : AbstractValidator<LeadSourceDto>
{
    public LeadSourceValidator(IAdRepository ad_repo, IOpcRepository opc_repo)
    {
        // Validate LeadSourceType first
        RuleFor(l => l.LeadSourceType)
            .Must(type => type == LeadSourceType.Opc || type == LeadSourceType.Ad)
            .WithMessage("Lead source type must be either OPC or Ad");

        // Validate LeadSourceId exists and matches the type
        RuleFor(l => l.LeadSourceId)
            .NotEmpty()
            .WithMessage("Lead source ID is required")
            .MustAsync(async (dto, leadSourceId, ct) => {
                return dto.LeadSourceType switch {
                    LeadSourceType.Opc => await opc_repo.ExistsAsync(leadSourceId.Value!) ,
                    LeadSourceType.Ad => await ad_repo.ExistsAsync(leadSourceId.Value!) ,
                    _ => false
                };
            })
            .WithMessage("Selected lead source does not exist or does not match the specified type");
    }
}