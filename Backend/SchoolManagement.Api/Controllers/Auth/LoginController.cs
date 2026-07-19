using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Application.Services;
using SchoolManagement.Infrastructure.Data ;
using SchoolManagement.Domain.Interfaces.Queries;

namespace SchoolManagement.Api.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class LoginController : ControllerBase
{
    JwtService _jwtService ;
    IUserQueryService _user_query;
    public LoginController(IUserQueryService Userquery, JwtService jwtService)
    {
        _jwtService= jwtService ;
        _user_query = Userquery;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        User? user = await _user_query.GetByEmailAsync(request.Email);
        if (user is null)
        {
            return BadRequest(new {message = "wrong credentials "}) ;
        }

        if(user.Password is null) return BadRequest(new {message = " your account is not activated "}) ;
        if(new PasswordHasher<User>()
        .VerifyHashedPassword(user , user.Password , request.Password) == 
         PasswordVerificationResult.Failed
        )
        {
           return BadRequest(new {message = "wrong credentials "}) ;
        };
        string token = _jwtService.generateToken(user) ;               
        return Ok(token) ;
    }
}