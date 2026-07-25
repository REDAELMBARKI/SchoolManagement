using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Domain.Utils;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Services.Intakes;

public class IntakeService : IIntakeService
{
    private readonly IIntakeRepository _repository;
    private readonly IIntakeQueryService _query;
    private readonly ICurrentUserContext _currentUserContext;
    public IntakeService(IIntakeRepository repository, IIntakeQueryService query, ICurrentUserContext currentUserContext)
    {
        _repository = repository;
        _query = query;
        _currentUserContext = currentUserContext;
    }

    public async Task<IEnumerable<IntakeResponseDto>> GetAllIntakesAsync()
    {
        return await _query.GetAllResponsesAsync();
    }

    public async Task<IntakeResponseDto?> GetIntakeByIdAsync(Guid id)
    {
        return await _query.GetResponseByIdAsync(id);
    }

    public async Task<IntakeResponseDto> AddIntakeAsync(IntakeCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");
        command.BranchId = branchId;

        var generatedSlug = await CustomSluger.Slug(slug => _query.IsExistsBySlugAsync(slug), command.FirstName, command.LastName);
        command.Slug = generatedSlug;

        Intake intake = IntakeMapper.ToDomain(command);
        var newEntity = await _repository.AddAsync(intake);
        return IntakeMapper.ToResponse(newEntity);
    }


   
    
    public async Task<IntakeResponseDto?> UpdateAsync(Guid id, IntakeCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");
        command.BranchId = branchId;

        var existingIntake = await _repository.GetByIdAsync(id);
        if (existingIntake is null)
        {
            throw new NotFoundException($"No intake found with id {id}");
        }
        
        if (existingIntake.HasStudents)
        {
            throw new DomainException("Cannot update an intake that already has students.");
        }
        
        var generatedSlug = await CustomSluger.Slug(slug => _query.IsExistsBySlugAsync(slug), command.FirstName, command.LastName);
        
        existingIntake.UpdateFirstName(command.FirstName);
        existingIntake.UpdateLastName(command.LastName);
        existingIntake.UpdateSlug(generatedSlug);
        existingIntake.UpdateGenderId(command.GenderId);
        existingIntake.UpdatePhone(command.Phone);
        existingIntake.UpdateEmail(command.Email);
        existingIntake.UpdateDateOfBirth(command.DateOfBirth);
        existingIntake.UpdateIntakeDate(command.IntakeDate);
        existingIntake.UpdateStatus(command.Status);
        existingIntake.UpdateFollowUpDate(command.FollowUpDate);
        existingIntake.UpdateNotes(command.Notes);
        existingIntake.UpdateCommercialAgentId(command.CommercialAgentId);
        existingIntake.UpdateLeadSourceId(command.LeadSourceId);
        existingIntake.UpdateSubjectId(command.SubjectId);
        existingIntake.UpdateBranchId(command.BranchId);
        existingIntake.UpdateIsIndependent(command.IsIndependent);
        existingIntake.UpdateTotalFees(command.TotalFees);
        existingIntake.UpdateAmountPaid(command.AmountPaid);
        
        existingIntake.UpdatedAt = DateTime.UtcNow;
        Intake  updatedIntake = await _repository.UpdateAsync(existingIntake);
        return IntakeMapper.ToResponse(updatedIntake); 
    }


    public async Task DeleteIntakeAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
 

}