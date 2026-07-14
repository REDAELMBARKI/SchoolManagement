

using System.Threading.Tasks;
using SchoolManagement.Infrastructure.Data.Factories;
 using SchoolManagement.Domain.Entities; 
 using SchoolManagement.Infrastructure.Data; 
 using SchoolManagement.Infrastructure.Repositories;
  namespace SchoolManagement.Infrastructure.Data.Seeders;

public class DatabaseSeeder
{

    private readonly AppDbContext _context;
    private readonly BranchFactory _branchFactory;
    private readonly SubjectRepository _subjectRepo;
    private readonly IntakeRepository _intakeRepo;

    public DatabaseSeeder(
        AppDbContext context,
        BranchFactory branchFactory,
        SubjectRepository subjectRepo,
        IntakeRepository intakeRepo

        )
    {
        _context = context;
        _branchFactory = branchFactory;
        _subjectRepo = subjectRepo;
        _intakeRepo = intakeRepo;
    }

    public async Task Seed()
    {
        await new BranchSeeder(_context , _branchFactory).RunAsync();           
        await new GenderSeeder(_context).RunAsync();          
        await new PlatformSeeder(_context).RunAsync();        
        await new SubjectSeeder(_context , _subjectRepo).RunAsync();    
        await new UserSeeder(_context).RunAsync();     
        await new AdSeeder(_context).RunAsync();               
        // LeadSource seeder must run after Branches, Ads, and Opcs are seeded       
        await new LeadSourceSeeder(_context).RunAsync();       
        await new IntakeSeeder(_context , _intakeRepo).RunAsync();
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