using SchoolManagement.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;


namespace SchoolManagement.Domain.Entities;

public class Opc : Employee
{ 
    public virtual ICollection<LeadSource> LeadSources { get; set; } = new List<LeadSource>();
    [NotMapped]
    public IEnumerable<Intake> Intakes => LeadSources.SelectMany(ls => ls.Intakes).ToList();
}