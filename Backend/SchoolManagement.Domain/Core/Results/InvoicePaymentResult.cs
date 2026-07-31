namespace SchoolManagement.Domain.Core.Results;

public sealed record InvoicePaymentResult(decimal AppliedAmount, decimal OverpaymentAmount);
