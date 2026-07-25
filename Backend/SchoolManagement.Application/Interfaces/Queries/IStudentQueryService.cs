using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries.Common;

namespace SchoolManagement.Domain.Interfaces.Queries;

public interface IStudentQueryService : IEntityQuery<Student>, ISluged
{
    Task<Student> FindByIdAsync(Guid id);
    Task<List<StudentResponseDto>> GetAllResponsesAsync();
    Task<StudentResponseDto?> GetResponseByIdAsync(Guid id);
    Task<bool> HasDuplicateByPhoneAsync(string phone, Guid? excludeId = null);
    Task<bool> HasDuplicateByEmailAsync(string? email, Guid? excludeId = null);
    Task<bool> HasDuplicateByNameDobAsync(string firstName, string lastName, DateOnly dateOfBirth, Guid? excludeId = null);
}
