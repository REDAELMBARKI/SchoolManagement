using SchoolManagement.Application.Core.Dtos.Requests;

namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface IStudentService
{
    Task<List<StudentResponseDto>> GetAllAsync();
    Task<StudentResponseDto> GetByIdAsync(Guid id);
    Task<StudentResponseDto> CreateAsync(StudentCommand command);
    Task<StudentResponseDto> UpdateAsync(Guid id, UpdateStudentCommand command);
    Task EnsureNoDuplicateStudentAsync(StudentCommand command);
    Task DeleteAsync(Guid id);
    Task<StudentResponseDto> TransferBranchAsync(Guid studentId, TransferBranchCommand command);
    Task<List<StudentResponsableResponseDto>> GetParentsByStudentIdAsync(Guid studentId);
    Task<StudentResponsableResponseDto> AddParentToStudentAsync(Guid studentId, StudentResponsableRequestDto request);
    Task RemoveParentFromStudentAsync(Guid studentId, Guid parentId);
}
