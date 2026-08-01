using MediatR;

namespace SchoolManagement.Domain.Core.DomainEvents;

public class PayrollPaidDomainEvent : INotification
{
    public Guid PayrollPaymentId { get; }
    public Guid EmployeeId { get; }
    public decimal NetAmount { get; }
    public int PayPeriodMonth { get; }
    public int PayPeriodYear { get; }
    public DateTime OccurredAt { get; }

    public PayrollPaidDomainEvent(
        Guid payrollPaymentId,
        Guid employeeId,
        decimal netAmount,
        int payPeriodMonth,
        int payPeriodYear)
    {
        PayrollPaymentId = payrollPaymentId;
        EmployeeId = employeeId;
        NetAmount = netAmount;
        PayPeriodMonth = payPeriodMonth;
        PayPeriodYear = payPeriodYear;
        OccurredAt = DateTime.UtcNow;
    }
}
