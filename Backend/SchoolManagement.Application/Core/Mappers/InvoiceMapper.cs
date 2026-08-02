using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Core.Mappers;

public static class InvoiceMapper
{
    public static Invoice ToDomain(InvoiceCommand command)
    {
        var invoice = Invoice.Create(
            enrollmentId: command.EnrollmentId,
            periodStart: command.PeriodStart,
            periodEnd: command.PeriodEnd,
            dueDate: command.DueDate,
            branchId: command.BranchId
        );

        if (command.Charge != null)
        {
            var charge = Charge.Create(
                invoiceId: invoice.Id,
                amount: command.Charge.Amount,
                dueDate: command.Charge.DueDate != default ? command.Charge.DueDate : invoice.DueDate
            );
            invoice.AddCharge(charge);
        }

        return invoice;
    }

    public static InvoiceResponseDto ToResponse(Invoice invoice)
    {
        return new InvoiceResponseDto
        {
            Id = invoice.Id,
            EnrollmentId = invoice.EnrollmentId,
            PeriodStart = invoice.PeriodStart,
            PeriodEnd = invoice.PeriodEnd,
            DueDate = invoice.DueDate,
            TotalAmount = invoice.TotalAmount,
            PaidAmount = invoice.PaidAmount,
            CreditAppliedAmount = invoice.CreditAppliedAmount,
            Status = invoice.Status,
            BranchId = invoice.BranchId,
            Charge = invoice.Charge == null ? null : ChargeMapper.ToResponse(invoice.Charge)
        };
    }
}
