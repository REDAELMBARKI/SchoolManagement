using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;

namespace SchoolManagement.Application.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _repository;
    private readonly IEnrollmentQueryService _queryService;

    public EnrollmentService(IEnrollmentRepository repository, IEnrollmentQueryService queryService)
    {
        _repository = repository;
        _queryService = queryService;
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
        var enrollment = EnrollmentMapper.ToDomain(dto);
        var created = await _repository.AddAsync(enrollment);
        return EnrollmentMapper.ToResponse(created);
    }

    public async Task<EnrollmentResponseDto> UpdateAsync(Guid id, EnrollmentRequestDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null) throw new NotFoundException($"Enrollment with id {id} not found");

        // TODO: Business logic for you to implement:
        // - Validate status transitions (Active -> Dropped, etc.)
        // - Handle group change (check new group capacity)

        var enrollment = EnrollmentMapper.ToDomain(dto);
        enrollment.Id = id;
        enrollment.Status = existing.Status;
        enrollment.EnrolledAt = existing.EnrolledAt;
        
        var updated = await _repository.UpdateAsync(enrollment);
        return EnrollmentMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
    


}
