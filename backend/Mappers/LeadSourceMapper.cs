using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Mappers;

public static class LeadSourceMapper
{
    public static LeadSourceResponseDto? MapLeadSource(LeadSource? leadSource)
    {
        if (leadSource is null) return null;

        if (leadSource.AdId != null && leadSource.Ad != null)
        {
            return new AdResponseDto
            {
                Id = leadSource.AdId.Value,
                PlatFormName = leadSource.Ad.Platform.Name,
                Type = nameof(Ad)
            };
        }

        if (leadSource.OpcId != null && leadSource.Opc != null)
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
