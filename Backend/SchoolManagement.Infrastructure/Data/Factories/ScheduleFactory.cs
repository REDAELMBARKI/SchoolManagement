using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data ;
namespace SchoolManagement.Infrastructure.Data.Factories;

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