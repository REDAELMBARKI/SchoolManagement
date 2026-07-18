using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;
namespace SchoolManagement.Api.Controllers;


[ApiController]
[Route("api/genders")]
public class GenderController : ControllerBase
{
      private readonly AppDbContext _context;
      public GenderController(AppDbContext context)
      {
            _context = context;
      }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var genders = await _context.Genders.ToListAsync();
            return Ok(genders);
        }


      [HttpPost]
      public async Task<IActionResult> AddGender()
      {   
           throw new NotImplementedException();
      }
}
