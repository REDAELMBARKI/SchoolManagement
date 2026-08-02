using SchoolManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

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
