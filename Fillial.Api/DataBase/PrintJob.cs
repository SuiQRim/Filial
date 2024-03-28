using System.ComponentModel.DataAnnotations;

namespace PrinterFil.Api.DataBase;

public partial class PrintJob
{
	[Key]
	public int Id { get; set; }

	[StringLength(2000)]
	public required string Name { get; set; }

    public int EmployeeId { get; set; }

    public byte Order { get; set; }

    public int LayerCount { get; set; }

    public bool? IsSuccessful { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
