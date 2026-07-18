using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class ScheduleMapper
{
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
                LastName = schedule.Teacher.LastName
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
