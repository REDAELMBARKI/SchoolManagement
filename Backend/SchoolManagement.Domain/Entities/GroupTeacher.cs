using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

public class GroupTeacher : BaseEntity
{
    public Guid TeacherId { get; private set; }
    public Guid GroupId { get; private set; }

    public virtual Teacher Teacher { get; private set; } = null!;
    public virtual Group Group { get; private set; } = null!;

    private GroupTeacher() { }

    public static GroupTeacher Create(Guid teacherId, Guid groupId)
    {
        if (teacherId == Guid.Empty)
            throw new DomainException("Teacher ID must not be empty.");
        if (groupId == Guid.Empty)
            throw new DomainException("Group ID must not be empty.");

        return new GroupTeacher
        {
            TeacherId = teacherId,
            GroupId = groupId
        };
    }

    public void UpdateTeacherId(Guid teacherId)
    {
        if (teacherId == Guid.Empty)
            throw new DomainException("Teacher ID must not be empty.");
        TeacherId = teacherId;
    }

    public void UpdateGroupId(Guid groupId)
    {
        if (groupId == Guid.Empty)
            throw new DomainException("Group ID must not be empty.");
        GroupId = groupId;
    }
}
