

using System.Threading.Tasks;
using SchoolManagement.Database.Seeders;

public class DatabaseSeeder
{
  public static async Task Seed(AppDbContext _context)
  {
        await new LanguageSeeder(_context).RunAsync();
        await new LevelSeeder(_context).RunAsync();    
        await new SessionSeeder(_context).RunAsync();   
        await new GroupSeeder(_context).RunAsync();    
        await new StudentSeeder(_context).RunAsync();   
        // await new ModuleSeeder(_context).RunAsync();   
        // await new RoomSeeder(_context).RunAsync();     
        // await new TeacherSeeder(_context).RunAsync();  
  }

  
}