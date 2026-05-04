using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.TablesConfigs;

public class OpcConfiguration : IEntityTypeConfiguration<Opc>
{
    public void Configure(EntityTypeBuilder<Opc> entityTypeBuilder)
    {
        // Table mapping for Opc entity (inherits from Employee using TPT)
        entityTypeBuilder.ToTable("Opcs");

    }
}
