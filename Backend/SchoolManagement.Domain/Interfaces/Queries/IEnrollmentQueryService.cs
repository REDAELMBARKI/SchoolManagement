using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Domain.Interfaces.Queries.Common;

namespace SchoolManagement.Domain.Interfaces.Queries;

public interface IEnrollmentQueryService : IQuery<Enrollment>
{
}
