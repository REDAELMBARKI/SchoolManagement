using SchoolManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Results
{
    public record EvaluatePaymentPlanResult
    {
        public bool IsFullyPaid = false;
        public decimal Amount ;
        public decimal PaidAmount ;
        public decimal AmountRemainingDueDays;
    }
}
