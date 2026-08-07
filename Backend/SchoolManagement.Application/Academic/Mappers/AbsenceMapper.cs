using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;

namespace SchoolManagement.Application.Academic.Mappers;

public static class AbsenceMapper
{
    public static Absence ToDomain(AbsenceCommand command)
    {
        return Absence.Create(
            studentId: command.StudentId,
            scheduleId: command.ScheduleId,
            branchId: command.BranchId,
            date: command.Date,
            status: command.Status,
            isJustified: command.IsJustified,
            reason: command.Reason);
    }

    public static AbsenceResponseDto ToResponse(Absence absence)
    {
        return new AbsenceResponseDto
        {
            Id = absence.Id,
            StudentId = absence.StudentId,
            ScheduleId = absence.ScheduleId,
            BranchId = absence.BranchId,
            Date = absence.Date,
            Status = absence.Status,
            IsJustified = absence.IsJustified,
            Reason = absence.Reason,
            CreatedAt = absence.CreatedAt
        };
    }
}
