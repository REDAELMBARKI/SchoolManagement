using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;

namespace SchoolManagement.Domain.Entities
{
    public class Plan : AggregateRoot
    {
        public string Name { get; set; } = string.Empty; // "1 Month", "3 Months", "Full Year"
        public int DurationMonths { get; set; }            // 1, 3, 6, 12
        public decimal? DiscountPercent { get; set; }

        public virtual ICollection<Enrollment> Enrollments {get;set;} = new List<Enrollment>();


    }
}