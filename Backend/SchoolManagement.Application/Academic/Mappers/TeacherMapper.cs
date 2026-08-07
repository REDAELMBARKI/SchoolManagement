using SchoolManagement.Domain.Academic.Entities;

namespace SchoolManagement.Application.Academic.Mappers;

public static class TeacherMapper
{
    public static Teacher ToDomain(TeacherCommand command)
    {
        return Teacher.Register(
            firstName: command.FirstName,
            lastName: command.LastName,
            slug: command.Slug,
            genderId: command.GenderId,
            email: command.Email,
            phone: command.Phone,
            dateOfBirth: command.DateOfBirth,
            hireDate: command.HireDate,
            salary: command.Salary,
            branchId: command.BranchId,
            specialization: command.Specialization);
    }

    public static TeacherResponseDto ToResponse(Teacher teacher)
    {
        return new TeacherResponseDto
        {
            Id = teacher.Id,
            FirstName = teacher.FirstName,
            LastName = teacher.LastName,
            Slug = teacher.Slug,
            GenderId = teacher.GenderId,
            Email = teacher.Email?.Value,
            Phone = teacher.Phone,
            DateOfBirth = teacher.DateOfBirth,
            HireDate = teacher.HireDate,
            Salary = teacher.Salary,
            BranchId = teacher.BranchId,
            Specialization = teacher.Specialization,
            CreatedAt = teacher.CreatedAt
        };
    }
}
