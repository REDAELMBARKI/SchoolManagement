using MediatR;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Events.Students;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Repositories;

namespace SchoolManagement.Application.Services;

public class StudentService
{
    private readonly StudentRepository _repository;
    private readonly StudentMapper _mapper;
    private readonly IMediator _mediator; 
    public StudentService(StudentRepository repository, StudentMapper mapper, IMediator mediator)
    {
        _repository = repository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<List<StudentResponseDto>> GetAllAsync()
    {
        var students = await _repository.GetAllAsync();
        return students.Select(s => _mapper.ToStudentDto(s)).ToList();
    }

    public async Task<StudentResponseDto> GetByIdAsync(int id)
    {
        var student = await _repository.FindByIdAsync(id);
        return _mapper.ToStudentDto(student);
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

        return new StudentResponseDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Gender = new GenderResponseDto
            {
                Id = student.Gender.Id,
                Slug = student.Gender.Slug,
                Name = student.Gender.Name
            },
            Parents = student.StudentParents.Select(sp => sp.Parent).ToList(),
            DateOfBirth = student.DateOfBirth,
            IntakeId = student.IntakeId,
            Intake = student.Intake != null ? new IntakeResponseDto
            {
                Id = student.Intake.Id,
                FirstName = student.Intake.FirstName,
                LastName = student.Intake.LastName,
                Email = student.Intake.Email,
                Phone = student.Intake.Phone,
                IntakeDate = student.Intake.IntakeDate,
                Status = student.Intake.Status,
                Slug = student.Intake.Slug,
                CreatedAt = student.Intake.CreatedAt,
                DateOfBirth = student.Intake.DateOfBirth,
                FollowUpDate = student.Intake.FollowUpDate,
                Notes = student.Intake.Notes,
                TotalFees = student.Intake.TotalFees,
                AmountPaid = student.Intake.AmountPaid,
                IsIndependent = student.Intake.IsIndependent,
                Gender = new GenderResponseDto
                {
                    Id = student.Intake.Gender.Id,
                    Slug = student.Intake.Gender.Slug,
                    Name = student.Intake.Gender.Name
                },
                Subject = new SubjectResponseDto
                {
                    Id = student.Intake.Subject.Id,
                    Slug = student.Intake.Subject.Slug,
                    Name = student.Intake.Subject.Name,
                    Description = student.Intake.Subject.Description
                },
                Branch = new BranchResponseDto
                {
                    Id = student.Intake.Branch.Id,
                    Slug = student.Intake.Branch.Slug,
                    Name = student.Intake.Branch.Name,
                    City = student.Intake.Branch.City,
                    Address = student.Intake.Branch.Address,
                    Phone = student.Intake.Branch.Phone
                }
            } : null,
        };
    }

    public async Task<StudentResponseDto> UpdateAsync(int id, StudentRequestDto dto)
    {
        var existing = await _repository.FindByIdAsync(id);

        var student = new Student
        {
            Id = id,
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



    private void ConvertFromIntakeMailer(Student student  ,  IMediator _mediator)
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