using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

public class Room : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; } = 20;
    public string? Floor { get; private set; }
    public string? Description { get; private set; }
    public Guid BranchId { get; private set; }
    public virtual Branch Branch { get; private set; } = null!;

    private Room() { }

    public static Room Create(string name, int capacity, string? floor, string? description, Guid branchId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Room name cannot be empty.");
        }
        if (capacity <= 0)
        {
            throw new DomainException("Capacity must be greater than zero.");
        }
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }

        return new Room
        {
            Name = name,
            Capacity = capacity,
            Floor = floor,
            Description = description,
            BranchId = branchId
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Room name cannot be empty.");
        }
        Name = name;
    }

    public void UpdateCapacity(int capacity)
    {
        if (capacity <= 0)
        {
            throw new DomainException("Capacity must be greater than zero.");
        }
        Capacity = capacity;
    }

    public void UpdateFloor(string? floor)
    {
        Floor = floor;
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public void UpdateBranchId(Guid branchId)
    {
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }
        BranchId = branchId;
    }
}
