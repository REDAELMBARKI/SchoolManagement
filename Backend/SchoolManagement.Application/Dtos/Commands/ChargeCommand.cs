using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Dtos.Commands;

public class ChargeCommand
{
    public Guid StudentId { get; set; }
    public Guid BranchId { get; set; }

    public ChargeType ChargeType { get; set; }
    public ChargeStatus Status { get; set; }
    public DateTime IssuedDate { get; set; }

    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; }

    public decimal AmountPaid { get; set; } = 0;

    public Guid? SourceId { get; set; }
}