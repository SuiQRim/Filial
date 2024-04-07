namespace PFilial.DAL.Entities;

public record PrintJobEntity
{
	public required int Id { get; init; }

	public required string Name { get; init; }

	public required int EmployeeId { get; init; }

	public required byte Order { get; init; }

	public required int LayerCount { get; init; }

	public bool? IsSuccessful { get; init; }
}
