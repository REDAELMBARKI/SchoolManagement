namespace SchoolManagement.Application.Core.Dtos.Commands;

public class UpdateChargeCommand
{
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
}
