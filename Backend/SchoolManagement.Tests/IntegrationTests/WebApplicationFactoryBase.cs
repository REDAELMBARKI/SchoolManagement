using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Tests.IntegrationTests;

public class WebApplicationFactoryBase<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private string _testConnectionString = "";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var originalDbString = config.Build().GetConnectionString("DefaultConnection");
            var builder2 = new SqlConnectionStringBuilder(originalDbString);
            builder2.InitialCatalog = "SchoolManagementTestDb";
            _testConnectionString = builder2.ConnectionString;
        });

        builder.ConfigureTestServices(services =>
        {
            // Remove existing DbContext registrations
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            // Register test database with lazy loading
            services.AddDbContext<AppDbContext>(options => 
                options.UseSqlServer(_testConnectionString)
                       .UseLazyLoadingProxies());

            // Register test authentication
            services.AddAuthentication(TestAuthenticationHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.SchemeName, 
                    options => { });
        });
    }

   
}
