using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Http.Controllers;


[ApiController]
[Route("api/lead-sources")]
public class LeadSourceController : ControllerBase
{
      private readonly AppDbContext _context;
      public LeadSourceController(AppDbContext context)
      {
            _context = context;
      }
      
      [HttpGet]
      public async Task<IActionResult> GetAll()
      {
        //   var leadSources = await _context.LeadSources.ToListAsync();
          var leadSources = await _context.LeadSources.ToListAsync();  
          return Ok(leadSources);
      }



      [HttpPost]
      public async Task<IActionResult> AddLeadSource()
      {  
          var leadSources = new List<LeadSource>
          {
              new LeadSource {  
                    AdId =  1 , 
                    OpcId = 1 ,
                },
                new LeadSource
                {
                    AdId = 2 ,
                    OpcId = 2
                    
                }
          };    
          await _context.LeadSources.AddRangeAsync(leadSources);
          await _context.SaveChangesAsync();
          return Ok(leadSources); 
      }
}
