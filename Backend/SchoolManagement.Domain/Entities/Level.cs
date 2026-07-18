using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

public class Level : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public Guid BranchId { get; private set; }
    public virtual Branch Branch { get; private set; } = null!;
    public int Order { get; private set; } = 1;

    private Level() { }

    public static Level Create(string name, Guid branchId, int order)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Level name cannot be empty.");
        }
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }

        return new Level
        {
            Name = name,
            BranchId = branchId,
            Order = order
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Level name cannot be empty.");
        }
        Name = name;
    }

    public void UpdateBranchId(Guid branchId)
    {
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }
        BranchId = branchId;
    }

    public void UpdateOrder(int order)
    {
        Order = order;
    }
}
