using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Api.Services;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Application.Academic.Services;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Common.Services;
using SchoolManagement.Application.Common.Validators;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Core.Services;
using SchoolManagement.CrossCutting.Identity.Services;
using SchoolManagement.Application.Core.Validators;
using SchoolManagement.Application.Options;
using SchoolManagement.CrossCutting.Identity.Authorizations.Extensions;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Infrastructure.Academic.Queries;
using SchoolManagement.Infrastructure.Academic.Repositories;
using SchoolManagement.Infrastructure.Common.Queries;
using SchoolManagement.Infrastructure.Common.Repositories;
using SchoolManagement.Infrastructure.Common.Services;
using SchoolManagement.Infrastructure.Core.Queries;
using SchoolManagement.Infrastructure.Core.Repositories;
using SchoolManagement.Infrastructure.Core.Repositories;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Infrastructure.Data.Configurations.Extensions;
using SchoolManagement.Infrastructure.Data.Seeders;
using Serilog;
using System.Text.Json.Serialization;
using SchoolManagement.Domain.Common.Entities;
using Microsoft.AspNetCore.Identity;
using SchoolManagement.CrossCutting.Identity.Entities;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.Console(outputTemplate: "[{Level}] {Message}{NewLine}{Exception}{NewLine}")
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);


// auto mapper 
builder.Services.AddAutoMapper(cfg => { },
typeof(Program).Assembly);
// configure context 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(connectionString)
                .UseLazyLoadingProxies()
);

// Add ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    
    // Email confirmation
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddValidatorsFromAssemblyContaining<EnrollmentValidator>();

// Add FluentValidation auto-validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();


// add policies 

builder.Services.AddAppAuthorizations();
builder.Services.AddBranchingAuthorizations();
builder.Services.AddAccountsAuthorizations();

