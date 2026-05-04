using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.TablesConfigs;

public class CommercialAgentConfiguration : IEntityTypeConfiguration<CommercialAgent>
{
    public void Configure(EntityTypeBuilder<CommercialAgent> entityTypeBuilder)
    {
        // Table mapping for CommercialAgent entity (inherits from Employee using TPT)
        entityTypeBuilder.ToTable("CommercialAgents");

    }
}
