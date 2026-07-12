using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SchoolManagement.Infrastructure.Data.Configurations.Extensions; 


public static class JwtExtension
{
     public static IServiceCollection  AddJwtConfigExtension(this IServiceCollection services , IConfiguration _configuration)
     {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                    ValidateIssuer = false,  
                    ValidateAudience = false, 
                    ValidateLifetime = true,   
                };
            });


           return services;
    }
}