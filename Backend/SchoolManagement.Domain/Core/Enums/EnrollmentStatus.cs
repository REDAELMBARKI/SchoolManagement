namespace SchoolManagement.Domain.Core.Enums;

public enum EnrollmentStatus
{
    Active,      // currently enrolled, attending / in progress
    Dropped,     // student left before finishing (cancelled)
    Completed    // course/term finished (regardless of payment)
}
