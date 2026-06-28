namespace SchoolManagement.Backend.Configurations.Extenstions; 


public static class JwtExtension
{
     public static IServiceCollection  AddJwtConfigExtension(this IServiceCollection services ,  IContiguration _configuration)
     {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                    ValidateIssuer = false,  // skip for simple project
                    ValidateAudience = false,  // skip for simple project
                    ValidateLifetime = true,   // ✅ checks token expiry
                };
            });


           return services;
    }
}