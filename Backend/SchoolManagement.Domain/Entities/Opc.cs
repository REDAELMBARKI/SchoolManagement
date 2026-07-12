using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Backend.Entities;

namespace SchoolManagement.Backend.Entities;

public class Opc : Employee
{ 
    public virtual ICollection<LeadSource> LeadSources { get; set; } = new List<LeadSource>();
    [NotMapped]
    public IEnumerable<Intake> Intakes => LeadSources.SelectMany(ls => ls.Intakes).ToList();
}