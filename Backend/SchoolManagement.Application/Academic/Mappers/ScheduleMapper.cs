using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Application.Common.Mappers;

namespace SchoolManagement.Application.Academic.Mappers;

public static class ScheduleMapper
{
    public static Schedule ToDomain(ScheduleCommand command)
    {
        return Schedule.Create(
            branchId: command.BranchId,
            teacherId: command.TeacherId,
            roomId: command.RoomId,
            dayId: command.DayId,
            timeSlotId: command.TimeSlotId,
            groupId: command.GroupId,
            subjectId: command.SubjectId
        );
    }

    public static ScheduleResponseDto ToResponse(Schedule schedule)
    {
        return new ScheduleResponseDto
        {
            Id = schedule.Id,
            Branch = schedule.Branch != null ? BranchMapper.ToResponse(schedule.Branch) : null,
            Teacher = schedule.Teacher != null ? new TeacherResponseDto
            {
                Id = schedule.Teacher.Id,
                FirstName = schedule.Teacher.FirstName,
                LastName = schedule.Teacher.LastName,
                Email = schedule.Teacher.Email?.Value ?? null,
                Phone = schedule.Teacher.Phone
            } : null,
            Room = schedule.Room != null ? new RoomResponseDto
            {
                Id = schedule.Room.Id,
                Name = schedule.Room.Name,
                Capacity = schedule.Room.Capacity,
                Floor = schedule.Room.Floor
            } : null,
            Day = schedule.Day != null ? new DayResponseDto
            {
                Id = schedule.Day.Id,
                Name = schedule.Day.Name
            } : null,
            TimeSlot = schedule.TimeSlot != null ? new TimeSlotResponseDto
            {
                Id = schedule.TimeSlot.Id,
                StartTime = schedule.TimeSlot.StartTime,
                EndTime = schedule.TimeSlot.EndTime
            } : null,
            Group = schedule.Group != null ? new GroupResponseDto
            {
                Id = schedule.Group.Id,
                Name = schedule.Group.Name,
                Capacity = schedule.Group.Capacity,
                Period = schedule.Group.Period
            } : null,
            Subject = schedule.Subject != null ? new SubjectResponseDto
            {
                Id = schedule.Subject.Id,
                Name = schedule.Subject.Name,
                Slug = schedule.Subject.Slug
            } : null
        };
    }
}
