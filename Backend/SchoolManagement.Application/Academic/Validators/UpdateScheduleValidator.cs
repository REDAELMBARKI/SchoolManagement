using FluentValidation;
using SchoolManagement.Application.Academic.Dtos.Requests;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Queries;

namespace SchoolManagement.Application.Academic.Validators;

public class UpdateScheduleValidator : AbstractValidator<UpdateScheduleRequestDto>
{
    public UpdateScheduleValidator(
        IDayQueryService dayQueryService,
        IRoomQueryService roomQueryService,
        ITeacherQueryService teacherQueryService,
        ISubjectQueryService subjectQueryService)
    {
        RuleFor(r => r.DayId)
            .NotEmpty()
            .WithMessage("DayId is required.")
            .MustAsync(async (dayId, ct) => await dayQueryService.IsExistsAsync(dayId))
            .WithMessage("Selected day does not exist.");

        RuleFor(r => r.RoomId)
            .NotEmpty()
            .WithMessage("RoomId is required.")
            .MustAsync(async (roomId, ct) => await roomQueryService.IsExistsAsync(roomId))
            .WithMessage("Selected room does not exist.");

        RuleFor(r => r.TeacherId)
            .NotEmpty()
            .WithMessage("TeacherId is required.")
            .MustAsync(async (teacherId, ct) => await teacherQueryService.IsExistsAsync(teacherId))
            .WithMessage("Selected teacher does not exist.");

        RuleFor(r => r.SubjectId)
            .NotEmpty()
            .WithMessage("SubjectId is required.")
            .MustAsync(async (subjectId, ct) => await subjectQueryService.IsExistsAsync(subjectId))
            .WithMessage("Selected subject does not exist.");

        RuleFor(r => r.StartTime)
            .NotEmpty()
            .WithMessage("StartTime is required.")
            .Must((dto, startTime) => startTime < dto.EndTime)
            .WithMessage("StartTime must be before EndTime.");

        RuleFor(r => r.EndTime)
            .NotEmpty()
            .WithMessage("EndTime is required.");
    }
}
