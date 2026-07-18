namespace SchoolManagement.Application.Dtos.Requests;

public class LeadSourceRequestDto
{
    public Guid BranchId { get; set; }
    public string Type { get; set; } = string.Empty;
    public Guid SourceId { get; set; }
}

