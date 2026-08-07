namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface IEnrollmentService
{
    Task<List<EnrollmentResponseDto>> GetAllAsync();
    Task<EnrollmentResponseDto?> GetByIdAsync(Guid id);
    Task<EnrollmentResponseDto> CreateAsync(EnrollmentCommand command);
    Task<EnrollmentResponseDto> UpdateAsync(Guid id, UpdateEnrollmentCommand command);
    Task<EnrollmentResponseDto> DropEnrollmentAsync(DropEnrollmentCommand command);
    Task<EnrollmentResponseDto> CompleteEnrollmentAsync(CompleteEnrollmentCommand command);
    Task<EnrollmentResponseDto> TransferGroupAsync(TransferGroupCommand command);
    Task<EnrollmentResponseDto> AddCreditAsync(Guid id, decimal amount);
    Task DeleteAsync(Guid id);
    
    /// <summary>
    /// Enrolls an existing student in an additional group/subject.
    /// Used when a student already exists and wants to add another subject (e.g., adding Math to existing English enrollment).
    /// </summary>
    /// <param name="studentId">The existing student's ID</param>
    /// <param name="command">Enrollment details with payment options</param>
    /// <returns>The created enrollment</returns>
    Task<EnrollmentResponseDto> EnrollStudentInAdditionalGroupAsync(Guid studentId, EnrollStudentInAdditionalGroupCommand command);
    
    /// <summary>
    /// Checks for schedule conflicts before enrollment or group transfer.
    /// Returns conflict details instead of throwing exceptions.
    /// </summary>
    Task<ScheduleConflictResponseDto> CheckScheduleConflictsAsync(Guid studentId, Guid groupId, Guid? excludeEnrollmentId = null);
}
