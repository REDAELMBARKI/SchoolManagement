using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class PlanFactory : Factory<Plan>
{
    public PlanFactory(AppDbContext context) : base(context)
    {
    }

    protected override Task<Plan> Make()
    {
        var plans = new (string Name, int Duration, decimal? Discount)[]
        {
            ("1 Month", 1, null),
            ("3 Months", 3, 10m),
            ("6 Months", 6, 15m),
            ("Full Year", 12, 20m)
        };

        var branches = Context.Branches.Select(b => b.Id).ToList();
        var branchId = branches.Any() ? faker.PickRandom(branches) : Guid.Empty;

        var selected = faker.PickRandom(plans);
        decimal baseAmount = selected.Duration * 500m;
        int remainingAmountDueDays = selected.Duration * 30;
        return Task.FromResult(Plan.Create(selected.Name, selected.Duration, remainingAmountDueDays, baseAmount, branchId, selected.Discount));
    }
}
