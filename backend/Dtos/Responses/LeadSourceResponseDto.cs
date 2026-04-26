using System;
using System.Text.Json.Serialization;

namespace SchoolManagement.Backend.Dtos.Responses;


[JsonPolymorphic]
[JsonDerivedType(typeof(AdResponseDto), "Ad")]
[JsonDerivedType(typeof(OpcResponseDto), "Opc")]

public abstract class LeadSourceResponseDto 
{ 

    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;

}


public class OpcResponseDto : LeadSourceResponseDto 
{
    public string FullName { get ; set; } = string.Empty;
}

public class AdResponseDto : LeadSourceResponseDto 
{
    public string PlatFormName { get ; set; } = string.Empty ;
}
