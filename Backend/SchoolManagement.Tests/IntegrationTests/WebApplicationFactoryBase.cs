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
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Tests.IntegrationTests;

public class WebApplicationFactoryBase<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private string _testConnectionString = "";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var originalDbString = config.Build().GetConnectionString("DefaultConnection");
            var builder2 = new SqlConnectionStringBuilder(originalDbString);
            builder2.InitialCatalog = "SchoolManagementTestDb";
            _testConnectionString = builder2.ConnectionString;
        });

        // Enable detailed logging for API exceptions
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.AddDebug();
            logging.SetMinimumLevel(LogLevel.Information);
        });

        builder.ConfigureTestServices(services =>
        {
            // Remove existing DbContext registrations
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            // Register test database with lazy loading + detailed errors
            services.AddDbContext<AppDbContext>(options => 
                options.UseSqlServer(_testConnectionString)
                       .UseLazyLoadingProxies()
                       .EnableSensitiveDataLogging() // Shows SQL parameter values
                       .EnableDetailedErrors());      // Shows detailed EF errors

            // Register test authentication
            services.AddAuthentication(TestAuthenticationHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.SchemeName, 
                    options => { });
        });
    }
}
