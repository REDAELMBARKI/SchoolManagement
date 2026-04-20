


using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using SchoolManagement.Models;

public class Intake : User
{
    //fk
    public int LeadSourceId {get;set;}
    public int OpcId { get; set; }
    public DateTime IntakeDate { get; set; }
    
    // navigation
    public LeadSource LeadSource { get; set; }
    public Opc Opc { get; set; }
}

