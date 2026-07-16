using AutoMapper;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Domain.Interfaces.Queries.Common;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Domain.Utils;

namespace SchoolManagement.Application.Services;

public class IntakeService
{
    private readonly IIntakeRepository _repository;
    private readonly IIntakeQueryService _query;
    public IntakeService(IIntakeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<IntakeResponseDto>> GetAllIntakesAsync()
    {
        return await _query.GetAllAsync();
        
    }

    public async Task<IntakeResponseDto?> GetIntakeByIdAsync(Guid id)
    {
        return await _query.GetByIdAsync(id);
    }

    public async Task<IntakeResponseDto> AddIntakeAsync(IntakeRequestDto intakeDto)
    {
        Intake intake = IntakeMapper.ToEntity(intakeDto);
        var generatedSlug = await CustomSluger.Slug(slug => _query.IsExistBySlug(slug), intake.FirstName, intake.LastName);
        intake.UpdateSlug(generatedSlug);
        var newEntity = await _repository.AddAsync(intake);
        return IntakeMapper.ToResponse(newEntity);
    }

   
    
    public async Task UpdateAsync(Guid id, IntakeRequestDto intakeDto)
    {
        // First get the existing entity from the repository
        var existingIntake = await _repository.GetByIdForUpdateAsync(id);
        if (existingIntake is null)
        {
            throw new NotFoundException($"No intake found with id {id}");
        }
        
        // Generate new slug if needed
        var generatedSlug = await CustomSluger.Slug(slug => _query.IsExistBySlug(slug), intakeDto.FirstName, intakeDto.LastName);
        
        // Now call all the update methods on the existing entity directly from the DTO
        existingIntake.UpdateFirstName(intakeDto.FirstName);
        existingIntake.UpdateLastName(intakeDto.LastName);
        existingIntake.UpdateSlug(generatedSlug);
        existingIntake.UpdateGenderId(intakeDto.GenderId);
        // Note: Email is a value object not fully implemented yet, so we skip it for now
        existingIntake.UpdatePhone(intakeDto.Phone);
        existingIntake.UpdateDateOfBirth(intakeDto.DateOfBirth);
        existingIntake.UpdateIntakeDate(intakeDto.IntakeDate);
        existingIntake.UpdateStatus(intakeDto.Status);
        existingIntake.UpdateFollowUpDate(intakeDto.FollowUpDate);
        existingIntake.UpdateNotes(intakeDto.Notes);
        existingIntake.UpdateCommercialAgentId(intakeDto.CommercialAgentId);
        existingIntake.UpdateLeadSourceId(intakeDto.LeadSource.LeadSourceId);
        existingIntake.UpdateSubjectId(intakeDto.SubjectId);
        existingIntake.UpdateBranchId(intakeDto.BranchId);
        existingIntake.UpdateIsIndependent(intakeDto.IsIndependent);
        existingIntake.UpdateTotalFees(intakeDto.TotalFees);
        existingIntake.UpdateAmountPaid(intakeDto.AmountPaid);
        
        existingIntake.UpdatedAt = DateTime.UtcNow;
        // Now save the updated entity via repository
        await _repository.UpdateAsync(existingIntake);
    }


    public async Task DeleteIntakeAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
 

}