using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Domain.Interfaces.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Stratigies.LeadSourceExistence
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
