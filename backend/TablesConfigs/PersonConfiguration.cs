using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.TablesConfigs;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> entityTypeBuilder)
    {
        // TPT mapping for Person hierarchy (excluding Employee which uses TPT)
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
