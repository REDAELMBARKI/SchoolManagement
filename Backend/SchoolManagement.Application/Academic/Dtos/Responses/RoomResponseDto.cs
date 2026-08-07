namespace SchoolManagement.Application.Academic.Dtos.Responses;

public class RoomResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string? Floor { get; set; }
    public string? Description { get; set; }
    public Guid BranchId { get; set; }
    public DateTime CreatedAt { get; set; }
}
