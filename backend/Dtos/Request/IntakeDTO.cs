using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Backend.Dtos;

public class IntakeDto : UserDto
{


        [Required]
        [DataType(DataType.Date)]
        public DateTime IntakeDate { get; set; }

        [Required]
        public int LeadSourceId { get; set; }

        public int GenderId { get; set; }
}
