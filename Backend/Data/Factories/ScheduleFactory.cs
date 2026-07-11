using SchoolManagement.Backend.Entities;
using SchoolManagement.Backend.Data ;
namespace SchoolManagement.Backend.Data.Factories;

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