using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Application.Common.Interfaces.Queries;

namespace SchoolManagement.Application.Core.Interfaces.Queries;

public interface IStudentQueryService : IEntityQuery<Student>, ISluged
{
    Task<Student> FindByIdAsync(Guid id);
    Task<List<StudentResponseDto>> GetAllResponsesAsync();
    Task<StudentResponseDto?> GetResponseByIdAsync(Guid id);
    Task<bool> HasDuplicateByPhoneAsync(string phone, Guid? excludeId = null);
    Task<bool> HasDuplicateByEmailAsync(string? email, Guid? excludeId = null);
    Task<bool> HasDuplicateByNameDobAsync(string firstName, string lastName, DateOnly dateOfBirth, Guid? excludeId = null);
}
