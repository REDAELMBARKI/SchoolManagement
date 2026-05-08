using SchoolManagement.Backend.Dtos.Requests;
using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Mappers;

public static class GroupMapper
{
    public static Group ToEntity(GroupRequestDto dto) => new()
    {
        Name = dto.Name,
        Capacity = dto.Capacity,
        Period = dto.Period,
        LevelId = dto.LevelId,
        SubjectId = dto.SubjectId
    };

    public static GroupResponseDto MapGroup(Group group) => new()
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
        Subject = SubjectMapper.MapSubject(group.Subject),
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
                Email = gt.Teacher.Email,
                Phone = gt.Teacher.Phone,
                DateOfBirth = gt.Teacher.DateOfBirth ?? default
            }
        }).ToList()
    };
}
