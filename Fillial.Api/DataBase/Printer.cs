using System.ComponentModel.DataAnnotations;

namespace PrinterFil.Api.DataBase;

public class Printer
{
	[Key]
	public int Id { get; set; }

	[StringLength(20)]
	public required string Name { get; set; }
}
