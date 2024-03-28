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
        Database.EnsureDeleted();
		Database.EnsureCreated();
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
            .HasValue<Printer>("Local")
			.HasValue<NetworkPrinter>("Network");

		modelBuilder.Entity<Installation>(entity =>
        {
			entity.HasOne(e => e.Device)
				.WithOne()
				.IsRequired();
		});

		modelBuilder.Entity<Filial>(entity =>
		{
			entity.HasMany(e => e.Installations)
				.WithOne()
				.HasForeignKey(e => e.FilialId)
				.IsRequired();

			entity.HasMany(e => e.Employees)
				.WithOne()
				.HasForeignKey(e => e.FilialId)
				.IsRequired();
		});

		modelBuilder.Entity<PrintJob>(entity =>
        {
            entity.HasOne(d => d.Employee)
                .WithMany()
                .HasForeignKey(d => d.EmployeeId)
                .IsRequired();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
