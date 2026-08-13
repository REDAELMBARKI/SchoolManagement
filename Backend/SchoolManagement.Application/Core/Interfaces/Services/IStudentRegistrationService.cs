using SchoolManagement.Application.Core.Dtos.Requests;

namespace SchoolManagement.Application.Core.Interfaces.Services;

/// <summary>
/// Service interface for orchestrating student registration flow.
/// Coordinates student creation, enrollment, invoice generation, and initial payment processing.
/// </summary>
public interface IStudentRegistrationService
{
    /// <summary>
    /// Registers a new student with complete enrollment and payment setup.
    /// </summary>
    /// <param name="request">Student registration request containing student, enrollment, and payment details</param>
    /// <returns>Complete registration response with created entities</returns>
    Task<StudentRegistrationResponseDto> RegisterStudentAsync(StudentRegistrationRequestDto request);
}
