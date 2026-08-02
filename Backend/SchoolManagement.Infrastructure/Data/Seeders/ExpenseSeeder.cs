using SchoolManagement.Infrastructure.Data.Factories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Seeders;

public class ExpenseSeeder : Seeder
{
    private readonly ExpenseFactory _expenseFactory;

    public ExpenseSeeder(AppDbContext context) : base(context)
    {
        _expenseFactory = new ExpenseFactory(context);
    }

    public override async Task RunAsync()
    {
        var items = await _expenseFactory.MakeMany(5);
        await Context.Expenses.AddRangeAsync(items);
        await Context.SaveChangesAsync();
    }
}
