using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;

namespace SchoolManagement.Domain.Entities
{
    public class Plan : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty; // "1 Month", "3 Months", "Full Year"
        public int DurationMonths { get; private set; }            // 1, 3, 6, 12
        public decimal BaseAmount { get; private set; }            // the actual fee before discount
        public decimal? DiscountPercent { get; private set; }
        public bool IsActive { get; private set; } = true;
        public int RemainingAmountDueDays { get; private set; }

        public virtual ICollection<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();

        private Plan() { }

        public decimal Amount => BaseAmount - (DiscountPercent.HasValue ? (BaseAmount * (DiscountPercent.Value / 100) ) : BaseAmount); 

        public static Plan Create(string name, int durationMonths, int remainingAmountDueDays , decimal baseAmount, decimal? discountPercent = null, bool isActive = true)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Plan name cannot be empty.");
            if (durationMonths <= 0)
                throw new DomainException("Duration months must be greater than zero.");
            if (baseAmount < 0)
                throw new DomainException("Base amount cannot be negative.");
            if (discountPercent.HasValue && (discountPercent.Value < 0 || discountPercent.Value > 100))
                throw new DomainException("Discount percent must be between 0 and 100.");

            return new Plan
            {
                Name = name,
                DurationMonths = durationMonths,
                BaseAmount = baseAmount,
                DiscountPercent = discountPercent,
                IsActive = isActive,
                RemainingAmountDueDays = remainingAmountDueDays
            };
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Plan name cannot be empty.");
            Name = name;
        }

        public void UpdateDurationMonths(int durationMonths)
        {
            if (durationMonths <= 0)
                throw new DomainException("Duration months must be greater than zero.");
            DurationMonths = durationMonths;
        }

        public void UpdateBaseAmount(decimal baseAmount)
        {
            if (baseAmount < 0)
                throw new DomainException("Base amount cannot be negative.");
            BaseAmount = baseAmount;
        }

        public void UpdateDiscountPercent(decimal? discountPercent)
        {
            if (discountPercent.HasValue && (discountPercent.Value < 0 || discountPercent.Value > 100))
                throw new DomainException("Discount percent must be between 0 and 100.");
            DiscountPercent = discountPercent;
        }

        public void UpdateIsActive(bool isActive)
        {
            IsActive = isActive;
        }

        public void UpdateRemainingAmountDueDate(int remainingAmountDueDate)
        {
            RemainingAmountDueDays = remainingAmountDueDate;
        }
    }
}