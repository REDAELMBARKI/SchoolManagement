using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;

namespace SchoolManagement.Domain.Entities
{
    public class Plan : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty; // "1 Month", "3 Months", "Full Year"
        public int DurationMonths { get; private set; }            // 1, 3, 6, 12
        public decimal? DiscountPercent { get; private set; }

        public virtual ICollection<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();

        private Plan() { }

        public static Plan Create(string name, int durationMonths, decimal? discountPercent = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Plan name cannot be empty.");
            if (durationMonths <= 0)
                throw new DomainException("Duration months must be greater than zero.");
            if (discountPercent.HasValue && (discountPercent.Value < 0 || discountPercent.Value > 100))
                throw new DomainException("Discount percent must be between 0 and 100.");

            return new Plan
            {
                Name = name,
                DurationMonths = durationMonths,
                DiscountPercent = discountPercent
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

        public void UpdateDiscountPercent(decimal? discountPercent)
        {
            if (discountPercent.HasValue && (discountPercent.Value < 0 || discountPercent.Value > 100))
                throw new DomainException("Discount percent must be between 0 and 100.");
            DiscountPercent = discountPercent;
        }
    }
}