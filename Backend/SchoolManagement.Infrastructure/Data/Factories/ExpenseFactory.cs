using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Enums;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class ExpenseFactory : Factory<Expense>
{
    public ExpenseFactory(AppDbContext context) : base(context)
    {
    }

    protected override Task<Expense> Make()
    {
        var branches = Context.Branches.Select(b => b.Id).ToList();
        var staff = Context.DomainUsers.Select(u => u.Id).ToList();

        if (branches.Count == 0 || staff.Count == 0)
            return Task.FromResult<Expense>(null!);

        var expense = Expense.Create(
            category: faker.PickRandom<ExpenseType>(),
            payeeName: faker.Company.CompanyName(),
            amount: Math.Round(faker.Random.Decimal(100, 5000), 2),
            expenseDate: faker.Date.Recent(30),
            paymentMethod: faker.PickRandom<PaymentMethod>(),
            branchId: faker.PickRandom(branches),
            processedByStaffId: faker.PickRandom(staff),
            description: faker.Lorem.Sentence(),
            reference: faker.Random.Replace("REF-####")
        );

        return Task.FromResult(expense);
    }
}
