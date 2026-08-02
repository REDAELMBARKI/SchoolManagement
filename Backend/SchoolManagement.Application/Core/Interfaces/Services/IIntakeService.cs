<<<<<<< HEAD
﻿ using SchoolManagement.Application.Academic.Dtos.Commands;
=======
using SchoolManagement.Application.Academic.Dtos.Commands;
>>>>>>> 5fb5c4738af634e9e79c8340f0172f22f69d2a31
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;

namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface IIntakeService
{
    Task<IEnumerable<IntakeResponseDto>> GetAllIntakesAsync();
    Task<IntakeResponseDto?> GetIntakeByIdAsync(Guid id);
    Task<IntakeResponseDto> AddIntakeAsync(IntakeCommand command);
    Task<IntakeResponseDto?> UpdateAsync(Guid id, UpdateIntakeCommand command);
    Task DeleteIntakeAsync(Guid id);
}
