using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolManagement.Backend;
using SchoolManagement.Backend.Database.Seeders;
using SchoolManagement.Backend.Database.Factories;
using Serilog;
using AutoMapper;

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
    options => options.UseSqlite(connectionString)
) ; 


// di
// add jwt barear 
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey         = new SymmetricSecurityKey(
//                 Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
//             ValidateIssuer   = false,  // skip for simple project
//             ValidateAudience = false,  // skip for simple project
//             ValidateLifetime = true,   // ✅ checks token expiry
//         };
// });
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// registre all classes 
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(c => 
          c.InNamespaces("SchoolManagement.Backend.Repositories",
                         "SchoolManagement.Backend.Services" ,
                         "SchoolManagement.Backend.Mappers" ,
                         "SchoolManagement.Backend.Database.Factories"
                         ))
    .AsSelf()                  
    .AsMatchingInterface()     
    .WithScopedLifetime());

builder.Services.AddControllers(); 

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>() ; 
        context.Database.CanConnect() ;
        context.Database.CloseConnection() ;
        Console.WriteLine("server runs successfullu") ; 
    }
    catch (Exception error)
    {
         Console.WriteLine($"❌ Database connection failed: {error.Message}");
        
    }
}


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    await DatabaseSeeder.Seed(context, mapper);
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
// app.UseAuthentication();
app.MapControllers();
app.Run();

