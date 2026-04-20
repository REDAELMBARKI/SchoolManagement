using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolManagement.Database.Seeders;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.Console(outputTemplate: "[{Level}] {Message}{NewLine}{Exception}{NewLine}")
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

// configure context 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ; 
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlite(connectionString)
) ; 

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
    .AddClasses(c => c.InNamespaces("SchoolManagement.Repositories", "SchoolManagement.Services"))
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

// fun seeders 
var isDbCommand = false ; 
using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {    
      if (args.Length > 0 && args[0] == "db")
        {   
            isDbCommand = true ;
            switch (args[1])
            {
                case "migrate":
                    await context.Database.MigrateAsync();
                    Console.WriteLine("migrations applied");
                    break;

                case "migrate:fresh":
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.MigrateAsync();
                    Console.WriteLine("database freshed");
                    break;

                case "migrate:fresh:seed":
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.MigrateAsync();
                    await DatabaseSeeder.Seed(context);
                    Console.WriteLine("freshed and seeded");
                    break;

                case "seed":
                    await DatabaseSeeder.Seed(context);
                    Console.WriteLine("seeded successfully");
                    break;

                default:
                    var seedArg = args.FirstOrDefault(a => a.StartsWith("--seed-class="));

                    if (seedArg == null)
                    {
                        Console.WriteLine("invalid command");
                        break;
                    }

                    var className = seedArg.Split('=')[1];

                    if (string.IsNullOrEmpty(className))
                    {
                        Console.WriteLine("no class name provided after =");
                        break;
                    }

                    var seederType = AppDomain.CurrentDomain
                        .GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .FirstOrDefault(c => c.Name == className);

                    if (seederType == null)
                    {
                        Console.WriteLine($"'{className}' not found");
                        break;
                    }

                    if (!typeof(Seeder).IsAssignableFrom(seederType))
                    {
                        Console.WriteLine($"'{className}' is not a valid seeder");
                        break;
                    }

                    var instance = Activator.CreateInstance(seederType, context);
                    await (Task) seederType.GetMethod("RunAsync")!.Invoke(instance, null)!;
                    break;
            }

        }
    }
    catch (Exception ex)
    {
        isDbCommand = true ;
        var message = ex.InnerException?.InnerException?.Message 
                ?? ex.InnerException?.Message 
                ?? ex.Message;
                
        Log.Error("❌ {Message}", message);

    }
}

           
if(isDbCommand)  return; // never run server


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();

