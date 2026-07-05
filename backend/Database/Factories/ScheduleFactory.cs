using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Contexts ;
namespace SchoolManagement.Backend.Database.Factories;

public class ScheduleFactory : Factory<Schedule>
{
    public ScheduleFactory(AppDbContext context) : base(context)
    {
    }

    protected override  Task<Schedule> Make()
    {
        var schedule = new Schedule
        {
            
        };
        return Task.FromResult(schedule);
    }
}