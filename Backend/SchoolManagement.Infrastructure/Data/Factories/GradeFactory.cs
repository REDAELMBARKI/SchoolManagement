using SchoolManagement.Backend.Entities;
using SchoolManagement.Backend.Data ;

namespace SchoolManagement.Backend.Data.Factories;

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