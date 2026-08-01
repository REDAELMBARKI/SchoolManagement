using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Api.Controllers;


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
       throw new NotImplementedException();
    }

}
