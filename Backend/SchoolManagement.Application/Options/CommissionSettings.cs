namespace SchoolManagement.Application.Options;

public class CommissionSettings
{
    public const string SectionName = "Commission";

    /// <summary>Fixed amount paid to an OPC for each enrollment originating from their intake.</summary>
    public decimal OpcFlatAmount { get; set; } = 50;

    /// <summary>Day of month when salaries go out and all commissions are locked (e.g. 13).</summary>
    public int SalaryDayOfMonth { get; set; } = 13;

    /// <summary>Hour of day (UTC, 24h) at which the salary lockout runs (e.g. 20 = 8pm).</summary>
    public int SalaryLockoutHour { get; set; } = 20;

    /// <summary>
    /// Returns true if the salary cutoff for the given period month has already passed,
    /// meaning commissions are locked and cannot be changed.
    /// </summary>
    public bool IsLocked(DateOnly periodMonth, DateTime utcNow)
    {
        // Lockout datetime = salary day of the same month as the commission period
        var lockoutDate = new DateTime(
            periodMonth.Year,
            periodMonth.Month,
            SalaryDayOfMonth,
            SalaryLockoutHour,
            0, 0,
            DateTimeKind.Utc);

        return utcNow >= lockoutDate;
    }

    /// <summary>
    /// Tiered monthly commission amounts for Commercial Agents.
    /// Tiers should be ordered by MinSalesCount ascending.
    /// </summary>
    public List<CommissionTierOption> AgentTiers { get; set; } = new();

    /// <summary>Finds the matching tier for a given sales count. Returns null if no tier matches.</summary>
    public CommissionTierOption? ResolveTier(int salesCount)
    {
        return AgentTiers
            .Where(t => salesCount >= t.MinSalesCount &&
                        (t.MaxSalesCount == null || salesCount <= t.MaxSalesCount))
            .FirstOrDefault();
    }
}

public class CommissionTierOption
{
    public int MinSalesCount { get; set; }
    public int? MaxSalesCount { get; set; }
    public decimal Amount { get; set; }
}
