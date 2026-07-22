using MediatR;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.DomainEvents.Students;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Domain.Interfaces.Queries;

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

    public async Task<StudentResponseDto> CreateAsync(StudentRequestDto dto)
    {
        var student = StudentMapper.ToDomain(dto);
        var createdStudent = await _repository.AddAsync(student);
        return StudentMapper.ToResponse(createdStudent);
    }

    public async Task<StudentResponseDto> UpdateAsync(Guid id, StudentRequestDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new NotFoundException($"No student found with id {id}");
        }
        
        var student = StudentMapper.ToDomain(dto);
        student.Id = id;
        
        var updated = await _repository.UpdateAsync(student);
        return StudentMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }


}