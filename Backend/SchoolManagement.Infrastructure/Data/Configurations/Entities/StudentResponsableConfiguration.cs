using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class StudentResponsableConfiguration : IEntityTypeConfiguration<StudentResponsable>
{
    public void Configure(EntityTypeBuilder<StudentResponsable> entityTypeBuilder)
    {
        entityTypeBuilder.Property(p => p.Id)
            .ValueGeneratedOnAdd();
                
        entityTypeBuilder.ToTable("StudentResponsables", tb =>
        {
            tb.HasCheckConstraint("CK_StudentResponsable_Email", "Email LIKE '%@%.%'");
        });

        entityTypeBuilder.Property(p => p.Email)
            .IsRequired(false)
            .HasMaxLength(255);
            
        entityTypeBuilder.Property(p => p.Phone)
            .IsRequired()
            .HasMaxLength(20);
            
        entityTypeBuilder.Property(p => p.Relationship)
            .IsRequired();

        entityTypeBuilder.HasIndex(p => p.Email).IsUnique();
        entityTypeBuilder.HasIndex(p => p.Phone);
        entityTypeBuilder.HasIndex(p => p.Relationship);
    }
}
