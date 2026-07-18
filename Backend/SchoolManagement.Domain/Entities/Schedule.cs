using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;
namespace SchoolManagement.Domain.Entities;

public class Schedule : AggregateRoot
{
    public Guid BranchId { get; private set; }
    public Guid TeacherId { get; private set; }
    public Guid RoomId { get; private set; }
    public Guid DayId { get; private set; }
    public Guid TimeSlotId { get; private set; }
    public Guid GroupId { get; private set; }
    public Guid SubjectId { get; private set; }

    // navigations  
    public virtual Branch Branch { get; private set; } = null!;
    public virtual Subject Subject { get; private set; } = null!;
    public virtual Group Group { get; private set; } = null!;
    public virtual Teacher Teacher { get; private set; } = null!;
    public virtual Room Room { get; private set; } = null!;
    public virtual TimeSlot TimeSlot { get; private set; } = null!;
    public virtual Day Day { get; private set; } = null!;

    private Schedule() { }

    public static Schedule Create(Guid branchId, Guid teacherId, Guid roomId, Guid dayId, Guid timeSlotId, Guid groupId, Guid subjectId)
    {
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");
        if (teacherId == Guid.Empty)
            throw new DomainException("Teacher ID must not be empty.");
        if (roomId == Guid.Empty)
            throw new DomainException("Room ID must not be empty.");
        if (dayId == Guid.Empty)
            throw new DomainException("Day ID must not be empty.");
        if (timeSlotId == Guid.Empty)
            throw new DomainException("Time Slot ID must not be empty.");
        if (groupId == Guid.Empty)
            throw new DomainException("Group ID must not be empty.");
        if (subjectId == Guid.Empty)
            throw new DomainException("Subject ID must not be empty.");

        return new Schedule
        {
            BranchId = branchId,
            TeacherId = teacherId,
            RoomId = roomId,
            DayId = dayId,
            TimeSlotId = timeSlotId,
            GroupId = groupId,
            SubjectId = subjectId
        };
    }

    public void UpdateBranchId(Guid branchId)
    {
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");
        BranchId = branchId;
    }

    public void UpdateTeacherId(Guid teacherId)
    {
        if (teacherId == Guid.Empty)
            throw new DomainException("Teacher ID must not be empty.");
        TeacherId = teacherId;
    }

    public void UpdateRoomId(Guid roomId)
    {
        if (roomId == Guid.Empty)
            throw new DomainException("Room ID must not be empty.");
        RoomId = roomId;
    }

    public void UpdateDayId(Guid dayId)
    {
        if (dayId == Guid.Empty)
            throw new DomainException("Day ID must not be empty.");
        DayId = dayId;
    }

    public void UpdateTimeSlotId(Guid timeSlotId)
    {
        if (timeSlotId == Guid.Empty)
            throw new DomainException("Time Slot ID must not be empty.");
        TimeSlotId = timeSlotId;
    }

    public void UpdateGroupId(Guid groupId)
    {
        if (groupId == Guid.Empty)
            throw new DomainException("Group ID must not be empty.");
        GroupId = groupId;
    }

    public void UpdateSubjectId(Guid subjectId)
    {
        if (subjectId == Guid.Empty)
            throw new DomainException("Subject ID must not be empty.");
        SubjectId = subjectId;
    }
}

public class Day : BaseEntity
{
    public string Name { get; private set; } = string.Empty;

    private Day() { }

    public static Day Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Day name cannot be empty.");

        return new Day
        {
            Name = name
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Day name cannot be empty.");
        Name = name;
    }
}

public class TimeSlot : BaseEntity
{
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }

    private TimeSlot() { }

    public static TimeSlot Create(TimeOnly startTime, TimeOnly endTime)
    {
        return new TimeSlot
        {
            StartTime = startTime,
            EndTime = endTime
        };
    }

    public void UpdateStartTime(TimeOnly startTime)
    {
        StartTime = startTime;
    }

    public void UpdateEndTime(TimeOnly endTime)
    {
        EndTime = endTime;
    }
}








