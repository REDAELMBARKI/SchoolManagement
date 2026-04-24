using System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Http.Controllers;


[ApiController]
[Route("api/opcs")]
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
      var opcs = await _context.Ads.ToListAsync();
      return Ok(opcs);
   }


   
    [HttpPost]

   public  async Task<IActionResult> AddAd()
   {
      var platform1 =  new Platform { Name = "Google" } ;
      var platform2 =  new Platform { Name = "Facebook" } ;
      
     
      var opcs = new List<Ad>
      {
         new Ad
         {
               Name = "Google Ads" ,
               Platform  = platform1 ,
         },
            new Ad
            {
                Name = "Facebook Ads" ,
                Platform = platform2 ,
            }
      };

      await _context.Ads.AddRangeAsync(opcs);
      await _context.SaveChangesAsync();
      return Ok(opcs);
   }

}
