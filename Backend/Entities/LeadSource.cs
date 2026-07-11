using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Backend.Dtos;

namespace SchoolManagement.Backend.Entities;


public class LeadSource  : BaseEntity
{
    public LeadSourceType LeadSourceType {get;set;}
    //fks
    public int? OpcId { get; set; }
    public int? AdId { get; set; }

    public int BranchId { get; set; }

    // navigation properties
    public virtual Branch Branch {get;set;} = null! ;
    public virtual Opc? Opc { get; set; }
    public virtual Ad? Ad { get; set; }
    public virtual ICollection<Intake> Intakes { get; set; } = new List<Intake>();

}


public enum LeadSourceType  {
    Opc ,  Ad
}






