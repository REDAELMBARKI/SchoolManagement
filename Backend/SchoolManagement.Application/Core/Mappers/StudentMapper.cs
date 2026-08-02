using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Core.Mappers;

public static class StudentMapper
{
    public static Student ToDomain(StudentCommand command)
    {
        return Student.Register(
            firstName: command.FirstName,
            lastName: command.LastName,
            slug: command.Slug,
            genderId: command.GenderId,
            email: command.Email,
            phone: command.Phone,
            dateOfBirth: command.DateOfBirth,
            intakeId: command.IntakeId,
            isDirectRegistration: command.IsDirectRegistration,
            branchId: command.BranchId
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
            CreditBalance = student.CreditBalance,
            IntakeId = student.IntakeId,
            IsDirectRegistration = student.IsDirectRegistration,
            BranchId = student.BranchId,
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
            StudentResponsables = student.StudentResponsables != null ? student.StudentResponsables.Select(p => StudentResponsableMapper.ToResponse(p)).ToList() : null,
            Gender = MapGender(student.Gender),
            Branch = student.Branch != null ? new BranchResponseDto
            {
                Id = student.Branch.Id,
                Slug = student.Branch.Slug,
                Name = student.Branch.Name,
                City = student.Branch.City,
                Address = student.Branch.Address,
                Phone = student.Branch.Phone
            } : null
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
