using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Contexts;

public  class IntakeContext : IEntityTypeConfiguration<Intake> {

      public  void Configure(EntityTypeBuilder<Intake> builder)
      {

            builder
            .HasOne(i => i.Gender)
            .WithMany()
            .HasForeignKey(i => i.GenderId)
            .OnDelete(DeleteBehavior.Restrict) ;

            builder
            .HasOne(i => i.LeadSource)
            .WithMany(ls => ls.Intakes)
            .HasForeignKey(i => i.LeadSourceId)
            .OnDelete(DeleteBehavior.SetNull);

            //  intake -> branch , branch -> intakes

            builder
            .HasOne(i => i.Branch)
            .WithMany()
            .HasForeignKey(i => i.BranchId)
            .OnDelete(DeleteBehavior.Restrict) ;


             builder
            .HasOne(i => i.SchoolProgram)
            .WithMany()
            .HasForeignKey(i => i.SchoolProgramId)
            .OnDelete(DeleteBehavior.Restrict) ;


             builder
            .HasOne(i => i.CommercialAgent)
            .WithMany()
            .HasForeignKey(i => i.CommercialAgentId)
            .OnDelete(DeleteBehavior.Restrict) ;
      }
}