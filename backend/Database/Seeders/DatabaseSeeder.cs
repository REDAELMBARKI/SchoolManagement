

using System.Threading.Tasks;
using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend;
using SchoolManagement.Backend.Database.Seeders;
using SchoolManagement.Backend.Models;
using AutoMapper;

public class DatabaseSeeder
{
  public static async Task Seed(AppDbContext _context , IMapper mapper)
  {
        await new GenderSeeder(_context).RunAsync();          
      await new PlatformSeeder(_context).RunAsync();        
      await new BranchSeeder(_context).RunAsync();           
      await new SchoolProgramSeeder(_context).RunAsync();    
      await new UserSeeder(_context, mapper).RunAsync();     
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