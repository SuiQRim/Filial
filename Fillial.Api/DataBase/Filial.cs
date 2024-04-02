using System.ComponentModel.DataAnnotations;

namespace PrinterFil.Api.DataBase;

public partial class Filial
{
	[Key]
    public int Id { get; set; }

	[StringLength(20)]
	public required string Name { get; set; }

	[StringLength(20)]
	public string? Location { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = [];

    public virtual ICollection<Installation> Installations { get; set; } = [];
}
