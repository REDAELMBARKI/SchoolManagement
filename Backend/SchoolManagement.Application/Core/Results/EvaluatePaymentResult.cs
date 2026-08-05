namespace SchoolManagement.Application.Core.Results
{
    public record EvaluatePaymentPlanResult
    {
        public bool IsFullyPaid = false;
        public decimal TotalAmount;
        public decimal RemainingAmount;
        public decimal PaidAmount ;
        public int RemainingAmountDueDays;
        public decimal CreditBalance;

    }
}
