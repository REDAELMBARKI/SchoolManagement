using SchoolManagement.Interfaces;
using SchoolManagement.Models;

namespace SchoolManagement.Repositories;

public class StudentRepository : IRepository<Student>
{
    AppDbContext _context ;
    public StudentRepository(AppDbContext context)
    {
        _context  = context ;
    }

    public List<Student> GetList()
    { 
        List<Student> students = [];
        return  students ;
    }

    // public async Student Create(int id)
    // {
    //     // await _context.Students.FindAsync((Student student) => student.Id == id) ;
    // }

    public async Task Destroy()
    {
        
    }
}