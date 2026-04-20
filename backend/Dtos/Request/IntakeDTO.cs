namespace SchoolManagement.DTOs;

public class IntakeDTO : UserDTO
{

     public DateTime IntakeDate { get; set; }
     public int LeadSourceId { get; set; }
     public int OpcId { get; set; }
}
