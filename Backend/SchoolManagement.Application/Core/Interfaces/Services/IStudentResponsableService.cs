using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Core.Dtos.Responses;

namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface IStudentResponsableService
{
    Task<StudentResponsableResponseDto> CreateAndLinkToStudentAsync(Guid studentId, StudentResponsableRequestDto request);
}
