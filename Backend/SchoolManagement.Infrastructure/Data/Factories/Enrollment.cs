using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Factories;

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