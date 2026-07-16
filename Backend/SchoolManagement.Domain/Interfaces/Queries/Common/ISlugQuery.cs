using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Domain.Interfaces.Queries.Common
{
    public interface ISlugQuery
    {
        Task<bool> IsExistBySlug(string slug);
    }
}
