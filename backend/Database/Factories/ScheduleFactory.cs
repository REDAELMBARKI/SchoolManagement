using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Database.Factories;

public class ScheduleFactory : Factory<Schedule>
{
    public ScheduleFactory(AppDbContext context) : base(context)
    {
    }

    protected override Schedule Make()
    {
        return new Schedule
        {
            
        };
    }
}