using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class GroupTeacher : BaseEntity
{
    public int TeacherId { get; set; }
    public int GroupId { get; set; }

    public virtual Teacher Teacher { get; set; } = null!;
    public virtual Group Group { get; set; } = null!;
}
