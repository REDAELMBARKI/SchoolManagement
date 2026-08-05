using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class PaymentFactory : Factory<Payment>
{
    public PaymentFactory(AppDbContext context) : base(context)
    {
    }

    protected override async Task<Payment> Make()
    {
        var enrollments = await Context.Enrollments.Select(e => new { e.Id, e.BranchId }).ToListAsync();
        var enrollment = enrollments.Any() ? faker.PickRandom(enrollments) : null;
        var statuses = new[] { PaymentStatus.Pending, PaymentStatus.Completed, PaymentStatus.Failed };

        var amount = faker.Finance.Amount(100, 1000);
        var transferFees = faker.Random.Bool(0.3f) ? faker.Finance.Amount(1, 20) : (decimal?)null;
        var isCompleted = faker.Random.Bool();
        var paidAt = faker.Date.Past();
        var methods = Enum.GetValues<PaymentMethod>();
        var method = faker.PickRandom(methods);
        var status = isCompleted ? PaymentStatus.Completed : faker.PickRandom(statuses.Where(s => s != PaymentStatus.Completed));
        var currencies = new[] { "USD", "EUR", "MAD", "GBP" };

        return Payment.Create(
            enrollmentId: enrollment?.Id ?? Guid.Empty,
            amount: amount,
            status: status,
            paidAt: paidAt,
            branchId: enrollment?.BranchId ?? Guid.Empty,
            receivedByStaffId: Guid.Empty,
            transferFees: transferFees,
            method: method,
            externalReferenceCode: faker.Random.Bool() ? faker.Finance.RoutingNumber() : null,
            methodDetailsJson: "{}"
        );
    }
}
