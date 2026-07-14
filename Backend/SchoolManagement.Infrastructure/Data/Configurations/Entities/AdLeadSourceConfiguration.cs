using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities
{
    public class AdLeadSourceConfiguration : IEntityTypeConfiguration<AdLeadSource>
    {
        void IEntityTypeConfiguration<AdLeadSource>.Configure(EntityTypeBuilder<AdLeadSource> builder)
        {
            builder.HasOne(AdL => AdL.Ad)
                 .WithMany()
                 .HasForeignKey(AdL => AdL.AdId);
        }
    }
}
