using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;
using System;
using System.Reflection.Emit;

namespace SchoolManagement.Infrastructure.Data.Configurations.Entities;

public class LeadSourceConfiguration: IEntityTypeConfiguration<LeadSource>
{
     public  void Configure(EntityTypeBuilder<LeadSource> builder)
     {
            builder.HasDiscriminator<string>("LeadSourceType")
            .HasValue<AdLeadSource>("Ad")
            .HasValue<OpcLeadSource>("Opc");

            builder
            .HasOne(ls => ls.Branch)
            .WithMany()
            .HasForeignKey(ls => ls.BranchId);

    }
}
