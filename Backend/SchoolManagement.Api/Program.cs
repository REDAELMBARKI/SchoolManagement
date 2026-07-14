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
using SchoolManagement.Infrastructure.Repositories;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.Console(outputTemplate: "[{Level}] {Message}{NewLine}{Exception}{NewLine}")
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

// auto mapper 
builder.Services.AddAutoMapper(typeof(Program).Assembly) ;
// configure context 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ; 
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(connectionString)
                .UseLazyLoadingProxies()
) ; 

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Add FluentValidation auto-validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Add controllers
builder.Services.AddControllers();


// add jwt barear 
builder.Services.AddJwtConfigExtension(builder.Configuration);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// registre all classes 
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(c => 
          c.InNamespaces("SchoolManagement.Infrastructure.Repositories",
                         "SchoolManagement.Application.Services" ,
                         "SchoolManagement.Application.Mappers" ,
                         "SchoolManagement.Infrastructure.Data.Factories" ,
                    "SchoolManagement.Infrastructure.Data.Seeders",
                         "SchoolManagement.Domain.Interfaces" ,
                         "SchoolManagement.Application.Dtos"
                         ))
    .AsSelf()                  
    .AsMatchingInterface()     
    .WithScopedLifetime());

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));


builder.Services.AddSwaggerGen();


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
         Console.WriteLine($"❌ Database connection failed: {error.Message}");
        
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
// app.UseAuthentication();
app.MapControllers();
app.Run();

