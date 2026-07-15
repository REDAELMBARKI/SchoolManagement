using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Mappers
{
    internal interface IMapper<T> where T : class
    {
        T MapToEntity<Rq>(Rq requestDto) where Rq : class;
        R MapToResponseDto<R>(T entity) where R : class;
    }
}
