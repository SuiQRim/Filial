namespace PFilial.DAL.Entities;

public record EmployeeEntity
{
    public required int Id { get; init; }

	public required string Name { get; init; }

	public required int FilialId { get; init; }
}
