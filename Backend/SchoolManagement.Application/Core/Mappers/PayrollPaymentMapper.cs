using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Application.Core.Mappers;

public static class PayrollPaymentMapper
{
    public static PayrollPayment ToDomain(PayrollPaymentCommand command, Guid branchId, Guid processedByStaffId)
    {
        return PayrollPayment.Create(
            employeeId: command.EmployeeId,
            grossAmount: command.GrossAmount,
            payPeriodMonth: command.PayPeriodMonth,
            payPeriodYear: command.PayPeriodYear,
            branchId: branchId,
            processedByStaffId: processedByStaffId,
            bonus: command.Bonus,
            deductions: command.Deductions,
            notes: command.Notes
        );
    }

    public static PayrollPaymentResponseDto ToResponse(PayrollPayment payroll)
    {
        return new PayrollPaymentResponseDto
        {
            Id = payroll.Id,
            EmployeeId = payroll.EmployeeId,
            GrossAmount = payroll.GrossAmount,
            Bonus = payroll.Bonus,
            Deductions = payroll.Deductions,
            NetAmount = payroll.NetAmount,
            PayPeriodMonth = payroll.PayPeriodMonth,
            PayPeriodYear = payroll.PayPeriodYear,
            Status = payroll.Status,
            PaidAt = payroll.PaidAt,
            PaymentMethod = payroll.PaymentMethod,
            ReferenceCode = payroll.ReferenceCode,
            BranchId = payroll.BranchId,
            ProcessedByStaffId = payroll.ProcessedByStaffId,
            Notes = payroll.Notes,
            CreatedAt = payroll.CreatedAt
        };
    }
}