// add hangfire 
builder.Services.AddHangfire(config =>
  config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddHangfireServer();

// Add controllers
builder.Services.AddControllers()
     .AddJsonOptions(options =>
     {
         // configure enum to be serialized as string in json
         options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
     });

builder.Services.AddHttpContextAccessor();
// add jwt barear 
builder.Services.AddJwtConfigExtension(builder.Configuration);

builder.Services.Configure<BillingOptions>(
    builder.Configuration.GetSection(BillingOptions.SectionName));

builder.Services.Configure<CommissionSettings>(
    builder.Configuration.GetSection(CommissionSettings.SectionName));

builder.Services.Configure<SchoolManagement.Application.Common.Settings.MediaStorageSettings>(
    builder.Configuration.GetSection("MediaStorage"));

// SMTP Settings
builder.Services.Configure<SchoolManagement.Api.Settings.SmtpSettings>(
    builder.Configuration.GetSection(SchoolManagement.Api.Settings.SmtpSettings.SectionName));

// Di registration 
builder.Services.Scan(scan => scan
    .FromAssemblies(typeof(Program).Assembly, typeof(StudentService).Assembly, typeof(CurrentUserContext).Assembly)
    .AddClasses(c =>
          c.InNamespaces("SchoolManagement.Infrastructure.Repositories",
                         "SchoolManagement.Infrastructure.Academic.Repositories",
                         "SchoolManagement.Infrastructure.Core.Repositories",
                         "SchoolManagement.Infrastructure.Common.Repositories",
                         "SchoolManagement.Application.Services",
                         "SchoolManagement.Application.Academic.Services",
                         "SchoolManagement.Application.Core.Services",
                         "SchoolManagement.Application.Core.Strategies",
                         "SchoolManagement.Application.Common.Services",
                         "SchoolManagement.Application.Mappers",
                         "SchoolManagement.Infrastructure.Services",
                         "SchoolManagement.Infrastructure.Data.Factories",
                         "SchoolManagement.Infrastructure.Data.Seeders",
                         "SchoolManagement.Domain.Interfaces",
                         "SchoolManagement.Application.Dtos",
                         "SchoolManagement.Infrastructure.Queries",
                         "SchoolManagement.Infrastructure.Academic.Queries",
                         "SchoolManagement.Infrastructure.Core.Queries",
                         "SchoolManagement.Infrastructure.Common.Queries"
                         ))
    .AsSelf()
    .AsMatchingInterface()
    .WithScopedLifetime());

builder.Services.AddScoped<ITransaction, EfTransaction>();
builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<ICommissionRepository, CommissionRepository>();

// JWT & Refresh Token Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ICommissionTierRepository, CommissionTierRepository>();
builder.Services.AddScoped<ICommissionQueryService, CommissionQueryService>();
builder.Services.AddScoped<ICommissionService, CommissionService>();
builder.Services.AddScoped<ICommissionTierService, CommissionTierService>();
builder.Services.AddScoped<IRefundRepository, RefundRepository>();
builder.Services.AddScoped<IRefundService, RefundService>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IExpenseQueryService, ExpenseQueryService>();
builder.Services.AddScoped<MediaStorageValidator>();

// WhatsApp Message Service
builder.Services.AddScoped<IWhatsAppMessageRepository, WhatsAppMessageRepository>();
builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
builder.Services.AddScoped<IWhatsAppMessageQueryService, WhatsAppMessageQueryService>();

// Gender, Opc, Ad, LeadSource - Fixed anti-pattern controllers
builder.Services.AddScoped<IGenderRepository, GenderRepository>();
builder.Services.AddScoped<IGenderService, GenderService>();
builder.Services.AddScoped<IGenderQueryService, GenderQueryService>();
builder.Services.AddScoped<IOpcRepository, OpcRepository>();
builder.Services.AddScoped<IOpcService, OpcService>();
builder.Services.AddScoped<IOpcQueryService, OpcQueryService>();
builder.Services.AddScoped<IAdRepository, AdRepository>();
builder.Services.AddScoped<IAdService, AdService>();
builder.Services.AddScoped<IAdQueryService, AdQueryService>();
builder.Services.AddScoped<ILeadSourceRepository, LeadSourceRepository>();
builder.Services.AddScoped<ILeadSourceService, LeadSourceService>();
builder.Services.AddScoped<ILeadSourceQueryService, LeadSourceQueryService>();

// Academic Management (Subject, Level, Room, Teacher, Absence, Grade)
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ILevelRepository, LevelRepository>();
builder.Services.AddScoped<ILevelQueryService, LevelQueryService>();
builder.Services.AddScoped<ILevelService, LevelService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<ITeacherQueryService, TeacherQueryService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IAbsenceRepository, AbsenceRepository>();
builder.Services.AddScoped<IAbsenceQueryService, AbsenceQueryService>();
builder.Services.AddScoped<IAbsenceService, AbsenceService>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<IGradeQueryService, GradeQueryService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IStudentRegistrationService, StudentRegistrationService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentQueryService, PaymentQueryService>();

// Financial Management (Plan, CommercialAgent, PayrollPayment)
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IPayrollPaymentQueryService, PayrollPaymentQueryService>();
builder.Services.AddScoped<IPayrollPaymentService, PayrollPaymentService>();
builder.Services.AddScoped<ICommercialAgentService, CommercialAgentService>();
builder.Services.AddScoped<IChargeRepository, ChargeRepository>();

// Common Management (Branch, Platform)
builder.Services.AddScoped<IBranchQueryService, BranchQueryService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddScoped<IPlatformService, PlatformService>();

// User Management - Domain/Business layer
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IDomainUserService, DomainUserService>();
builder.Services.AddScoped<IDomainUserRepository, DomainUserRepository>();

// User Management - Identity/Auth layer
builder.Services.AddScoped<SchoolManagement.CrossCutting.Identity.Interfaces.IAuthService, SchoolManagement.CrossCutting.Identity.Services.AuthService>();

// end Di registration

// add media tr event publisher 
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(InvoiceService).Assembly);
});

// Email Templates & Service (Blazor Razor Components)
builder.Services.AddRazorComponents();
builder.Services.AddScoped<Microsoft.AspNetCore.Components.Web.HtmlRenderer>();
builder.Services.AddScoped<EmailTemplateService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation();


var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        //context.Database.Migrate();
        Console.WriteLine("server runs succesfully");
    }
    catch (Exception error)
    {
        Console.WriteLine($"Database connection failed: {error.Message}");

    }
}


using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.Seed();
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseHangfireDashboard("/hangfire");
app.RegisterHangfireJobs();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

