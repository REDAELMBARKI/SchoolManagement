using MediatR;
using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.DomainEvents.Students;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Domain.Utils;

namespace SchoolManagement.Application.Services.Students;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;
    private readonly IStudentQueryService _query;
    private readonly IMediator _mediator; 
    public StudentService(IStudentRepository repository, IStudentQueryService query, IMediator mediator)
    {
        _repository = repository;
        _query = query;
        _mediator = mediator;
    }

    public async Task<List<StudentResponseDto>> GetAllAsync()
    {
        return await _query.GetAllResponsesAsync();
    }

    public async Task<StudentResponseDto> GetByIdAsync(Guid id)
    {
        var student = await _query.GetResponseByIdAsync(id);
        if(student == null ) {
            throw new NotFoundException($"No student found with id {id}");
        }
        return student;
    }

    public async Task<StudentResponseDto> CreateAsync(StudentCommand command)
    {
        var generatedSlug = await CustomSluger.Slug(
            slug => _query.IsExistsBySlugAsync(slug), 
            command.FirstName, 
            command.LastName
        );
        command.Slug = generatedSlug;

        var student = StudentMapper.ToDomain(command);
        var createdStudent = await _repository.AddAsync(student);
        
        await _mediator.Publish(new StudentCreatedDomainEvent(createdStudent.Id));
        
        return StudentMapper.ToResponse(createdStudent);
    }

    public async Task<StudentResponseDto> UpdateAsync(Guid id, StudentRequestDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new NotFoundException($"No student found with id {id}");
        }
        
        existing.UpdateFirstName(dto.FirstName);
        existing.UpdateLastName(dto.LastName);
        existing.UpdateEmail(dto.Email);
        existing.UpdatePhone(dto.Phone);
        existing.UpdateDateOfBirth(dto.DateOfBirth);
        existing.UpdateGender(dto.GenderId);
        existing.UpdateIntakeId(dto.IntakeId);
        existing.UpdateIsDirectRegistration(dto.IsDirectRegistration);
        
        var updated = await _repository.UpdateAsync(existing);
        return StudentMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }


}