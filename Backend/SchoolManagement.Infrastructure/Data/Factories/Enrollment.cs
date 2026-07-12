using SchoolManagement.Backend.Entities;
using SchoolManagement.Backend.Data ;

namespace SchoolManagement.Backend.Data.Factories;

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