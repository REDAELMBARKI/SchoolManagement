using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Entities;
using System;
using System.Reflection.Emit;

namespace SchoolManagement.Backend.Configurations;

public class LeadSourceConfiguration: IEntityTypeConfiguration<LeadSource>
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

            builder
            .HasOne(ls => ls.Branch)
            .WithMany()
            .HasForeignKey(ls => ls.BranchId);

  

    }
}
