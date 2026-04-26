using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Dtos.Responses;

public class IntakeResponseDto {
    public int Id {get;set;}
    public string FirstName { get; set; }  = string.Empty ;
    public string LastName { get; set; } = string.Empty ;
    public string Email { get; set; } = string.Empty ;
    public string Phone { get; set; } = string.Empty ;
    public DateTime IntakeDate { get; set; }

    public DateTime? DateOfBirth { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Notes { get; set; }
    public IntakeStatus Status { get; set; }

    public LeadSourceResponseDto? LeadSource { get; set; }
    public GenderResponseDto Gender { get; set; } = null!;
    public SchoolProgramResponseDto SchoolProgram { get; set; } = null!;
    public BranchResponseDto Branch { get; set; } = null!;
    public CommercialAgentResponseDto? CommercialAgent { get; set; }
} 


// GenderResponseDto.cs
public class GenderResponseDto
{
    public int Id { get; set; }
    public string Slug {get;set;} = string.Empty ;
    public string Name { get; set; } = string.Empty;
}

// SchoolProgramResponseDto.cs
public class SchoolProgramResponseDto
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