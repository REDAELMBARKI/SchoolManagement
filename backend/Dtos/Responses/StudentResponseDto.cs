using System;
using SchoolManagement.Models;

namespace _.Dtos.Responses;

public record StudentResponseDto
(
   int Id  ,
   string FirstName ,
   string LastName ,
   string Gender , 
   string Group , 
   string Level ,
   DateOnly DateOfBirth,
   ICollection<Parent> Parents

);
