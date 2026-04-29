using FluentValidation;
using SchoolManagement.Backend.Interfaces;
using SchoolManagement.Backend.Models;
namespace SchoolManagement.Backend.Validators; 


public class IntakeValidator : AbstractValidator<IntakeDto>
{
    public IntakeValidator(
         IGenderRepository  gender_repo ,
         IBranchRepository  branch_repo ,
         ICommercialAgentRepository  commercialAgent_repo ,
         IOpcRepository  opc_repo 
         )
    {


        RuleFor( i => i.LeadSource)
        .NotEmpty()
        .SetValidator(new LeadSourceValidator())
        .When(i => !i.IsIndependent);
        

        RuleFor(i => i.GenderId)
        .NotEmpty()
        .MustAsync(async (genderId , ct) =>
        {
             return await gender_repo.ExistsAsync(genderId) ;
        }) ;

        RuleFor(i => i.BranchId)
        .NotEmpty()
        .MustAsync(async (branchId , ct) =>
        {
            return await branch_repo.ExistsAsync(branchId) ;
        }) ;


         RuleFor(i => i.CommercialAgentId)
        .MustAsync(async (commercialAgentId , ct) =>
        {
            return await commercialAgent_repo.ExistsAsync(commercialAgentId!.Value);
        })
        .When( i  => i.CommercialAgentId.HasValue);
         

         
    }
}


public class LeadSourceValidator : AbstractValidator<LeadSourceDto>
{  
    public LeadSourceValidator(IAdRepository ad_repo , IOpcRepository opc_repo)
    {
        RuleFor(l => l.LeadSourceId)
        .NotEmpty()
        .MustAsync((dto , leadSourceId , ct) => {
             return dto.LeadSourceType switch {
                  LeadSourceType.Opc => opc_repo.ExistsAsync(leadSourceId) ,
                  LeadSourceType.Ad => ad_repo.ExistsAsync(leadSourceId) ,
                  _ => false
             }
        })
        ;
    }

}