using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Application.Core.Mappers;

public static class LeadSourceMapper
{
    public static LeadSource? ToDomain(LeadSourceResponseDto dto, Guid branchId)
    {
        return dto switch
        {
            AdResponseDto => AdLeadSource.Create(branchId: branchId, adId: dto.Id),
            OpcResponseDto => OpcLeadSource.Create(branchId: branchId, opcId: dto.Id),
            _ => null
        };
    }

    public static LeadSourceResponseDto? MapLeadSource(LeadSource leadSource)
    {
        if (leadSource is AdLeadSource adLeadSource)
        {
            return new AdResponseDto
            {
                Id = adLeadSource.AdId,
                PlatFormName = adLeadSource.Ad.Platform.Name,
                Type = nameof(Ad)
            };
        }

        if (leadSource is OpcLeadSource opcLeadSource)
        {
            return new OpcResponseDto
            {
                Id = opcLeadSource.OpcId,
                FullName = opcLeadSource.Opc.FirstName + " " + opcLeadSource.Opc.LastName,
                Type = nameof(Opc)
            };
        }

        return null;
    }
}
