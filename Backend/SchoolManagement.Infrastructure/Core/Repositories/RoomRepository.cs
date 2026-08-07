using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Infrastructure.Common.Repositories;
using SchoolManagement.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Infrastructure.Core.Repositories
{
    public class RoomRepository : Repository<Room>, IRoomRepository
    {
        public RoomRepository(AppDbContext context) : base(context) { } 
    }
}

