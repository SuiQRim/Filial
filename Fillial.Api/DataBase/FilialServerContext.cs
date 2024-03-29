using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace PrinterFil.Api.DataBase;

public partial class FilialServerContext : DbContext
{
    public FilialServerContext()
    {
    }

    public FilialServerContext(DbContextOptions<FilialServerContext> options)
        : base(options)
    {
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
				.WithMany()
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

		CreateData(modelBuilder);
		OnModelCreatingPartial(modelBuilder);
	}

    private void CreateData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Filial>().HasData([
            new Filial {
				Id = 1,
				Name = "Тридевятое царство"
            },
			new Filial {
				Id = 2,
				Name = "Дремучий лес"
			},
			new Filial {
				Id = 3,
				Name = "Луна"
			}
		]);

	   modelBuilder.Entity<Employee>().HasData([
            new Employee () {
                Id = 1,
                Name = "Царь",
				FilialId = 1
			},
			new Employee () {
				Id = 2,
				Name = "Яга",
				FilialId = 1
			},
			new Employee () {
				Id = 3,
				Name = "Копатыч",
				FilialId = 3
			},
			new Employee () {
				Id = 4,
				Name = "Добрыня",
				FilialId = 1
			},
			new Employee () {
				Id = 5,
				Name = "Кощей",
				FilialId = 3
			},
			new Employee () {
				Id = 6,
				Name = "Лосяш",
				FilialId = 3
			},
		]);

		modelBuilder.Entity<Printer>().HasData([
			new Printer {
				Id = 1,
				Name = "Папирус"
			},
			new Printer {
				Id = 2,
				Name = "Бумага"
			}
		]);

		modelBuilder.Entity<NetworkPrinter>().HasData([
			new NetworkPrinter {
				Id = 3,
				Name = "Камень",
				MacAddress = "abababababab"
			},
		]);

		modelBuilder.Entity<Installation>().HasData([
			new Installation {
				Id = 1,
				Name = "Дворец",
				FilialId = 1,
				Order = 1,
				DeviceId = 1,
				IsDefault = true
			},
			new Installation {
				Id = 2,
				Name = "Конюшни",
				FilialId = 1,
				Order = 2,
				DeviceId = 2,
				IsDefault = false
			},
			new Installation {
				Id = 3,
				Name = "Оружейная",
				FilialId = 1,
				Order = 3,
				DeviceId = 3,
				IsDefault = false
			},
			new Installation {
				Id = 4,
				Name = "Кратер",
				FilialId = 3,
				Order = 1,
				DeviceId = 1,
				IsDefault = true
			},
			new Installation {
				Id = 5,
				Name = "Избушка",
				FilialId = 2,
				Order = 2,
				DeviceId = 3,
				IsDefault = false
			},
			new Installation {
				Id = 6,
				Name = "Топи",
				FilialId = 2,
				Order = 1,
				DeviceId = 2,
				IsDefault = true
			}
		]);
	}

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
