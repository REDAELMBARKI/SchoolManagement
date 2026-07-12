using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Entities;
using SchoolManagement.Backend.Interfaces.Models;

namespace SchoolManagement.Backend.Mappers;

public static class LeadSourceMapper
{
    public static LeadSourceResponseDto? MapLeadSource(ILeadSource? leadSource)
    {
        if (leadSource is null) return null;

        if (leadSource is  typeof(AdLeadSource))
        {
            return new AdResponseDto
            {
                Id = leadSource.AdId.Value,
                PlatFormName = leadSource.Ad.Platform.Name,
                Type = nameof(Ad)
            };
        }

        if (typeof(leadSource) == typeof(OpcLeadSource))
        {
            return new OpcResponseDto
            {
                Id = leadSource.OpcId.Value,
                FullName = leadSource.Opc.FirstName + " " + leadSource.Opc.LastName,
                Type = nameof(Opc)
            };
        }

        return null;
    }
}
