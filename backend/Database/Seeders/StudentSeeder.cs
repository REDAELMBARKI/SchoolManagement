
using SchoolManagement.Database.Factories;
using SchoolManagement.Models;

namespace SchoolManagement.Database.Seeders ; 
public class StudentSeeder  : Seeder
{
    
    
    private readonly StudentFactory _factory;
    private readonly AppDbContext _context;

    public StudentSeeder(AppDbContext context)
    {
            _factory = new StudentFactory();
            _context = context ;

    }
    public override async Task RunAsync()
    {
        List<Student> students = _factory.MakeMany(10) ; 
        await _context.Students.AddRangeAsync(students) ;
        await _context.SaveChangesAsync() ;
    }


}