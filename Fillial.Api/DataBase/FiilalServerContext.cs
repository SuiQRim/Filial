using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase.Entities;

namespace PrinterFil.Api.DataBase;

public partial class FiilalServerContext : DbContext
{
    public FiilalServerContext()
    {
    }

    public FiilalServerContext(DbContextOptions<FiilalServerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ConnectionType> ConnectionTypes { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Filial> Filials { get; set; }

    public virtual DbSet<Installation> Installations { get; set; }

    public virtual DbSet<PrintJob> PrintJobs { get; set; }

    public virtual DbSet<PrintingDevice> PrintingDevices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConnectionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ConnectionType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Fillial).WithMany(p => p.Employees)
                .HasForeignKey(d => d.FillialId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Employees_Fillials");
        });

        modelBuilder.Entity<Filial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Fillials");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Location).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Installation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Installation");

            entity.HasIndex(e => e.Order, "IX_Installations");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Device).WithMany(p => p.Installations)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Installations_PrintingDevices");

            entity.HasOne(d => d.Fillial).WithMany(p => p.Installations)
                .HasForeignKey(d => d.FillialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Installations_Fillials");
        });

        modelBuilder.Entity<PrintJob>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Task).HasMaxLength(2000);

            entity.HasOne(d => d.Employee).WithMany(p => p.PrintJobs)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_PrintJobs_Employees");
        });

        modelBuilder.Entity<PrintingDevice>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.MacAddress)
                .HasMaxLength(12)
                .HasDefaultValue("00000000000");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.ConnectionType).WithMany(p => p.PrintingDevices)
                .HasForeignKey(d => d.ConnectionTypeId)
                .HasConstraintName("FK_PrintingDevices_ConnectionType");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
