using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.CrossCutting.Identity.Entities;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Infrastructure.Common.Configurations;
using SchoolManagement.Infrastructure.Data.Configurations.Entities;

namespace SchoolManagement.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    private readonly ICurrentUserContext? _currentUserContext;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserContext? currentUserContext = null) 
        : base(options)
    {
        _currentUserContext = currentUserContext;
    }


    // ── Lookup ──
    public DbSet<Gender> Genders { get; set; }
    public DbSet<LeadSource> LeadSources { get; set; }
    public DbSet<Level> Levels { get; set; }
    public DbSet<Subject> Subjects { get; set; }

    // ── Authentication ──
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    // ── Platforms and Ads ──
    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Ad> Ads { get; set; }

    // ── Branches ──
    public DbSet<Branch> Branches { get; set; }

    // ── People ──

    public DbSet<DomainUser> DomainUsers { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<StudentResponsable> StudentResponsables { get; set; }
    public DbSet<CommercialAgent> CommercialAgents { get; set; }
    public DbSet<Opc> Opcs { get; set; }

    // ── Intakes and Students ──
    public DbSet<Intake> Intakes { get; set; }
    public DbSet<Student> Students { get; set; }

    // ── Physical ──
    public DbSet<Room> Rooms { get; set; }

    // ── Academic ──
    public DbSet<Group> Groups { get; set; }


    // ── Schedules ──
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Day> Days { get; set; }
    public DbSet<TimeSlot> TimeSlots { get; set; }
    public DbSet<GroupTeacher> GroupTeachers { get; set; }

    // ── Operations ──
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<EnrollmentPlan> EnrollmentPlans { get; set; }
    public DbSet<Absence> Absences { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Charge> Charges { get; set; }
    // <summary> 
    // expenses any bills but not salaries
    // </summary>
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<WhatsAppMessage> WhatsAppMessages { get; set; }

    // <summary> 
    // only emplyee salaries payrolls
    // </summary>
    public DbSet<PayrollPayment> PayrollPayments { get; set; }
    public DbSet<Commission> Commissions { get; set; }
    public DbSet<CommissionTier> CommissionTiers { get; set; }
    public DbSet<Refund> Refunds { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    public DbSet<Media> Medias { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //this.IgnoreEntities(modelBuilder);
        this.ApplyEntityConfigurations(modelBuilder);
        this.ApplySoftDeleteFilter(modelBuilder);
        this.ApplyBranchIsolationFilter(modelBuilder);
    }

    /// <summary>
    /// Applies global query filter for branch-level isolation.
    /// ALL users (including SuperAdmin) are filtered by their BranchId by default.
    /// SuperAdmin can bypass this filter using .IgnoreQueryFilters() when needed.
    /// 
    /// Usage:
    /// - Normal query: var students = await _context.Students.ToListAsync(); // Filtered by user's branch
    /// - SuperAdmin bypass: var students = await _context.Students.IgnoreQueryFilters().ToListAsync(); // See all branches
    /// </summary>
    private void ApplyBranchIsolationFilter(ModelBuilder modelBuilder)
    {
        // Skip if no current user context (e.g., during migrations)
        if (_currentUserContext == null)
            return;

        var userBranchId = _currentUserContext.BranchId;

        // Apply filter to Person base type - automatically covers ALL derived types
        // (DomainUser, Employee, Teacher, Opc, CommercialAgent, Intake, Student, StudentResponsable)
        // SuperAdmin has SYSTEM_BRANCH_ID, so they see everything with .IgnoreQueryFilters()
        modelBuilder.Entity<Person>().HasQueryFilter(e => 
            e.BranchId == userBranchId || e.BranchId == Branch.SYSTEM_BRANCH_ID);

        // Intakes and Students
        modelBuilder.Entity<Intake>().HasQueryFilter(e => 
            e.BranchId == userBranchId);
        
        modelBuilder.Entity<Student>().HasQueryFilter(e => 
            e.BranchId == userBranchId);
        
        modelBuilder.Entity<StudentResponsable>().HasQueryFilter(e => 
            e.BranchId == userBranchId);

        // Physical
        modelBuilder.Entity<Room>().HasQueryFilter(e => 
            e.BranchId == userBranchId);

        // Academic
        modelBuilder.Entity<Group>().HasQueryFilter(e => 
            e.BranchId == userBranchId);
        
        modelBuilder.Entity<Schedule>().HasQueryFilter(e => 
            e.Group.BranchId == userBranchId);

        // Operations
        modelBuilder.Entity<Enrollment>().HasQueryFilter(e => 
            e.Student.BranchId == userBranchId);
        
        modelBuilder.Entity<Absence>().HasQueryFilter(e => 
            e.Student.BranchId == userBranchId);
        
        modelBuilder.Entity<Grade>().HasQueryFilter(e => 
            e.Student.BranchId == userBranchId);

        // Financial
        modelBuilder.Entity<Payment>().HasQueryFilter(e => 
            e.BranchId == userBranchId);
        
        modelBuilder.Entity<Invoice>().HasQueryFilter(e => 
            e.BranchId == userBranchId);
        
        modelBuilder.Entity<Charge>().HasQueryFilter(e => 
            e.Invoice.BranchId == userBranchId);
        
        modelBuilder.Entity<Expense>().HasQueryFilter(e => 
            e.BranchId == userBranchId);
        
        modelBuilder.Entity<PayrollPayment>().HasQueryFilter(e => 
            e.BranchId == userBranchId);
        
        modelBuilder.Entity<Commission>().HasQueryFilter(e => 
            e.BranchId == userBranchId);
        
        modelBuilder.Entity<Refund>().HasQueryFilter(e => 
            e.BranchId == userBranchId);

        // WhatsApp Messages
        modelBuilder.Entity<WhatsAppMessage>().HasQueryFilter(e => 
            e.BranchId == userBranchId);

        // Audit Logs - Include user's branch logs AND global system logs
        modelBuilder.Entity<AuditLog>().HasQueryFilter(e => 
            e.BranchId == userBranchId || e.BranchId == Branch.SYSTEM_BRANCH_ID);
    }



    private void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>().HasQueryFilter(e => e.DeletedAt == null);
       
    }


    private void IgnoreEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<BaseEntity>();
    }


    private void ApplyEntityConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PersonConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeConfigurations());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new DomainUserConfiguration());
        modelBuilder.ApplyConfiguration(new IntakeConfiguration());
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        modelBuilder.ApplyConfiguration(new StudentResponsableConfiguration());
        modelBuilder.ApplyConfiguration(new TeacherConfiguration());
        modelBuilder.ApplyConfiguration(new CommercialAgentConfiguration());
        modelBuilder.ApplyConfiguration(new OpcConfiguration());
        modelBuilder.ApplyConfiguration(new GroupConfiguration());
        modelBuilder.ApplyConfiguration(new ScheduleConfiguration());
        modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
        modelBuilder.ApplyConfiguration(new EnrollmentPlanConfiguration());
        modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
        modelBuilder.ApplyConfiguration(new ChargeConfiguration());
        modelBuilder.ApplyConfiguration(new GradeConfiguration());
        modelBuilder.ApplyConfiguration(new MediaConfiguration());
        modelBuilder.ApplyConfiguration(new LeadSourceConfiguration());
        modelBuilder.ApplyConfiguration(new OpcLeadSourceConfiguration());
        modelBuilder.ApplyConfiguration(new AdLeadSourceConfiguration());
        modelBuilder.ApplyConfiguration(new CommissionConfiguration());
        modelBuilder.ApplyConfiguration(new RefundConfiguration());
        modelBuilder.ApplyConfiguration(new PayrollPaymentConfiguration());
        modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
        modelBuilder.ApplyConfiguration(new WhatsAppMessageConfiguration());
    }


}