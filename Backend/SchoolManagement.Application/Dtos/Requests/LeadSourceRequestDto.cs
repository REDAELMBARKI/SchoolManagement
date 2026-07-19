namespace SchoolManagement.Application.Dtos.Requests;

public class LeadSourceRequestDto
{
    public LeadSourceType SourceType { get; set; } = LeadSourceType.Opc;
    public Guid SourceId { get; set; }
}

public enum LeadSourceType
{
    Opc,
    Ad
}