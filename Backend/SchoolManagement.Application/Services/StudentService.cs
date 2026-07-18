using MediatR;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Events.Students;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Domain.Interfaces.Services;

namespace SchoolManagement.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;
    private readonly IMediator _mediator; 
    public StudentService(IStudentRepository repository, IMediator mediator)
    {
        _repository = repository;
        _mediator = mediator;
    }

    public async Task<List<StudentResponseDto>> GetAllAsync()
    {
        var students = await _repository.GetAllAsync();
        return students.Select(s => StudentMapper.ToResponse(s)).ToList();
    }

    public async Task<StudentResponseDto> GetByIdAsync(int id)
    {
        var student = await _repository.FindByIdAsync(id);
        return StudentMapper.ToResponse(student);
    }

    public async Task<StudentResponseDto> CreateAsync(StudentRequestDto dto)
    {
        var student_map = new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            DateOfBirth = dto.DateOfBirth,
            GenderId = dto.GenderId,
            IntakeId = dto.IntakeId
        };

        Student student  =  await _repository.AddAsync(student_map);
        // student creation events
        if (student.IntakeId != null)
        {
            this.ConvertFromIntakeMailer(student , _mediator);
        }
        else
        {
            this.NewStudentMailer(student , _mediator);
        }

        return StudentMapper.ToResponse(student);
    }

    public async Task<StudentResponseDto> UpdateAsync(int id, StudentRequestDto dto)
    {
        var existing = await _repository.FindByIdAsync(id);

        var student = new Student
        {
            Id = new Guid(id.ToString()),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            DateOfBirth = dto.DateOfBirth,
            GenderId = dto.GenderId,
            IntakeId = dto.IntakeId
        };

        await _repository.UpdateAsync(id, student);
        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
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