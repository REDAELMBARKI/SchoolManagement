using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> entityTypeBuilder)
    {
        // TODO: Implement MediaConfiguration
    }
}
