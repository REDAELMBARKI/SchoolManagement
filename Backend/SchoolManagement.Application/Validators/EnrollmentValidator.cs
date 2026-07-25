
using FluentValidation;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Domain.Interfaces.Queries;

namespace SchoolManagement.Application.Validators;

public class EnrollmentValidator : AbstractValidator<EnrollmentRequestDto>
{
    public EnrollmentValidator(
        ISubjectQueryService subjectQueryService,
        IScheduleQueryService scheduleQueryService,
        IStudentQueryService studentQueryService,
        IPlanQueryService planQueryService,
        IGroupQueryService groupQueryService)
    {
        // Basic required fields
        RuleFor(e => e.LevelId).NotEmpty();
        RuleFor(e => e.StudentId).NotEmpty();
        RuleFor(e => e.SubjectId).NotEmpty();
        RuleFor(e => e.PlanId).NotEmpty();
        // Validate FKs exist
        RuleFor(e => e.SubjectId)
            .MustAsync(async (subjectId, ct) => await subjectQueryService.IsExistsAsync(subjectId))
            .WithMessage("Selected subject does not exist.");

        RuleFor(e => e.PreferedScheduleId)
            .MustAsync(async (scheduleId, ct) => scheduleId.HasValue && await scheduleQueryService.IsExistsAsync(scheduleId.Value))
            .WithMessage("Selected schedule does not exist.");


        RuleFor(e => e.StudentId)
            .MustAsync(async (studentId, ct) => await studentQueryService.IsExistsAsync(studentId))
            .WithMessage("Selected student does not exist.")
            .When(e => e.StudentId != Guid.Empty); 

        RuleFor(e => e.PlanId)
            .MustAsync(async (planId, ct) =>
            {
                var plan = await planQueryService.GetByIdAsync(planId);
                return plan != null;
            })
            .WithMessage("Selected plan does not exist.");

        RuleFor(e => e)
            .MustAsync(async (dto, ct) =>
            {
                var plan = await planQueryService.GetByIdAsync(dto.PlanId);
                return plan == null || plan.IsActive;
            })
            .WithMessage("Selected plan is inactive or not exists.")
            .When(e => e.PlanId != Guid.Empty);


        RuleFor(e => e.GroupId)
            .MustAsync(async (groupId, ct) =>
            {
                if (!groupId.HasValue || groupId.Value == Guid.Empty) return true;
                return await groupQueryService.IsExistsAsync(groupId.Value);
            })
            .WithMessage("Selected group does not exist.")
            .When(e => e.GroupId.HasValue && e.GroupId.Value != Guid.Empty);

        // Basic LevelId non-empty validation (since no ILevelQueryService exists yet)
        RuleFor(e => e.LevelId)
            .Must(levelId => levelId != Guid.Empty)
            .WithMessage("Level must be selected.");


        // check if grroupIId is for the thr prefered schedule
        When(e => e.GroupId.HasValue && e.PreferedScheduleId.HasValue, () =>
        {
            RuleFor(e => e)
              .MustAsync(async (e , ct) =>
              {
                  var group = await groupQueryService.GetByIdAsync(e.GroupId!.Value);
                  return e.PreferedScheduleId == group!.Schedule.Id;
              });
        });
    }
}
