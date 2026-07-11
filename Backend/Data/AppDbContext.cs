using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SchoolManagement.Backend.Configurations;
using SchoolManagement.Backend.Entities;
using System.Linq;
namespace SchoolManagement.Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  
    // ── Lookup ──
    public DbSet<Gender> Genders { get; set; }
    public DbSet<LeadSource> LeadSources { get; set; }
    public DbSet<Level> Levels { get; set; }
    public DbSet<Subject> Subjects { get; set; }

    // ── Platforms and Ads ──
    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Ad> Ads { get; set; }

    // ── Branches ──
    public DbSet<Branch> Branches { get; set; }

    // ── People ──
    public DbSet<User> Users { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Parent> Parents { get; set; }
    public DbSet<CommercialAgent> CommercialAgents { get; set; }
    public DbSet<Opc> Opcs { get; set; }

    // ── Intakes and Students ──
    public DbSet<Intake> Intakes { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentParent> StudentParents { get; set; }

    // ── Physical ──
    public DbSet<Room> Rooms { get; set; }

    // ── Academic ──
     public DbSet<Group> Groups { get; set; }
    public DbSet<GroupTeacher> GroupTeachers { get; set; }


    // ── Schedules ──
    public DbSet<Schedule> Schedules { get; set; }

    // ── Operations ──
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Absence> Absences { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<Payment> Payments { get; set; }

    public DbSet<Media> Medias { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //this.IgnoreEntities(modelBuilder);
        this.ApplyEntityConfigurations(modelBuilder);
        this.ApplySoftDeleteFilter(modelBuilder);
    }

 

    private void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().HasQueryFilter(e => e.DeletedAt == null);
    }


    private  void IgnoreEntities(ModelBuilder  modelBuilder){
           modelBuilder.Ignore<BaseEntity>();
    }


    private void ApplyEntityConfigurations(ModelBuilder modelBuilder){
        modelBuilder.ApplyConfiguration(new PersonConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeConfigurations());
        modelBuilder.ApplyConfiguration(new IntakeConfiguration());
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        modelBuilder.ApplyConfiguration(new ParentConfiguration());
        modelBuilder.ApplyConfiguration(new TeacherConfiguration());
        modelBuilder.ApplyConfiguration(new CommercialAgentConfiguration());
        modelBuilder.ApplyConfiguration(new OpcConfiguration());
        modelBuilder.ApplyConfiguration(new GroupConfiguration());
        modelBuilder.ApplyConfiguration(new GroupTeacherConfiguration());
        modelBuilder.ApplyConfiguration(new ScheduleConfiguration());
        modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
        modelBuilder.ApplyConfiguration(new GradeConfiguration());
        modelBuilder.ApplyConfiguration(new MediaConfiguration());
        modelBuilder.ApplyConfiguration(new LeadSourceConfiguration());
    }





    
}