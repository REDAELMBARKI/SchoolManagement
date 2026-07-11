using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Entities;

namespace SchoolManagement.Backend.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> entityTypeBuilder)
    {
        // Configure Id for auto-increment for TPC
        entityTypeBuilder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        entityTypeBuilder.UseTpcMappingStrategy();
        // Shared Person property validations
        entityTypeBuilder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        entityTypeBuilder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(50);

        entityTypeBuilder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(100);

    

        entityTypeBuilder.Property(p => p.GenderId)
            .IsRequired(false);

        // Shared Person indexes
        entityTypeBuilder.HasIndex(p => p.FirstName);
        entityTypeBuilder.HasIndex(p => p.LastName);
        entityTypeBuilder.HasIndex(p => p.Slug);
        entityTypeBuilder.HasIndex(p => p.GenderId);
    }
}
