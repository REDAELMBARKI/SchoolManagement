
using FluentValidation;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Domain.Interfaces.Queries;

namespace SchoolManagement.Application.Validators;

public class EnrollmentValidator : AbstractValidator<EnrollmentRequestDto>
{
    public EnrollmentValidator(
        ISubjectQueryService subjectQueryService,
        IScheduleQueryService scheduleQueryService,
        IStudentQueryService studentQueryService)
    {
        // Basic required fields
        RuleFor(e => e.PreferedScheduleId).NotEmpty();
        RuleFor(e => e.LevelId).NotEmpty();
        RuleFor(e => e.StudentId).NotEmpty();
        RuleFor(e => e.SubjectId).NotEmpty();
        RuleFor(e => e.PlanId).NotEmpty();

        // Validate FKs exist
        RuleFor(e => e.SubjectId)
            .MustAsync(async (subjectId, ct) => await subjectQueryService.IsExistsAsync(subjectId))
            .WithMessage("Selected subject does not exist.");

        RuleFor(e => e.PreferedScheduleId)
            .MustAsync(async (scheduleId, ct) => await scheduleQueryService.IsExistsAsync(scheduleId))
            .WithMessage("Selected schedule does not exist.");

        RuleFor(e => e.StudentId)
            .MustAsync(async (studentId, ct) => await studentQueryService.IsExistsAsync(studentId))
            .WithMessage("Selected student does not exist.");
    }
}

