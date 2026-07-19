using SchoolManagement.Application.Dtos.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Stratigies.LeadSourceExistence
{
    public  class LeadSourceExistenceCheckerResolver
    {
        public readonly IDictionary<LeadSourceType, ILeadSourceChecker> _checkers;
        public LeadSourceExistenceCheckerResolver(IEnumerable<ILeadSourceChecker> checkers)
        {
            _checkers = checkers.ToDictionary(c => c.SourceType);
        }

        public async Task<bool> IsExistsResolver(LeadSourceType sourceType, Guid SourceId)
        {
            return  await _checkers[sourceType].IsExistsChecker(SourceId);
        }
    }
}
