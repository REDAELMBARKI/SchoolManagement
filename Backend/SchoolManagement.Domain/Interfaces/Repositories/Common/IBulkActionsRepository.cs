using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Domain.Interfaces.Repositories.Common
{
    public interface IBulkActionsRepository<T> where T : class
    {
        Task<List<T>> AddManyAsync(IEnumerable<T> entities);
            Task UpdateManyAsync(IEnumerable<T> entities);
            Task DeleteManyAsync(IEnumerable<int> ids);
    }
}
