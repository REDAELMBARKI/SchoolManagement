using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Common.ValueObjects;

namespace SchoolManagement.Domain.Common.Entities;

public class DomainUser : Person
{
    public string ApplicationUserId { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;  // SuperAdmin, Director, Administrator, Receptionist, Teacher, CommercialAgent
    
    public Email? Email { get; private set; }
    public string? Phone { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    
    public bool IsActive { get; private set; } = true;
    // BranchId and Branch are inherited from Person base class

    public DateTime? LastActiveAt { get; private set; }

    private DomainUser() { }

    public static DomainUser Register(
        string firstName, 
        string lastName, 
        string email,
        string slug, 
        Guid? genderId, 
        string? phone,
        DateOnly? dateOfBirth,
        string role,
        Guid branchId,  // Now required (not nullable)
        string applicationUserId)
    {
        // Validation: SuperAdmin must use SYSTEM_BRANCH_ID (not a real branch)
        if (role == "SuperAdmin" && branchId != Branch.SYSTEM_BRANCH_ID)
            throw new DomainException("SuperAdmin must use SYSTEM_BRANCH_ID.");

        // Validation: Non-SuperAdmin must have a real BranchId (not SYSTEM_BRANCH_ID)
        if (role != "SuperAdmin" && branchId == Branch.SYSTEM_BRANCH_ID)
            throw new DomainException("User must be assigned to a real branch.");

        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");

        if (string.IsNullOrWhiteSpace(applicationUserId))
            throw new DomainException("Application user ID cannot be empty.");

        var validRoles = new[] { "SuperAdmin", "Director", "Administrator", "Reciptionest", "Teacher" };
        if (!validRoles.Contains(role))
            throw new DomainException($"Invalid role: {role}. Valid roles: {string.Join(", ", validRoles)}");

        var user = new DomainUser
        {
            ApplicationUserId = applicationUserId,
            Role = role,
            Email = !string.IsNullOrWhiteSpace(email) ? new Email(email) : null,
            Phone = phone,
            DateOfBirth = dateOfBirth,
            IsActive = true
        };
        
        user.RegisterPerson(firstName, lastName, slug, genderId, branchId);
        return user;
    }

    public void UpdateBranch(Guid branchId)
    {
        // SuperAdmin cannot change branch
        if (Role == "SuperAdmin")
            throw new DomainException("SuperAdmin cannot change branch (always SYSTEM_BRANCH_ID).");

        // Non-SuperAdmin must have a real branch (not SYSTEM_BRANCH_ID)
        if (branchId == Guid.Empty || branchId == Branch.SYSTEM_BRANCH_ID)
            throw new DomainException("User must be assigned to a real branch.");

        UpdateBranchId(branchId);
    }

    public void UpdateEmail(string? email)
    {
        Email = !string.IsNullOrWhiteSpace(email) ? new Email(email) : null;
    }

    public void UpdatePhone(string? phone)
    {
        Phone = phone;
    }

    public void UpdateDateOfBirth(DateOnly? dateOfBirth)
    {
        DateOfBirth = dateOfBirth;
    }

    public void ChangeRole(string newRole)
    {
        var validRoles = new[] { "SuperAdmin", "Director", "Administrator", "Receptionist", "Teacher", "CommercialAgent" };
        if (!validRoles.Contains(newRole))
            throw new DomainException($"Invalid role: {newRole}. Valid roles: {string.Join(", ", validRoles)}");

        // If changing to SuperAdmin, remove branch
        if (newRole == "SuperAdmin")
            BranchId = Branch.SYSTEM_BRANCH_ID;

        // If changing from SuperAdmin to other role, must assign branch later
        if (Role == "SuperAdmin" && newRole != "SuperAdmin" && BranchId == Guid.Empty)
            throw new DomainException("Must assign a branch when changing from SuperAdmin to another role.");

        Role = newRole;
    }

    public void LinkToApplicationUser(string applicationUserId)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
            throw new DomainException("Application user ID cannot be empty.");
        ApplicationUserId = applicationUserId;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void MarkActiveNow()
    {
        LastActiveAt = DateTime.UtcNow;
    }
}
