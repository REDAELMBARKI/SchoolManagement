using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Entities;

namespace SchoolManagement.Backend.Configurations;

public class OpcConfiguration : IEntityTypeConfiguration<Opc>
{
    public void Configure(EntityTypeBuilder<Opc> entityTypeBuilder)
    {
        // Explicitly set auto-increment Id for TPC
        entityTypeBuilder.Property(o => o.Id)
            .ValueGeneratedOnAdd();
                
        // Table mapping for Opc entity (TPC inherited from Employee)
        entityTypeBuilder.ToTable("Opcs");

    }
}
