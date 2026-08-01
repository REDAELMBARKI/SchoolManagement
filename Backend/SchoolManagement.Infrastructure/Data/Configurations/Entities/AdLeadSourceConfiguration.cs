using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities
{
    public class AdLeadSourceConfiguration : IEntityTypeConfiguration<AdLeadSource>
    {
        public void Configure(EntityTypeBuilder<AdLeadSource> builder)
        {
            builder.HasOne(AdL => AdL.Ad)
                 .WithMany()
                 .HasForeignKey(AdL => AdL.AdId);
        }
    }
}
