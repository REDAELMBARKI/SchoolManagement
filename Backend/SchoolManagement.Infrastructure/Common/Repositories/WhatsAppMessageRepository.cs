using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Common.Repositories;

public class WhatsAppMessageRepository : Repository<WhatsAppMessage>, IWhatsAppMessageRepository
{
    public WhatsAppMessageRepository(AppDbContext context) : base(context) { }
}
