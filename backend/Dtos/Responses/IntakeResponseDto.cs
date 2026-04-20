using _.Models;

namespace _.Dtos.Responses;

public record IntakeResponseDto(
   int Id  ,
   string FirstName ,
   string LastName ,
   string Email ,
   string Phone , 
   DateTime IntakeDate ,
   LeadSource LeadSource , 
   Opc Opc ,
   Gender Gender 
); 