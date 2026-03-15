using System;
using System.Collections.Immutable;
using System.Data.Common;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
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
    options => options.UseMySql(
        connectionString , 
        ServerVersion.AutoDetect(connectionString)
    )
   
) ; 
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
