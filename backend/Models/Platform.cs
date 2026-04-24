using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Backend.Models
{

    public class Platform
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }        // "Facebook", "Google Ads", "TikTok"
    }

}



