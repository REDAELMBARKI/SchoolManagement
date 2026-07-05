using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Contexts ;

namespace SchoolManagement.Backend.Database.Factories;

public class EnrollmentFactory : Factory<Enrollment>
{
    public EnrollmentFactory(AppDbContext context) : base(context)
    {
    }

    protected override Task<Enrollment> Make()
    {
        var enrollment = new Enrollment
        {
            
        };
        return Task.FromResult(enrollment);
    }
}