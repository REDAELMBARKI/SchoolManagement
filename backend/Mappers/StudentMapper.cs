using AutoMapper;
using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Mappers;

public class StudentMapper
{
    private readonly IMapper _mapper;

    public StudentMapper(IMapper mapper)
    {
        _mapper = mapper;
    }

    public StudentResponseDto ToStudentDto(Student student)
    {
        return new StudentResponseDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Slug = student.Slug,
            Email = student.Email,
            Phone = student.Phone,
            DateOfBirth = student.DateOfBirth,
            IntakeId = student.IntakeId,
            Intake = student.Intake != null ? new IntakeResponseDto
            {
                Id = student.Intake.Id,
                FirstName = student.Intake.FirstName,
                LastName = student.Intake.LastName,
                Email = student.Intake.Email,
                Phone = student.Intake.Phone,
                DateOfBirth = student.Intake.DateOfBirth,
                TotalFees = student.Intake.TotalFees,
                AmountPaid = student.Intake.AmountPaid,
            } : null,
            StudentParents = _mapper.Map<ICollection<StudentParentResponseDto>>(student.StudentParents),
            Enrollments = _mapper.Map<ICollection<EnrollmentResponseDto>>(student.Enrollments),
            Gender = MapGender(student.Gender)
        };
    }

    private static GenderResponseDto? MapGender(Gender? gender)
    {
        if (gender == null) return null;
        
        return new GenderResponseDto
        {
            Id = gender.Id,
            Name = gender.Name
        };
    }
}
