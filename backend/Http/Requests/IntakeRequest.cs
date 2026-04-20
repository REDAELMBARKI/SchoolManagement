using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SchoolManagement.Http.Requests;

public class IntakeRequest : UserRequest
{

        [Required]
        [DataType(DataType.Date)]
        public DateTime IntakeDate { get; set; }

        [Required]
        public int OpcId { get; set; }

        [Required]
        public int LeadSourceId { get; set; }

        public int GenderId { get; set; }

} 