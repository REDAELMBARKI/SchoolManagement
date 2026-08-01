using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Options;

namespace SchoolManagement.Infrastructure.Data.Configurations.Extensions;

public static class HangfireJobsExtensions
{
    /// <summary>
    /// Registers all recurring Hangfire background jobs for the application.
    /// </summary>
    public static IApplicationBuilder RegisterHangfireJobs(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
        var commissionSettings = scope.ServiceProvider.GetRequiredService<IOptions<CommissionSettings>>().Value;

        recurringJobManager.AddOrUpdate<IInvoiceService>(
            "daily-overdue-invoice-check",
            service => service.ProcessPastDueInvoicesAsync(),
            Cron.Daily);

        recurringJobManager.AddOrUpdate<IInvoiceService>(
            "daily-invoice-generation",
            service => service.GenerateDailyInvoicesAsync(),
            Cron.Daily);

        // Runs on the 1st of every month at 02:00 UTC — calculates agent tier commissions
        // for the previous calendar month.
        recurringJobManager.AddOrUpdate<ICommissionService>(
            "monthly-agent-commission",
            service => service.ProcessAgentMonthlyCommissionsAsync(
                DateTime.UtcNow.AddMonths(-1).Year,
                DateTime.UtcNow.AddMonths(-1).Month),
            Cron.Monthly(1, 2));

        // Salary lockout — runs on the configured day/hour from appsettings.
        // Flips all Approved commissions for the current month to Paid.
        // Blocked ones stay Blocked. After this nothing changes.
        var salaryDay  = commissionSettings.SalaryDayOfMonth;
        var salaryHour = commissionSettings.SalaryLockoutHour;
        var salaryCron = $"0 {salaryHour} {salaryDay} * *";

        recurringJobManager.AddOrUpdate<ICommissionService>(
            "monthly-salary-lockout",
            service => service.ProcessSalaryLockoutAsync(
                DateTime.UtcNow.Year,
                DateTime.UtcNow.Month),
            salaryCron);

        recurringJobManager.AddOrUpdate(
            "system-health-check",
            () => Console.WriteLine($"[Hangfire] Health check executed at: {DateTime.UtcNow}"),
            Cron.Hourly);

        return app;
    }
}
