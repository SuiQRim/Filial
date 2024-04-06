namespace PFilial.DAL.Entities;

public class FilialEntity
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public string? Location { get; set; }
	public virtual ICollection<EmployeeEntity> Employees { get; set; } = [];
	public virtual ICollection<InstallationEntity> Installations { get; set; } = [];
}
