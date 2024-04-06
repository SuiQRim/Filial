namespace PFilial.DAL.Entities;

public class PrintJobEntity
{
	public int Id { get; set; }

	public required string Name { get; set; }

	public int EmployeeId { get; set; }

	public byte Order { get; set; }

	public int LayerCount { get; set; }

	public bool? IsSuccessful { get; set; }
}
