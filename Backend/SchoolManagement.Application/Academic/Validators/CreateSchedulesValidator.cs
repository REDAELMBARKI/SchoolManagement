using FluentValidation;
using SchoolManagement.Application.Academic.Dtos.Requests;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Queries;

namespace SchoolManagement.Application.Academic.Validators;

public class CreateSchedulesValidator : AbstractValidator<CreateSchedulesRequestDto>
{
    public CreateSchedulesValidator(
        IGroupQueryService groupQueryService,
        IDayQueryService dayQueryService,
        IRoomQueryService roomQueryService,
        ITeacherQueryService teacherQueryService,
        ISubjectQueryService subjectQueryService)
    {
        RuleFor(r => r.Schedules)
            .NotEmpty()
            .WithMessage("At least one schedule must be provided.");

        RuleForEach(r => r.Schedules).ChildRules(schedule =>
        {
            schedule.RuleFor(s => s.GroupId)
                .NotEmpty()
                .WithMessage("GroupId is required.")
                .MustAsync(async (groupId, ct) => await groupQueryService.IsExistsAsync(groupId))
                .WithMessage("Selected group does not exist.");

            schedule.RuleFor(s => s.DayId)
                .NotEmpty()
                .WithMessage("DayId is required.")
                .MustAsync(async (dayId, ct) => await dayQueryService.IsExistsAsync(dayId))
                .WithMessage("Selected day does not exist.");

            schedule.RuleFor(s => s.RoomId)
                .NotEmpty()
                .WithMessage("RoomId is required.")
                .MustAsync(async (roomId, ct) => await roomQueryService.IsExistsAsync(roomId))
                .WithMessage("Selected room does not exist.");

            schedule.RuleFor(s => s.TeacherId)
                .NotEmpty()
                .WithMessage("TeacherId is required.")
                .MustAsync(async (teacherId, ct) => await teacherQueryService.IsExistsAsync(teacherId))
                .WithMessage("Selected teacher does not exist.");

            schedule.RuleFor(s => s.SubjectId)
                .NotEmpty()
                .WithMessage("SubjectId is required.")
                .MustAsync(async (subjectId, ct) => await subjectQueryService.IsExistsAsync(subjectId))
                .WithMessage("Selected subject does not exist.");

            schedule.RuleFor(s => s.StartTime)
                .NotEmpty()
                .WithMessage("StartTime is required.")
                .Must((dto, startTime) => startTime < dto.EndTime)
                .WithMessage("StartTime must be before EndTime.");

            schedule.RuleFor(s => s.EndTime)
                .NotEmpty()
                .WithMessage("EndTime is required.");
        });
    }
}
