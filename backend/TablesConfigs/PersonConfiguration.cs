using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.TablesConfigs;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void  Configure(EntityTypeBuilder<Person> entityTypeBuilder)
    {
        entityTypeBuilder.UseTpcMappingStrategy();
    }
}
