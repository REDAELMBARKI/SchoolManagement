using FluentValidation;
using SchoolManagement.Backend.Models;
namespace SchoolManagement.Backend.Validators; 


public class IntakeValidator : AbstractValidator<Intake>
{
    public IntakeValidator()
    {
        RuleFor(i => i.LeadSourceId)
                .NotNull() ;
                // .when(i => i.LeadSource is "Opc" || i.LeadSource == "Ad");
    }
}