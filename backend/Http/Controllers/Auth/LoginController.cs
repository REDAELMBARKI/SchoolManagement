using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Models;
using SchoolManagement.Services;

namespace SchoolManagement.Http.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class LoginController : ControllerBase
{
    AppDbContext _context ;
    JwtService _jwtService ;
    public LoginController(AppDbContext context , JwtService jwtService)
    {
        _context = context ;
        _jwtService= jwtService ;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        bool exists = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (!exists)
        {
            return BadRequest(new {message = "wrong credentials "}) ;
        }
        User user = await _context.Users.Where(u => u.Email == request.Email)
                                            .FirstOrDefaultAsync();

        if(new PasswordHasher<User>()
        .VerifyHashedPassword(user , user.PasswordHash , request.Password) == 
         PasswordVerificationResult.Failed
        )
        {
           return BadRequest(new {message = "wrong credentials "}) ;
        };
        string token = _jwtService.generateToken(user) ;               
        return Ok(token) ;
    }
}