using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class OpcConfiguration : IEntityTypeConfiguration<Opc>
{
    public void Configure(EntityTypeBuilder<Opc> entityTypeBuilder)
    {
        // Email is optional for Employees
        entityTypeBuilder
            .OwnsOne(emp => emp.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(100)
                    .IsRequired(false);
            });

        // Explicitly set auto-increment Id for TPC
        entityTypeBuilder.Property(o => o.Id)
            .ValueGeneratedOnAdd();
                
        // Table mapping for Opc entity (TPC inherited from Employee)
        entityTypeBuilder.ToTable("Opcs");

    }
}
