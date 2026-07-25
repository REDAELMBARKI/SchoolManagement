using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mappers;

public static class GroupMapper
{
    public static GroupResponseDto ToResponse(Group group) => new()
    {
        Id = group.Id,
        Name = group.Name,
        Capacity = group.Capacity,
        Period = group.Period,
        Level = new LevelResponseDto
        {
            Id = group.Level.Id,
            Name = group.Level.Name
        },
        Subject = SubjectMapper.ToResponse(group.Subject),
        Teachers = group.Teachers.Select(gt => new GroupTeacherResponseDto
        {
            Id = gt.Id,
            TeacherId = gt.TeacherId,
            GroupId = gt.GroupId,
            Teacher = new TeacherResponseDto
            {
                Id = gt.Teacher.Id,
                FirstName = gt.Teacher.FirstName,
                LastName = gt.Teacher.LastName,
                Slug = gt.Teacher.Slug,
                Email = gt.Teacher.Email?.Value ?? null,
                Phone = gt.Teacher.Phone,
                DateOfBirth = gt.Teacher.DateOfBirth ?? default
            }
        }).ToList()
    };

    public static Group ToDomain(GroupCommand command)
    {
        return Group.Create(
            name: command.Name,
            capacity: command.Capacity,
            period: command.Period,
            branchId: command.BranchId,
            levelId: command.LevelId,
            subjectId: command.SubjectId
        );
    }
}
