using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.Application.Interfaces.Services;

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

            // 1. Daily Overdue Invoice Check (Runs daily at midnight)
            // Note: Once ProcessOverdueInvoicesAsync is implemented in IInvoiceService, this job will execute it.
            /*
            recurringJobManager.AddOrUpdate<IInvoiceService>(
                "daily-overdue-invoice-check",
                service => service.ProcessOverdueInvoicesAsync(),
                Cron.Daily
            );
            */

            // Example placeholder job to verify Hangfire registration on startup
            recurringJobManager.AddOrUpdate(
                "system-health-check",
                () => Console.WriteLine($"[Hangfire] Health check executed at: {DateTime.UtcNow}"),
                Cron.Hourly
            );
        }

        return app;
    }
}
