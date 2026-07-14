using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Domain.Entities;


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





