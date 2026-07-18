using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class CommercialAgentConfiguration : IEntityTypeConfiguration<CommercialAgent>
{
    public void Configure(EntityTypeBuilder<CommercialAgent> entityTypeBuilder)
    {
        // Explicitly set auto-increment Id for TPC
        entityTypeBuilder.Property(c => c.Id)
            .ValueGeneratedOnAdd();
                
        // Table mapping for CommercialAgent entity (TPC inherited from Employee)
        entityTypeBuilder.ToTable("CommercialAgents");

    }
}