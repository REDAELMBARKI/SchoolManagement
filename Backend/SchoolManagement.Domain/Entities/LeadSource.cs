using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Domain.Entities;


public class LeadSource  : AggregateRoot
{
    public int BranchId { get; set; }
    public virtual Branch Branch {get;set;} = null! ;
    public virtual ICollection<Intake> Intakes { get; set; } = new List<Intake>();

}


public class  OpcLeadSource : LeadSource 
{

     public int OpcId { get; set; }
     public virtual Opc Opc { get; set; } = null!;
}
public class  AdLeadSource  : LeadSource 
{
    public int AdId { get; set; }
    public virtual Ad Ad { get; set; } = null!;
}





