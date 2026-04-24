


namespace SchoolManagement.Backend.Models;
public class Intake 
{

    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set;}  = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public int GenderId { get; set; }
    public Gender Gender { get; set; } = null! ;

    //fk
    public int LeadSourceId {get;set;}
    public DateTime IntakeDate { get; set; }
    // navigation
    public LeadSource? LeadSource { get; set; }
}


