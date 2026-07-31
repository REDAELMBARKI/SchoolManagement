namespace SchoolManagement.Domain.Results;

public sealed record InvoicePaymentResult(decimal AppliedAmount, decimal OverpaymentAmount);
