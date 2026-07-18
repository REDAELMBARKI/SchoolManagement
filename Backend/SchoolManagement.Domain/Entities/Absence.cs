
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;
namespace SchoolManagement.Domain.Entities;

public class Absence : AggregateRoot
{
    [Required]
    public DateTime Date { get; private set; } = DateTime.UtcNow;

    // Absent / Late
    [Required, MaxLength(20)]
    public string Status { get; private set; } = "Absent";

    public bool IsJustified { get; private set; } = false;

    [MaxLength(500)]
    public string? Reason { get; private set; }

    // FKs
    public Guid StudentId { get; private set; }
    public Guid ScheduleId { get; private set; }

    // navigations
    public virtual Student Student { get; private set; } = null!;
    public virtual Schedule Schedule { get; private set; } = null!;

    private Absence() { }

    public static Absence Create(Guid studentId, Guid scheduleId, DateTime? date = null, string status = "Absent", bool isJustified = false, string? reason = null)
    {
        if (studentId == Guid.Empty)
            throw new DomainException("Student ID must not be empty.");
        if (scheduleId == Guid.Empty)
            throw new DomainException("Schedule ID must not be empty.");
        if (string.IsNullOrWhiteSpace(status))
            throw new DomainException("Status cannot be empty.");

        return new Absence
        {
            StudentId = studentId,
            ScheduleId = scheduleId,
            Date = date ?? DateTime.UtcNow,
            Status = status,
            IsJustified = isJustified,
            Reason = reason
        };
    }

    public void UpdateDate(DateTime date)
    {
        Date = date;
    }

    public void UpdateStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new DomainException("Status cannot be empty.");
        Status = status;
    }

    public void UpdateIsJustified(bool isJustified)
    {
        IsJustified = isJustified;
    }

    public void UpdateReason(string? reason)
    {
        Reason = reason;
    }

    public void UpdateStudentId(Guid studentId)
    {
        if (studentId == Guid.Empty)
            throw new DomainException("Student ID must not be empty.");
        StudentId = studentId;
    }

    public void UpdateScheduleId(Guid scheduleId)
    {
        if (scheduleId == Guid.Empty)
            throw new DomainException("Schedule ID must not be empty.");
        ScheduleId = scheduleId;
    }
}