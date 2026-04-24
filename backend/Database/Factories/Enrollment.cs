using SchoolManagement.Backend.Models;
namespace SchoolManagement.Backend.Database.Factories;

public class EnrollmentFactory : Factory<Enrollment>
{
    public EnrollmentFactory(AppDbContext context) : base(context)
    {
    }

    protected override Enrollment Make()
    {
        return new Enrollment
        {
            
        };
    }
}