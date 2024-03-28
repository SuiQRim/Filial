using System.ComponentModel.DataAnnotations;

namespace PrinterFil.Api.DataBase;

public partial class Employee
{
	[Key]
	public int Id { get; set; }

    [StringLength(20)]
    public required string Name { get; set; }

    public int? FilialId { get; set; }
}
