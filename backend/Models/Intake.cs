
using SchoolManagement.Backend.Dtos;

namespace SchoolManagement.Backend.Models;
public class Intake : Person
{

    public DateTime IntakeDate { get; set; }
    public IntakeStatus Status {get;set;}
    public DateTime? FollowUpDate { get; set; } 
    public string? Notes { get; set; }   
    //fk
    public int? CommercialAgentId { get; set; } 
    public int? LeadSourceId { get; set; }
    public int SchoolProgramId { get; set; }
    public int BranchId {get;set;}
    // financial properties
    public bool IsIndependent { get; set; } = false;
    public decimal TotalFees { get; set; }
    public decimal AmountPaid { get; set; }
    // navigation
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