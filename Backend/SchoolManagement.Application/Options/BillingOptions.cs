namespace SchoolManagement.Application.Options;

public class BillingOptions
{
    public const string SectionName = "Billing";

    /// <summary>When payment exceeds invoice balance, excess is added to enrollment credit.</summary>
    public bool AllowOverpaymentToCredit { get; set; } = true;

    /// <summary>Auto-apply enrollment credit when generating renewal invoices (not on manual payments).</summary>
    public bool ApplyCreditOnRenewalOnly { get; set; } = true;

    /// <summary>Percentage (0-100) of applied credit restored to enrollment when an invoice is cancelled.</summary>
    public decimal CreditRestorePercentage { get; set; } = 100;

    /// <summary>When true, credit restore after period start requires GracePeriodDaysAfterPeriodStart.</summary>
    public bool RestoreCreditBeforePeriodStartOnly { get; set; } = true;

    /// <summary>Days after PeriodStart during which credit restore is still allowed (0 = none after start).</summary>
    public int GracePeriodDaysAfterPeriodStart { get; set; } = 0;

    /// <summary>Recommended max active charges per billing invoice (enforced in application layer).</summary>
    public int MaxActiveChargesPerInvoice { get; set; } = 1;

    public bool IsCreditRestoreAllowed(DateTime periodStart, DateTime utcNow)
    {
        if (!RestoreCreditBeforePeriodStartOnly)
            return true;

        if (utcNow < periodStart)
            return true;

        return GracePeriodDaysAfterPeriodStart > 0
            && utcNow <= periodStart.AddDays(GracePeriodDaysAfterPeriodStart);
    }

    public decimal CalculateCreditRestoreAmount(decimal creditAppliedAmount, DateTime periodStart, DateTime utcNow)
    {
        if (creditAppliedAmount <= 0 || CreditRestorePercentage <= 0)
            return 0;

        if (!IsCreditRestoreAllowed(periodStart, utcNow))
            return 0;

        return Math.Round(
            creditAppliedAmount * (CreditRestorePercentage / 100m),
            2,
            MidpointRounding.AwayFromZero);
    }
}
