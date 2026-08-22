using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Infrastructure.Data.Seeders;

namespace SchoolManagement.Tests.IntegrationTests;

public class WebApplicationFactoryBase<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private string _testConnectionString = "";
    
    public WebApplicationFactoryBase()
    {
        // Force Testing environment before anything else
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var originalDbString = config.Build().GetConnectionString("DefaultConnection");
            var builder2 = new SqlConnectionStringBuilder(originalDbString);
            builder2.InitialCatalog = "SchoolManagementTestDb";
            _testConnectionString = builder2.ConnectionString;
            
            // Add test JWT configuration
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Jwt:Key"] = "TestSecretKeyForJwtTokenGeneration123456789012345678901234567890",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
                ["Jwt:AccessTokenExpiryMinutes"] = "60",
                ["Jwt:RefreshTokenExpiryDays"] = "7",
                ["Jwt:RememberMeRefreshTokenExpiryDays"] = "30"
            }!);
        });

        // Set Testing environment to skip Serilog
        builder.UseEnvironment("Testing");
        
        // Enable console logging for tests with explicit Debug level
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            
            // Add console with explicit configuration to show Debug logs
            logging.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
            
            logging.SetMinimumLevel(LogLevel.Trace);
            
            // Explicitly set Debug level for SchoolManagement namespaces
            logging.AddFilter("SchoolManagement", LogLevel.Information);
            logging.AddFilter("Microsoft", LogLevel.Warning);
            logging.AddFilter("System", LogLevel.Warning);
            logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information);
        });

        builder.ConfigureTestServices(services =>
        {
            // This runs AFTER Program.cs services are added

            // Remove existing DbContext registrations
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            // Register test database
            services.AddDbContext<AppDbContext>(options => 
                options.UseSqlServer(_testConnectionString)
                       .UseLazyLoadingProxies()
                       .EnableSensitiveDataLogging()
                       .EnableDetailedErrors());

            // Replace real EmailService with fake no-op version
            services.RemoveAll<SchoolManagement.Application.Common.Interfaces.Services.IEmailService>();
            services.AddSingleton<SchoolManagement.Application.Common.Interfaces.Services.IEmailService, FakeEmailService>();

            // CRITICAL: Replace JWT authentication with test authentication
            // Clear the authentication configuration
            services.Configure<Microsoft.AspNetCore.Authentication.AuthenticationOptions>(options =>
            {
                options.DefaultScheme = TestAuthenticationHandler.SchemeName;
                options.DefaultAuthenticateScheme = TestAuthenticationHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthenticationHandler.SchemeName;
            });

            // Add test authentication
            services.AddAuthentication(TestAuthenticationHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.SchemeName,
                    options => { });

            // Build the service provider to access DbContext and seed data
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var dbContext = scopedServices.GetRequiredService<AppDbContext>();

            // Ensure database is created and migrated
            dbContext.Database.Migrate();

            // Run seeders
            var seeder = scopedServices.GetRequiredService<DatabaseSeeder>();
            seeder.Seed().GetAwaiter().GetResult();
        });
    }
}
