using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Backend.Models
{

    public class Platform : BaseEntity
    {
        public string Slug {get;set;} = string.Empty ;
      
        public string Name { get; set; } = string.Empty ;       // "Facebook", "Google Ads", "TikTok"
        //fk
        public int BranchId { get; set; }

        // navigation
        public Branch Branch {get;set;} = null! ;
    
    }

}



