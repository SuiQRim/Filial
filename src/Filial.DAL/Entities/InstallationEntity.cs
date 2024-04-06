namespace PFilial.DAL.Entities;

public class InstallationEntity
{
	public int Id { get; set; }

	public required string Name { get; set; }

	public int FilialId { get; set; }

	public int DeviceId { get; set; }

	public bool IsDefault { get; set; }

	public byte Order { get; set; }
}
