using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class StudentMapper
{
    public static Student ToDomain(StudentRequestDto dto)
    {
        // Temporary slug, proper slug generation should happen in service layer with IsRecordExists check
        var initialSlug = $"{dto.FirstName}-{dto.LastName}".ToLowerInvariant();
        
        return Student.Register(
            firstName: dto.FirstName,
            lastName: dto.LastName,
            slug: initialSlug,
            genderId: dto.GenderId,
            email: dto.Email,
            phone: dto.Phone,
            dateOfBirth: dto.DateOfBirth,
            intakeId: dto.IntakeId
        );
    }

    public static StudentResponseDto ToResponse(Student student)
    {
        return new StudentResponseDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Slug = student.Slug,
            Email = student.Email?.Value ?? string.Empty,
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
