using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;

namespace SchoolManagement.Application.Academic.Interfaces.Services;

public interface ISubjectService
{
    Task<List<SubjectResponseDto>> GetAllAsync();
    Task<SubjectResponseDto> GetByIdAsync(Guid id);
    Task<SubjectResponseDto> CreateAsync(SubjectCommand command);
    Task<SubjectResponseDto> UpdateAsync(Guid id, UpdateSubjectCommand command);
    Task DeleteAsync(Guid id);
}
