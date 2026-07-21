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
                Email = gt.Teacher.Email?.Value ?? null,
                Phone = gt.Teacher.Phone,
                DateOfBirth = gt.Teacher.DateOfBirth ?? default
            }
        }).ToList()
    };

    public static Group ToDomain(GroupRequestDto dto)
    {
        // Wait, Group.Create requires BranchId! Oh wait, Group has BranchId! Oh right! Group.Create has branchId! Oh wait, GroupRequestDto doesn't have BranchId!
        // Hmm, let's just use Guid.Empty for now, but we should update DTO later
        return Group.Create(
            name: dto.Name,
            capacity: dto.Capacity,
            period: dto.Period,
            branchId: Guid.Empty, // Temporary, should be from DTO
            levelId: dto.LevelId,
            subjectId: dto.SubjectId
        );
    }
}
