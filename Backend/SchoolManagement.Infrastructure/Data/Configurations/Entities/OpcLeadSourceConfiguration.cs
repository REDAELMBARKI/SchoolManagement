using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities
{
    public class OpcLeadSourceConfiguration : IEntityTypeConfiguration<OpcLeadSource>
    {
        void IEntityTypeConfiguration<OpcLeadSource>.Configure(EntityTypeBuilder<OpcLeadSource> builder)
        {
            builder.HasOne(OpcL => OpcL.Opc)
                 .WithMany()
                 .HasForeignKey(OpcL => OpcL.OpcId);
        }
    
    }
}
