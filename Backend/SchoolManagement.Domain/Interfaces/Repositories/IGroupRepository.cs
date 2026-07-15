using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Domain.Interfaces.Queries.Common;
using SchoolManagement.Domain.Interfaces.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Domain.Interfaces.Repositories
{
    public interface IGroupRepository : IRepository<Group> , IQuery<Group>
    {
    }
}
