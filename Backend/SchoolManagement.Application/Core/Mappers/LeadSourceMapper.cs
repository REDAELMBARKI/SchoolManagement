using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Application.Core.Mappers;

public static class LeadSourceMapper
{
    public static AdLeadSource ToDomain(AdLeadSourceCommand command)
    {
        return AdLeadSource.Create(
            branchId: command.BranchId,
            adId: command.AdId
        );
    }

    public static OpcLeadSource ToDomain(OpcLeadSourceCommand command)
    {
        return OpcLeadSource.Create(
            branchId: command.BranchId,
            opcId: command.OpcId
        );
    }

    public static LeadSourceResponseDto ToResponse(LeadSource leadSource)
    {
        return leadSource switch
        {
            AdLeadSource adLead => new LeadSourceResponseDto
            {
                Id = adLead.Id,
                BranchId = adLead.BranchId,
                Type = "Ad",
                AdId = adLead.AdId,
                OpcId = null,
                CreatedAt = adLead.CreatedAt
            },
            OpcLeadSource opcLead => new LeadSourceResponseDto
            {
                Id = opcLead.Id,
                BranchId = opcLead.BranchId,
                Type = "Opc",
                AdId = null,
                OpcId = opcLead.OpcId,
                CreatedAt = opcLead.CreatedAt
            },
            _ => throw new InvalidOperationException($"Unknown LeadSource type: {leadSource.GetType().Name}")
        };
    }
}
