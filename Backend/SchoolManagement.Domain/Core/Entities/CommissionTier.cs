using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Domain.Core.Entities;

/// <summary>
/// Represents a tiered commission structure for Commercial Agents.
/// Each tier defines a sales count range and the commission amount earned when that range is achieved.
/// </summary>
public class CommissionTier : AggregateRoot
{
    /// <summary>Minimum number of sales (inclusive) required to reach this tier.</summary>
    public int MinSalesCount { get; private set; }

    /// <summary>Maximum number of sales (inclusive) for this tier. Null means no upper limit.</summary>
    public int? MaxSalesCount { get; private set; }

    /// <summary>Commission amount paid when this tier is achieved.</summary>
    public decimal Amount { get; private set; }

    /// <summary>Whether this tier is currently active and should be used in calculations.</summary>
    public bool IsActive { get; private set; }

    /// <summary>Display order for UI sorting (lower numbers appear first).</summary>
    public int DisplayOrder { get; private set; }

    protected CommissionTier() { } // EF Core

    public static CommissionTier Create(
        int minSalesCount,
        int? maxSalesCount,
        decimal amount,
        int displayOrder)
    {
        if (minSalesCount < 0)
            throw new ArgumentException("MinSalesCount cannot be negative.", nameof(minSalesCount));

        if (maxSalesCount.HasValue && maxSalesCount.Value < minSalesCount)
            throw new ArgumentException("MaxSalesCount cannot be less than MinSalesCount.", nameof(maxSalesCount));

        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));

        return new CommissionTier
        {
            Id = Guid.NewGuid(),
            MinSalesCount = minSalesCount,
            MaxSalesCount = maxSalesCount,
            Amount = amount,
            IsActive = true,
            DisplayOrder = displayOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(int minSalesCount, int? maxSalesCount, decimal amount, int displayOrder)
    {
        if (minSalesCount < 0)
            throw new ArgumentException("MinSalesCount cannot be negative.", nameof(minSalesCount));

        if (maxSalesCount.HasValue && maxSalesCount.Value < minSalesCount)
            throw new ArgumentException("MaxSalesCount cannot be less than MinSalesCount.", nameof(maxSalesCount));

        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));

        MinSalesCount = minSalesCount;
        MaxSalesCount = maxSalesCount;
        Amount = amount;
        DisplayOrder = displayOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
