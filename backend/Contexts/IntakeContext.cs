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
            .HasForeignKey(i => i.GenderId);

            builder
            .HasOne(i => i.LeadSource)
            .WithMany(ls => ls.Intakes)
            .HasForeignKey(i => i.LeadSourceId);

            //  intake -> branch , branch -> intakes

            builder
            .HasOne(i => i.Branch)
            .WithMany()
            .HasForeignKey(i => i.BranchId) ;


             builder
            .HasOne(i => i.Subject)
            .WithMany()
            .HasForeignKey(i => i.SubjectId) ;


             builder
            .HasOne(i => i.CommercialAgent)
            .WithMany()
            .HasForeignKey(i => i.CommercialAgentId);


            // ConvertedToStudent relationship
            builder.HasOne(i => i.ConvertedToStudent)
                  .WithMany()
                  .HasForeignKey(i => i.ConvertedToStudentId);
    }
}