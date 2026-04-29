using System;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.TablesConfigs;

public static class TablePerTypeConfigs
{
    public static void ConfigureUsersTptMethod(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StaffUser>().ToTable("StaffUsers");
        modelBuilder.Entity<Student>().ToTable("Students");
        modelBuilder.Entity<Opc>().ToTable("Opcs");
        modelBuilder.Entity<Teacher>().ToTable("Teachers");
        modelBuilder.Entity<Parent>().ToTable("Parents");
    }
}
