
namespace SchoolManagement.Backend.Models;
public class Intake : BaseEntity
{

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Slug {get;set;} = string.Empty ;

    public string Email { get; set;}  = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime IntakeDate { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public IntakeStatus Status {get;set;}
   
    public DateTime? FollowUpDate { get; set; } 
    public string? Notes { get; set; }   
    //fk
    public int? CommercialAgentId { get; set; } 
    public int GenderId { get; set; }  
    public int LeadSourceId {get;set;}
    public int SchoolProgramId { get; set; }
    public int BranchId {get;set;}


    // navigation
    public Gender Gender { get; set; } = null! ;
    public LeadSource? LeadSource { get; set; }
    public CommercialAgent? CommercialAgent { get; set; }
    public SchoolProgram SchoolProgram {get;set;} = null!;
    public Branch Branch {get;set;} = null!;
}


public enum IntakeStatus
{
    New,
    Contacted,
    Interested,
    Enrolled,
    NotInterested
}