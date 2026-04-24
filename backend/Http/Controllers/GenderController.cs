using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Http.Controllers;


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
          var genders = new List<Gender>
            {
                new Gender { Id = 1, Name = "Male" },
                new Gender { Id = 2, Name = "Female" }
            };

          await _context.Genders.AddRangeAsync(genders);
          await _context.SaveChangesAsync();
          return Ok(genders);
      }
}
