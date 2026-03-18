using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bogus.DataSets;
using Microsoft.IdentityModel.Tokens;
using SchoolManagement.Models;

namespace SchoolManagement.Services;

public class JwtService
{
   IConfiguration _config ;
    public JwtService(IConfiguration config)
    {
        _config = config ;
    }
    public string generateToken(User user)
    { 
      var claims = new[]
      {
          new Claim(ClaimTypes.NameIdentifier , user.Id.ToString()),
          new Claim(ClaimTypes.Email , user.Email),
          new Claim(ClaimTypes.Name , user.FirstName + " " + user.LastName),
      } ;

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:secret"])) ; 
      var creds  = new SigningCredentials(key , SecurityAlgorithms.HmacSha256) ;
      
      var token = new JwtSecurityToken(
        issuer : null, 
        audience :null,
         claims : claims,
         notBefore :DateTime.UtcNow,
         expires :DateTime.UtcNow.AddDays(7),
         signingCredentials : creds
      );
       return new JwtSecurityTokenHandler().WriteToken(token) ;
        
    }

    
}