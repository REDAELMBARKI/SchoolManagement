using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Domain.Core.Interfaces
{
    public interface IDomainUserRepository : IRepository<DomainUser>
    {
    }
}
