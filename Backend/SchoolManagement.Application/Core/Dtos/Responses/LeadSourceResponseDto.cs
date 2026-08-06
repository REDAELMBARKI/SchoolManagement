namespace SchoolManagement.Application.Core.Dtos.Responses;

public class LeadSourceResponseDto
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public string Type { get; set; } = string.Empty; // "Ad" or "Opc"
    public DateTime CreatedAt { get; set; }
    
    // Only one of these will be populated based on Type
    public Guid? AdId { get; set; }
    public Guid? OpcId { get; set; }
}
