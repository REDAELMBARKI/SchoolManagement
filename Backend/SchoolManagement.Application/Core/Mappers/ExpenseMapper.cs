using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Application.Core.Mappers;

public static class ExpenseMapper
{
    public static Expense ToDomain(ExpenseCommand command)
    {
        return Expense.Create(
            category: command.Category,
            payeeName: command.PayeeName,
            amount: command.Amount,
            expenseDate: command.ExpenseDate,
            paymentMethod: command.PaymentMethod,
            branchId: command.BranchId,
            processedByStaffId: command.ProcessedByStaffId,
            description: command.Description,
            reference: command.Reference
        );
    }

    public static ExpenseResponseDto ToResponse(Expense expense)
    {
        return new ExpenseResponseDto
        {
            Id = expense.Id,
            Category = expense.Category,
            PayeeName = expense.PayeeName,
            Description = expense.Description,
            Amount = expense.Amount,
            ExpenseDate = expense.ExpenseDate,
            PaymentMethod = expense.PaymentMethod,
            Reference = expense.Reference,
            ProcessedByStaffId = expense.ProcessedByStaffId,
            BranchId = expense.BranchId,
            CurrencyCode = expense.CurrencyCode,
            CreatedAt = expense.CreatedAt
        };
    }
}
