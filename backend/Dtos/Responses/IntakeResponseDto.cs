using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Dtos.Responses;

public class IntakeResponseDto {
    public int Id {get;set;}
    public string FirstName { get; set; }  = string.Empty ;
    public string LastName { get; set; } = string.Empty ;
    public string Slug { get; set; } = string.Empty ;
    public string Email { get; set; } = string.Empty ;
    public string Phone { get; set; } = string.Empty ;
    public DateTime IntakeDate { get; set; }

    public DateOnly? DateOfBirth { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Notes { get; set; }
    public IntakeStatus Status { get; set; }

    // Financial properties
    public bool IsIndependent { get; set; }
    public decimal TotalFees { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountRemaining => TotalFees - AmountPaid;
    public bool IsFullyPaid => AmountRemaining <= 0;

    public LeadSourceResponseDto? LeadSource { get; set; }
    public GenderResponseDto Gender { get; set; } = null!;
    public SubjectResponseDto Subject { get; set; } = null!;
    public BranchResponseDto Branch { get; set; } = null!;
    public CommercialAgentResponseDto? CommercialAgent { get; set; }
    public StudentResponseDto? ConvertedToStudent { get; set; }
} 

// StudentResponseDto.cs
public class StudentResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public int? IntakeId { get; set; }
    public IntakeResponseDto? Intake { get; set; }
    public ICollection<Parent> Parents { get; set; } = new List<Parent>();
}


// GenderResponseDto.cs
public class GenderResponseDto
{
    public int Id { get; set; }
    public string Slug {get;set;} = string.Empty ;
    public string Name { get; set; } = string.Empty;
}

// SubjectResponseDto.cs
public class SubjectResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug {get;set;} = string.Empty ;

    public string? Description { get; set; }
}

// BranchResponseDto.cs
public class BranchResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug {get;set;} = string.Empty ;

    public string City { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
}

// CommercialAgentResponseDto.cs
public class CommercialAgentResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
    public string Slug {get;set;} = string.Empty ;
    public string FullName => $"{FirstName} {LastName}";
    public string? Email { get; set; }
    public string? Phone { get; set; }
}