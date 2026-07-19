using FluentValidation;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Domain.Entities;
using DtoLeadSourceType = SchoolManagement.Application.Dtos.Requests.LeadSourceType;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Application.Stratigies.LeadSourceExistence;

namespace SchoolManagement.Application.Validators;


public class IntakeValidator : AbstractValidator<IntakeRequestDto>
{
    public IntakeValidator(
        IGenderQueryService gender_query,
        IBranchQueryService branch_query,
        ICommercialAgentQueryService commercialAgent_query,
        IOpcQueryService opc_query,
        IAdQueryService ad_query ,
        LeadSourceExistenceCheckerResolver _leadSourceResolver
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
        .SetValidator(new LeadSourceValidator(ad_query, opc_query , _leadSourceResolver))
        .When(i => i.IsIndependent == false)
        .WithMessage("Lead source is required when intake is not independent");

        // Subject validation - basic check only (no querysitory available)
        RuleFor(i => i.SubjectId)
        .NotEmpty()
        .WithMessage("Subject must be selected");

        // Gender validation
        RuleFor(i => i.GenderId)
        .MustAsync(async (genderId, ct) =>
        {
            return await gender_query.IsExistsAsync(genderId.Value);
        })
        .When(i => i.GenderId.HasValue)
        .WithMessage("Selected gender does not exist");

        // Branch validation
        RuleFor(i => i.BranchId)
        .NotEmpty()
        .MustAsync(async (branchId, ct) =>
        {
            return await branch_query.IsExistsAsync(branchId);
        })
        .WithMessage("Selected branch does not exist");

        // CommercialAgent validation
        RuleFor(i => i.CommercialAgentId)
        .MustAsync(async (commercialAgentId, ct) =>
        {
            return await commercialAgent_query.IsExistsAsync(commercialAgentId!.Value);
        })
        .When(i => i.CommercialAgentId.HasValue)
        .WithMessage("Selected commercial agent does not exist");
    }
}

public class LeadSourceValidator : AbstractValidator<LeadSourceRequestDto>
{
    public LeadSourceValidator(IAdQueryService ad_query, IOpcQueryService opc_query , LeadSourceExistenceCheckerResolver _resolver)
    {

        // Validate LeadSourceId exists and matches the type
        RuleFor(l => l.SourceId)
            .NotEmpty()
            .MustAsync(async (l , sourceId, ct) =>
            {
                return await _resolver.IsExistsResolver(l.SourceType, sourceId);
            })
            .WithMessage("Lead source ID is required")
            .WithMessage("Selected lead source does not exist or does not match the specified type");
    }
}