using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Domain.Entities.Payment
{
    public class PaymentAllocation : BaseEntity
    {
            public Guid PaymentId { get; set; }
            public Payment Payment { get; set; } = null!; 

            public Guid EnrollmentId { get; set; }
            public Enrollment Enrollment { get; set; } = null!;

            public decimal AllocatedAmount { get; set; }  // how much of the payment went to THIS enrollment
    }
}
