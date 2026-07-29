namespace SchoolManagement.Application.Dtos.Commands;

public class UpdateChargeCommand
{
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
}
