using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Http.Requests;

public class StudentRequest : UserRequest
{
        [Required]
        public int LevelId { get; set; }

        [Required]
        public int GroupId { get; set; }

}