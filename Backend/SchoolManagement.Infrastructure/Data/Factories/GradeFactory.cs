using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data ;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class GradeFactory : Factory<Grade>
{
    public GradeFactory(AppDbContext context) : base(context)
    {
    }

    protected override Task<Grade> Make()
    {
        var grad =  new Grade
        {
            
        };

        return Task.FromResult(grad);
    }
}