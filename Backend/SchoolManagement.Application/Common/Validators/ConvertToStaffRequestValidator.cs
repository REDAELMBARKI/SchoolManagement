using FluentValidation;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.Domain.Common.Utils;

namespace SchoolManagement.Application.Common.Validators;

public class ConvertToStaffRequestValidator : AbstractValidator<ConvertToStaffRequestDto>
{
    public ConvertToStaffRequestValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(role => new[] { RoleHelper.Director, RoleHelper.Administrator, RoleHelper.Reciptionest, RoleHelper.Teacher }.Contains(role))
            .WithMessage($"Role must be one of: {RoleHelper.Director}, {RoleHelper.Administrator}, {RoleHelper.Reciptionest}, {RoleHelper.Teacher}");

        RuleFor(x => x.BranchId)
            .NotEmpty().WithMessage("BranchId is required");
    }
}
