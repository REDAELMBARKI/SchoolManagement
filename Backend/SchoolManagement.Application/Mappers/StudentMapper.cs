using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class StudentMapper
{
    public static Student ToDomain(StudentResponseDto dto)
    {
        return new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            DateOfBirth = dto.DateOfBirth,
            GenderId = dto.GenderId,
            IntakeId = dto.IntakeId
        };
    }

    public static StudentResponseDto ToResponse(Student student)
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
                Email = student.Intake.Email?.Value ?? "Email Empty",
                Phone = student.Intake.Phone,
                DateOfBirth = student.Intake.DateOfBirth,
                TotalFees = student.Intake.TotalFees,
                AmountPaid = student.Intake.AmountPaid,
            } : null,
            Parents = student.Parents != null ? student.Parents.Select(p => ParentMapper.ToResponse(p)).ToList() : null,
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
