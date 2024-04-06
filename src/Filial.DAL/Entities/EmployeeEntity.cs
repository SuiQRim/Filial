namespace PFilial.DAL.Entities;

public class EmployeeEntity
{
	public int Id { get; set; }

	public required string Name { get; set; }

	public int FilialId { get; set; }
}
