using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Backend.Models ;


public class LeadSource
{
    public int Id { get; set; }
    public string? Name {get; set;}  = string.Empty ;


    //fks
    public int? OpcId { get; set; }
    public int? AdId { get; set; }


    // navigation properties
    public Opc? Opc { get; set; }
    public Ad? Ad { get; set; }

}






