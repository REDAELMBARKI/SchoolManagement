using MediatR;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Events.Students;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Domain.Interfaces.Queries;

namespace SchoolManagement.Application.Services;

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
        
        // student creation events
        if (createdStudent.IntakeId != null)
        {
            this.ConvertFromIntakeMailer(createdStudent, _mediator);
        }
        else
        {
            this.NewStudentMailer(createdStudent, _mediator);
        }

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



    private void ConvertFromIntakeMailer(Student student ,  IMediator _mediator)
    {
        // update the intake status 

        IntakeConvertedToStudentEvent args = new IntakeConvertedToStudentEvent(student);
        _mediator.Publish<IntakeConvertedToStudentEvent>(args);
    }

    private void NewStudentMailer(Student student, IMediator _mediator)
    {
        NewStudentAsignedEvent args = new NewStudentAsignedEvent(student);
        _mediator.Publish<NewStudentAsignedEvent>(args);
    }
}