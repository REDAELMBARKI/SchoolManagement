using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Api.Controllers;



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



    //[HttpPost]
    //public async Task<IActionResult> AddLeadSource()
    //{  
    //    var leadSources = new List<LeadSource>
    //    {
    //        new AdLeadSource {  
    //              AdId =  1 , 
    //              BeId = 1 ,
    //          },
    //          new LeadSource
    //          {
    //              AdId = 2 ,
    //              OpcId = 2

    //          }
    //    };    
    //    await _context.LeadSources.AddRangeAsync(leadSources);
    //    await _context.SaveChangesAsync();
    //    return Ok(leadSources); 
    //}
}
