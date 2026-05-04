using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SchoolManagement.Backend.Contexts;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.TablesConfigs;

namespace SchoolManagement.Backend;

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
    public DbSet<Employee> Employees { get; set; }
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

    // ── Period ──
    public DbSet<Session> Sessions { get; set; }

    // ── Schedules ──
    public DbSet<Schedule> Schedules { get; set; }

    // ── Operations ──
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Absence> Absences { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<Payment> Payments { get; set; }




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations
        modelBuilder.ApplyConfiguration(new PersonConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeConfigurations());
        modelBuilder.ApplyConfiguration(new IntakeConfiguration());
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        modelBuilder.ApplyConfiguration(new ParentConfiguration());
        modelBuilder.ApplyConfiguration(new TeacherConfiguration());
        modelBuilder.ApplyConfiguration(new CommercialAgentConfiguration());
        modelBuilder.ApplyConfiguration(new OpcConfiguration());

        // Apply relationship configurations from Contexts folder
        modelBuilder.ApplyConfiguration(new IntakeContext());
        modelBuilder.ApplyConfiguration(new StudentContext());

        // Apply soft delete filter globally
        ApplySoftDeleteFilter(modelBuilder);

        


        // LeadSource relationships
        modelBuilder.Entity<LeadSource>()
        .HasOne(ls => ls.Branch)
        .WithMany()
        .HasForeignKey(ls => ls.BranchId);

        modelBuilder.Entity<LeadSource>()
        .HasOne(ls => ls.Ad)
        .WithMany()
        .HasForeignKey(ls => ls.AdId);

        modelBuilder.Entity<LeadSource>()
        .HasOne(ls => ls.Opc)
        .WithMany()
        .HasForeignKey(ls => ls.OpcId);
    }

    private void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Employee>().HasQueryFilter(e => e.DeletedAt == null);
     
    }

}