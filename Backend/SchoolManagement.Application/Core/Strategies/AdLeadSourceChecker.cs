using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Core.Interfaces.Queries;

namespace SchoolManagement.Application.Core.Strategies
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
