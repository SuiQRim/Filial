using System.ComponentModel.DataAnnotations;

namespace PrinterFil.Api.DataBase;

public partial class Installation
{
	[Key]
	public int Id { get; set; }

	[StringLength(20)]
	public required string Name { get; set; }

	public int FilialId { get; set; }

	public int DeviceId { get; set; }

    public bool IsDefault { get; set; }

	[Range(1, byte.MaxValue)]
	public byte Order { get; set; }
}
