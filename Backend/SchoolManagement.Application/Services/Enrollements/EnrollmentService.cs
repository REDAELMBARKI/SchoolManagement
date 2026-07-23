using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Services.Enrollements;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _repository;
    private readonly IEnrollmentQueryService _queryService;
    private readonly IGroupQueryService _groupQueryService;

    public EnrollmentService(IEnrollmentRepository repository, IEnrollmentQueryService queryService, IGroupQueryService groupQueryService)
    {
        _repository = repository;
        _queryService = queryService;
        _groupQueryService = groupQueryService;
    }

    public async Task<List<EnrollmentResponseDto>> GetAllAsync()
    {
        return await _queryService.GetAllResponsesAsync();
    }

    public async Task<EnrollmentResponseDto?> GetByIdAsync(Guid id)
    {
        return await _queryService.GetResponseByIdAsync(id);
    }

    public async Task<EnrollmentResponseDto> CreateAsync(EnrollmentRequestDto dto)
    {
        var availableGroupsWithSameLevel = await _groupQueryService.GetAvailableGroupsByLevelId(dto.LevelId);
        dto.GroupId = EvaluateStudentGroup(availableGroupsWithSameLevel, dto.PreferedScheduleId ,  dto.GroupId);
        var enrollment = EnrollmentMapper.ToDomain(dto);
        var created = await _repository.AddAsync(enrollment);
        return EnrollmentMapper.ToResponse(created);
    }

    public async Task<EnrollmentResponseDto> UpdateAsync(Guid id, UpdateEnrollmentRequestDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) throw new NotFoundException($"Enrollment with id {id} not found");

        existing.UpdateStudentId(dto.StudentId);
        existing.UpdateSubjectId(dto.SubjectId);
        existing.UpdateGroupId(dto.GroupId);
        existing.UpdateBranchId(dto.BranchId);
        existing.UpdatePlanId(dto.PlanId);
        existing.UpdateNotes(dto.Notes);
        
        var updated = await _repository.UpdateAsync(existing);
        return EnrollmentMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }


 
    private Guid EvaluateStudentGroup(List<Group> availableGroupsWithSameLevel,Guid? PreferedScheduleId,  Guid? groupId)
    {
        Guid? GroupId = groupId;
        if (groupId != null)
        {
            CheckGroupAvailability(availableGroupsWithSameLevel , groupId!.Value);
        }
        else
        {
            GroupId = AssignNewGroup(availableGroupsWithSameLevel , PreferedScheduleId);
        }

        return GroupId!.Value;
    }

    private Guid AssignNewGroup(List<Group> availableGroupsWithSameLevel , Guid? PreferedScheduleId)
    {
        Group? groupPrefered =  availableGroupsWithSameLevel.FirstOrDefault(g => g.Schedule.Id == PreferedScheduleId);
        if (groupPrefered == null) {
            return availableGroupsWithSameLevel.First().Id;
        }
        return groupPrefered.Id;
    }

    private void CheckGroupAvailability(List<Group> availableGroupsWithSameLevel , Guid groupId)
    { 

        if (!availableGroupsWithSameLevel.Any() || !availableGroupsWithSameLevel.Select(g => g.Id).Contains(groupId))
            throw new UnAvailableResourceException("No Empty Place  Available group with same level ");
    }


}
