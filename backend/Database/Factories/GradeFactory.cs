using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Contexts ;

namespace SchoolManagement.Backend.Database.Factories;

public class GradeFactory : Factory<Grade>
{
    public GradeFactory(AppDbContext context) : base(context)
    {
    }

    protected override Grade Make()
    {
        return new Grade
        {
            
        };
    }
}