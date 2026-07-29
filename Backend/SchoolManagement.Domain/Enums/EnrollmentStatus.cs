using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Domain.Enums
{
    public enum EnrollmentStatus
    {
        Active,      // currently enrolled, attending / in progress
        Dropped,     // student left before finishing (cancelled)
        Completed    // course/term finished (regardless of payment)
    }
}
