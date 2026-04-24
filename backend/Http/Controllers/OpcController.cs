using System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend;

namespace SchoolManagement.Backend.Http.Controllers;


[ApiController]
[Route("api/opcs")]
public class OpcController : ControllerBase
{
 
   public readonly AppDbContext _context ;

   public OpcController(AppDbContext context)
   {
      _context = context;
   }

 
  [HttpGet]
   public async Task<IActionResult> GetAll()
   {
      var opcs = await _context.Opcs.ToListAsync();
      return Ok(opcs);
   }


   
    [HttpPost]

   public  async Task<IActionResult> AddOpc()
   {
      var opcs = new List<Opc>
      {
         new Opc {  
                    FirstName = "Morning" ,
                    LastName = "Session" ,
                    Email = "morning.session@example.com" ,
                    Phone = "123-456-7890" ,
                    GenderId = 1 ,
                    HireDate = DateTime.UtcNow.AddYears(-2) ,
                    IsActivated = false , 
                    Salary = 50000.00m ,
                    },
         new Opc {
                    FirstName = "Afternoon" ,
                    LastName = "Session" ,
                    Email = "test@exampple.com" ,
                    Phone = "098-765-4321" ,
                    GenderId = 2 ,
                    HireDate = DateTime.UtcNow.AddYears(-1) ,
                    IsActivated = true ,
                    Salary = 55000.00m
            }
      };

      await _context.Opcs.AddRangeAsync(opcs);
      await _context.SaveChangesAsync();
      return Ok(opcs);
   }

}
