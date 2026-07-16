using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class GroupTeacher : BaseEntity
{
    public Guid TeacherId { get; set; }
    public Guid GroupId { get; set; }

    public virtual Teacher Teacher { get; set; } = null!;
    public virtual Group Group { get; set; } = null!;
}
