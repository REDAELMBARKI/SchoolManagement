using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Infrastructure.Data.Seeders;
using SchoolManagement.Infrastructure.Data.Factories;
using Serilog;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using SchoolManagement.Infrastructure.Data.Configurations.Extensions;
using SchoolManagement.Application.Core.Services;
using SchoolManagement.Application.Options;
using SchoolManagement.Application.Core.Services;
using SchoolManagement.Application.Core.Validators;
using SchoolManagement.Infrastructure.Academic.Repositories;
using SchoolManagement.Infrastructure.Core.Repositories;
using SchoolManagement.Infrastructure.Common.Repositories;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Infrastructure.Common.Services;
using SchoolManagement.Domain.Core.Interfaces;
using Hangfire;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.Console(outputTemplate: "[{Level}] {Message}{NewLine}{Exception}{NewLine}")
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);


// auto mapper 
builder.Services.AddAutoMapper(cfg => { }  , 
typeof(Program).Assembly) ;
// configure context 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ; 
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(connectionString)
                .UseLazyLoadingProxies()
) ; 

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddValidatorsFromAssemblyContaining<EnrollmentValidator>();

// Add FluentValidation auto-validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();


// add hangfire 
builder.Services.AddHangfire(config => 
  config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddHangfireServer();

// Add controllers
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// add jwt barear 
builder.Services.AddJwtConfigExtension(builder.Configuration);

builder.Services.Configure<BillingOptions>(
    builder.Configuration.GetSection(BillingOptions.SectionName));

builder.Services.Configure<CommissionSettings>(
    builder.Configuration.GetSection(CommissionSettings.SectionName));

// Di registration 
builder.Services.Scan(scan => scan
    .FromAssemblies(typeof(Program).Assembly, typeof(StudentService).Assembly, typeof(CurrentUserContext).Assembly)
    .AddClasses(c => 
          c.InNamespaces("SchoolManagement.Infrastructure.Repositories",
                         "SchoolManagement.Application.Services" ,
                         "SchoolManagement.Application.Mappers" ,
                         "SchoolManagement.Infrastructure.Services",
                         "SchoolManagement.Infrastructure.Data.Factories" ,
                         "SchoolManagement.Infrastructure.Data.Seeders",
                         "SchoolManagement.Domain.Interfaces" ,
                         "SchoolManagement.Application.Dtos",
                         "SchoolManagement.Infrastructure.Queries"
                         ))
    .AsSelf()                  
    .AsMatchingInterface()     
    .WithScopedLifetime());

builder.Services.AddScoped<ITransaction, EfTransaction>();
builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<ICommissionRepository, CommissionRepository>();
builder.Services.AddScoped<ICommissionService, CommissionService>();
builder.Services.AddScoped<IRefundRepository, RefundRepository>();
builder.Services.AddScoped<IRefundService, RefundService>();

// end Di registration

// add media tr event publisher 
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(InvoiceService).Assembly);
});

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
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>() ; 
        context.Database.Migrate() ;
        Console.WriteLine("server runs succesfully") ; 
    }
    catch (Exception error)
    {
         Console.WriteLine($"Database connection failed: {error.Message}");
        
    }
}


using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.Seed() ;
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseHangfireDashboard("/hangfire");
app.RegisterHangfireJobs();

// app.UseAuthentication();
app.MapControllers();
app.Run();

