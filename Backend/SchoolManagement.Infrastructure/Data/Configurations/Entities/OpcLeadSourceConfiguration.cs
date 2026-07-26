using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities
{
    public class OpcLeadSourceConfiguration : IEntityTypeConfiguration<OpcLeadSource>
    {
        public void Configure(EntityTypeBuilder<OpcLeadSource> builder)
        {
            builder.HasOne(OpcL => OpcL.Opc)
                 .WithMany(o => o.LeadSources)
                 .HasForeignKey(OpcL => OpcL.OpcId);
        }
    }
}
