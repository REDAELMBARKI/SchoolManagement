using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class PaymentFactory : Factory<Payment>
{
    public PaymentFactory(AppDbContext context) : base(context)
    {
    }

    protected override async Task<Payment> Make()
    {
        var enrollments = await Context.Enrollments.Select(e => e.Id).ToListAsync();
        var statuses = new[] { "Pending", "Paid", "Overdue" };

        var periodStart = faker.Date.Past();
        var periodEnd = periodStart.AddMonths(1);
        var feeAmount = faker.Finance.Amount(100, 1000);
        var isPaid = faker.Random.Bool();
        var amountPaid = isPaid ? feeAmount : faker.Finance.Amount(0, feeAmount);
        var paidAt = isPaid ? faker.Date.Past() : (DateTime?)null;
        var status = isPaid ? "Paid" : faker.PickRandom(statuses.Where(s => s != "Paid"));

        return Payment.Create(
            enrollmentId: enrollments.Any() ? faker.PickRandom(enrollments) : Guid.Empty,
            feeAmount: feeAmount,
            periodStart: periodStart,
            periodEnd: periodEnd,
            amountPaid: amountPaid,
            paidAt: paidAt,
            status: status
        );
    }
}
