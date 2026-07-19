using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Domain.Interfaces.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Stratigies.LeadSourceExistence
{
    public class AdLeadSourceChecker : ILeadSourceChecker
    {
        public readonly IAdQueryService _ad_query;
        public  LeadSourceType SourceType => LeadSourceType.Ad;
        public AdLeadSourceChecker(IAdQueryService ad_query)
        {
            _ad_query = ad_query;
        }
        public async Task<bool> IsExistsChecker(Guid id)
        {
           return await _ad_query.IsExistsAsync(id);
        }
    }
}
