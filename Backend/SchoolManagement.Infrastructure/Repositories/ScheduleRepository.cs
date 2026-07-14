using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data ;

namespace SchoolManagement.Infrastructure.Repositories;

public class ScheduleRepository : Repository<Schedule>
{
    public ScheduleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<GroupedScheduleDto> GetGroupSchedule(int groupId)
    {
        var flat = await Context.Schedules
            .Where(sc => sc.GroupId == groupId)
            .Select(sc => new
            {
                DayName = sc.Day.Name,
                StartTime = sc.TimeSlot.StartTime,
                EndTime = sc.TimeSlot.EndTime,
                TeacherFullName = sc.Teacher.FirstName + " " + sc.Teacher.LastName,
                RoomName = sc.Room.Name,
                SubjectName = sc.Subject.Name
            })
            .ToListAsync();

        var days = flat
            .GroupBy(sc => sc.DayName)
            .Select(g => new DayScheduleDto
            {
                DayName = g.Key,
                TimeSlots = g.Select(sc => new TimeSlotScheduleDto
                {
                    StartTime = sc.StartTime,
                    EndTime = sc.EndTime,
                    TeacherFullName = sc.TeacherFullName,
                    RoomName = sc.RoomName,
                    SubjectName = sc.SubjectName
                }).ToList()
            }).ToList();

        return new GroupedScheduleDto { Days = days };
    }
}
