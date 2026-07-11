using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Entities;

namespace SchoolManagement.Backend.Configurations;

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
