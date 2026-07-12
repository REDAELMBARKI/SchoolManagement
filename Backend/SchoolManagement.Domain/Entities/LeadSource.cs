using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Backend.Dtos;
using SchoolManagement.Backend.Interfaces.Models;

namespace SchoolManagement.Backend.Entities;


public class LeadSource  : BaseEntity
{
    public int BranchId { get; set; }
    public virtual Branch Branch {get;set;} = null! ;
    public virtual ICollection<Intake> Intakes { get; set; } = new List<Intake>();

}


internal class  OpcLeadSource : LeadSource  , ILeadSource
{

     public int OpcId { get; set; }
     public virtual Opc Opc { get; set; } = null!;
}

internal class  AdLeadSource  : LeadSource , ILeadSource{
    public int AdId { get; set; }
    public virtual Ad Ad { get; set; } = null!;
}





