using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Application.Mappers;

public static class LeadSourceMapper
{
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
