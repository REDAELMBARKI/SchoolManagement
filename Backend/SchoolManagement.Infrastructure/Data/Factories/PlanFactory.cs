using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

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

        var selected = faker.PickRandom(plans);
        return Task.FromResult(Plan.Create(selected.Name, selected.Duration, selected.Discount));
    }
}
