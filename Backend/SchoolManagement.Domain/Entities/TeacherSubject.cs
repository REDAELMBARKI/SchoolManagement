using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Domain.Entities;

public class TeacherSubject : BaseEntity
{
    public virtual Teacher Teacher { get; private set; } = null!;
    public virtual Subject Subject { get; private set; } = null!;

    public Guid TeacherId { get; private set; }
    public Guid SubjectId { get; private set; }

    private TeacherSubject() { }

    public static TeacherSubject Create(Guid teacherId, Guid subjectId)
    {
        if (teacherId == Guid.Empty)
            throw new DomainException("Teacher ID must not be empty.");
        if (subjectId == Guid.Empty)
            throw new DomainException("Subject ID must not be empty.");

        return new TeacherSubject
        {
            TeacherId = teacherId,
            SubjectId = subjectId
        };
    }

    public void UpdateTeacherId(Guid teacherId)
    {
        if (teacherId == Guid.Empty)
            throw new DomainException("Teacher ID must not be empty.");
        TeacherId = teacherId;
    }

    public void UpdateSubjectId(Guid subjectId)
    {
        if (subjectId == Guid.Empty)
            throw new DomainException("Subject ID must not be empty.");
        SubjectId = subjectId;
    }
}
