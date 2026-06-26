using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Contexts ;
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