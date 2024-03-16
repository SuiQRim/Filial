using Microsoft.EntityFrameworkCore;

namespace PrinterFil.Api.DataBase;

public partial class FilialServerContext : DbContext
{
    public FilialServerContext()
    {
    }

    public FilialServerContext(DbContextOptions<FilialServerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Filial> Filials { get; set; }

    public virtual DbSet<Installation> Installations { get; set; }

    public virtual DbSet<PrintJob> PrintJobs { get; set; }

    public virtual DbSet<Printer> Printers { get; set; }

    public virtual DbSet<NetworkPrinter> NetworkPrinters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Printer>().UseTphMappingStrategy()
            .HasDiscriminator<string>("Type")
            .HasValue<LocalPrinter>("Local")
            .HasValue<NetworkPrinter>("Network");

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Filial).WithMany(p => p.Employees)
                .HasForeignKey(d => d.FilialId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Employees_Fillials");
        });

        modelBuilder.Entity<Filial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Fillials");

            entity.Property(e => e.Location).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.DefaultInstallation).WithMany(p => p.Filials)
                .HasForeignKey(d => d.DefaultInstallationId)
                .HasConstraintName("FK_Filials_Installations");
        });

        modelBuilder.Entity<Installation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Installation");

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Device).WithMany(p => p.Installations)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Installations_PrintingDevices");

            entity.HasOne(d => d.Filial).WithMany(p => p.Installations)
                .HasForeignKey(d => d.FilialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Installations_Fillials");
        });

        modelBuilder.Entity<PrintJob>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(2000);

            entity.HasOne(d => d.Employee).WithMany(p => p.PrintJobs)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_PrintJobs_Employees");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
