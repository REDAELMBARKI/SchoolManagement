namespace SchoolManagement.Application.Dtos.Commands;

public class StudentRegistrationCommand
{
    public StudentCommand StudentCommand { get; set; } = null!;
    public EnrollmentCommand EnrollmentCommand { get; set; } = null!;
    public PaymentCommand PaymentCommand { get; set; } = null!;
    public ChargeCommand? ChargeCommand { get; set; }
}
