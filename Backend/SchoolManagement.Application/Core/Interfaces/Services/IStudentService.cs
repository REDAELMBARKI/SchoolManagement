namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface IStudentService
{
    Task<List<StudentResponseDto>> GetAllAsync();
    Task<StudentResponseDto> GetByIdAsync(Guid id);
    Task<StudentResponseDto> CreateAsync(StudentCommand command);
    Task<StudentResponseDto> UpdateAsync(Guid id, UpdateStudentCommand command);
    Task EnsureNoDuplicateStudentAsync(StudentCommand command);
    Task DeleteAsync(Guid id);

}
