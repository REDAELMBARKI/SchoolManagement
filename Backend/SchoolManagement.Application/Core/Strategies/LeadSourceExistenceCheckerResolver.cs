using SchoolManagement.Application.Core.Dtos.Requests;

namespace SchoolManagement.Application.Core.Strategies
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
