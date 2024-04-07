namespace PFilial.DAL.Entities;

public record InstallationEntity
{
	public required int Id { get; init; }

	public required string Name { get; init; }

	public required int FilialId { get; init; }

	public required int DeviceId { get; init; }

	public required bool IsDefault { get; init; }

	public required byte Order { get; init; }
}
