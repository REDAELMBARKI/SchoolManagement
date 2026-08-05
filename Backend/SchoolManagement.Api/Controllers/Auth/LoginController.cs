using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Services;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Api.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class LoginController : ControllerBase
{
    JwtService _jwtService;
    UserManager<ApplicationUser> _userManager;
    public LoginController(UserManager<ApplicationUser> userManager, JwtService jwtService)
    {
        _jwtService = jwtService;
        _userManager = userManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return BadRequest(new { message = "wrong credentials " });
        }

        bool passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            return BadRequest(new { message = "wrong credentials " });
        }

        string token = _jwtService.generateToken(user);
        return Ok(token);
    }
}