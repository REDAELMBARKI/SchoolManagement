using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Contexts;

public class LeadSourceContext : IEntityTypeConfiguration<LeadSource>
{
     public  void Configure(EntityTypeBuilder<LeadSource> builder)
    {
        
            builder
           .HasOne(ld => ld.Ad)
           .WithMany()
           .HasForeignKey(ld => ld.AdId);

           builder
           .HasOne(ld => ld.Opc)
           .WithMany()
           .HasForeignKey(ld => ld.OpcId);

    }
}
