using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Common.Interfaces.Services;

namespace SchoolManagement.Infrastructure.Data.Configurations.Extensions;

public static class HangfireJobsExtensions
{
    /// <summary>
    /// Registers all recurring Hangfire background jobs for the application.
    /// </summary>
    public static IApplicationBuilder RegisterHangfireJobs(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();


            recurringJobManager.AddOrUpdate<IInvoiceService>(
                "daily-overdue-invoice-check",
                service => service.ProcessPastDueInvoicesAsync(),
                Cron.Daily
            );

            recurringJobManager.AddOrUpdate<IInvoiceService>(
                "daily-invoice-generation",
                service => service.GenerateDailyInvoicesAsync(),
                Cron.Daily
            );

            recurringJobManager.AddOrUpdate(
                "system-health-check",
                () => Console.WriteLine($"[Hangfire] Health check executed at: {DateTime.UtcNow}"),
                Cron.Hourly
            );
        }

        return app;
    }
}
