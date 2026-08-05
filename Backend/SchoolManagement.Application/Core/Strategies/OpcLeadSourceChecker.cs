using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Core.Interfaces.Queries;

namespace SchoolManagement.Application.Core.Strategies
{
    public class OpcLeadSourceChecker : ILeadSourceChecker
    {
        public readonly IOpcQueryService _opc_query;
        public LeadSourceType SourceType => LeadSourceType.Opc;


        public OpcLeadSourceChecker(IOpcQueryService ad_query)
        {
            _opc_query = ad_query;
        }

        public async Task<bool> IsExistsChecker(Guid id)
        {
            return await _opc_query.IsExistsAsync(id);
        }
    }
}
