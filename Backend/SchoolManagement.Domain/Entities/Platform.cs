using SchoolManagement.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Domain.Entities
{

    public class Platform : AggregateRoot
    {
        public string Slug {get;set;} = string.Empty ;
      
        public string Name { get; set; } = string.Empty ;     
       
    
    }

}



