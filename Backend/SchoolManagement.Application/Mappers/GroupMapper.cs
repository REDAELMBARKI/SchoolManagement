using SchoolManagement.Application.Dtos.Requests;
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
                Email = null,
                Phone = gt.Teacher.Phone,
                DateOfBirth = gt.Teacher.DateOfBirth ?? default
            }
        }).ToList()
    };

    public static Group ToDomain(GroupRequestDto dto) => new()
    {
        Name = dto.Name,
        Capacity = dto.Capacity,
        Period = dto.Period,
        LevelId = new Guid(dto.LevelId.ToString()),
        SubjectId = new Guid(dto.SubjectId.ToString())
    };
}
