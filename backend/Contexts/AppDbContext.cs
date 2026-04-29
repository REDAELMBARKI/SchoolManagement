using SchoolManagement.Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SchoolManagement.Backend.Contexts;
using SchoolManagement.Backend.TablesConfigs;

namespace SchoolManagement.Backend;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ── People ──
    public DbSet<User> Users { get; set; }
    public DbSet<StaffUser> StaffUsers { get; set; }
    public DbSet<Intake> Intakes { get; set; }  
    public DbSet<Student> Students  { get; set; }  
    public DbSet<Opc> Opcs  { get; set; }  
    public DbSet<Teacher> Teachers  { get; set; }  
    public DbSet<Parent> Parents { get; set; }
    public DbSet<CommercialAgent> CommercialAgents { get; set; }
    public DbSet<StudentParent> StudentParents { get; set; }  
   
    public DbSet<Branch> Branches { get; set; }
   
    // ── Lookup ──
    public DbSet<Level> Levels { get; set; }
    public DbSet<SchoolProgram> SchoolPrograms { get; set; }     

    // ── period ──
    public DbSet<Session> Sessions { get; set; }    
    // ── Physical ──
    public DbSet<Room> Rooms { get; set; }         

    // ── Academic ──
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupTeacher> GroupTeachers { get; set; } 
    public DbSet<Schedule> Schedules { get; set; } 

    // ── Operations ──
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Absence> Absences { get; set; }
    public DbSet<Grade> Grades { get; set; }       
    public DbSet<Payment> Payments { get; set; }
    

    public DbSet<Gender> Genders {get;set;} 
    public DbSet<LeadSource> LeadSources {get;set;}
    
    public DbSet<Platform> Platforms {get;set;}
    public DbSet<Ad> Ads {get;set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // to tpt  
        modelBuilder.ConfigureUsersTptMethod();
        
    

        modelBuilder.Entity<Parent>()
        .HasMany(p => p.Students)
        .WithMany(s => s.Parents)
        .UsingEntity<StudentParent>(
            j => j.HasOne(sp => sp.Student)
                  .WithMany()
                  .HasForeignKey(sp => sp.StudentId)
            ,
            j => j.HasOne(sp => sp.Parent)
                 .WithMany()
                 .HasForeignKey(sp => sp.ParentId)
        ) 
        ;

 

        modelBuilder.Entity<Ad>()
        .HasOne(ad => ad.Platform)
        .WithMany()
        .HasForeignKey(ad => ad.PlatformId) ;
        /* 
           Group - Student relationships 
           group -> students 
           student -> group 
        */
        modelBuilder.Entity<Group>()
        .HasMany(g =>  g.Students)
        .WithOne(s => s.Group)
        .HasForeignKey(s => s.GroupId) ;

      

    }

}