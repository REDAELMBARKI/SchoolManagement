using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Backend.Dtos;

namespace SchoolManagement.Backend.Models ;


public class LeadSource  : BaseEntity
{
    public LeadSourceType LeadSourceType {get;set;}

    //fks
    public int? OpcId { get; set; }
    public int? AdId { get; set; }

    public int BranchId { get; set; }

    // navigation properties
    public Branch Branch {get;set;} = null! ;
    public Opc? Opc { get; set; }
    public Ad? Ad { get; set; }
    public ICollection<Intake> Intakes { get; set; } = new List<Intake>();

}






