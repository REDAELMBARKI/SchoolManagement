

using System.Threading.Tasks;
using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend;
using SchoolManagement.Backend.Database.Seeders;
using SchoolManagement.Backend.Models;

public class DatabaseSeeder
{
  public static async Task Seed(AppDbContext _context , UserFactory userFactory)
  {

        await new OpcSeeder(userFactory , _context ).RunAsync();
        await new GenderSeeder(_context).RunAsync();
        await new LanguageSeeder(_context).RunAsync();
        await new AdSeeder(_context).RunAsync();
        await new LeadSourceSeeder(_context).RunAsync();
        await new IntakeSeeder(_context).RunAsync();
        // await new LevelSeeder(_context).RunAsync();    
        // await new SessionSeeder(_context).RunAsync();   
        // await new GroupSeeder(_context).RunAsync();    
        // await new RoomSeeder(_context).RunAsync();   
        // await new ScheduleSeeder(_context).RunAsync();   
        // await new EnrollmentSeeder(_context).RunAsync();   
        // await new ModuleSeeder(_context).RunAsync();   
        // await new RoomSeeder(_context).RunAsync();     
        // await new GradeSeeder(_context).RunAsync();     
        
  }

  
}