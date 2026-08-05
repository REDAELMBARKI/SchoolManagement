using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Domain.Academic.Entities;

namespace SchoolManagement.Application.Core.Interfaces.Queries;

public interface ITeacherQueryService : IEntityQuery<Teacher>
{
    Task<Teacher?> GetBySlugAsync(string slug);
    Task<Teacher?> GetByEmailAsync(string email);

}
