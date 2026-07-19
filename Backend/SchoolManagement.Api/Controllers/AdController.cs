using System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;
namespace SchoolManagement.Api.Controllers;


[ApiController]
[Route("api/ads")]
public class AdController : ControllerBase
{
 
   public readonly AppDbContext _context ;

   public AdController(AppDbContext context)
   {
      _context = context;
   }


   [HttpGet]
   public async Task<IActionResult> GetAll()
   {
      var opcs = await _context.Ads.Include(ad => ad.Platform).ToListAsync();
      return Ok(opcs);
   }


   
    [HttpPost]

   public  async Task<IActionResult> AddAd()
   {
       throw new NotImplementedException();
    }

}
