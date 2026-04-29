using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Backend.Models ;


public class LeadSource  : BaseEntity
{
    
    public string? Name {get; set;}  = string.Empty ;
    

    //fks
    public int? OpcId { get; set; }
    public int? AdId { get; set; }

    public int BranchId { get; set; }

    // navigation properties
    public Branch Branch {get;set;} = null! ;
    public Opc? Opc { get; set; }
    public Ad? Ad { get; set; }

}






