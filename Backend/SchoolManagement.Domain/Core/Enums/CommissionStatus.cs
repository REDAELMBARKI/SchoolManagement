namespace SchoolManagement.Domain.Core.Enums;

public enum CommissionStatus
{
    Approved,   // enrollment active, will be paid on salary day
    Blocked,    // enrollment dropped/cancelled or manually blocked — will not be paid
    Paid        // salary day passed, was Approved → now Paid (locked forever)
}
