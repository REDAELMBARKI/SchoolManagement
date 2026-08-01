using SchoolManagement.Application.Academic.Dtos.Requests;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Queries;
using System;
using System.Collections.Generic;
using System.Text;

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
