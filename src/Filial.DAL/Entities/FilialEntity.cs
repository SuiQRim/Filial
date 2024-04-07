namespace PFilial.DAL.Entities;

public record FilialEntity
{
    public required int Id { get; init; }

	public required string Name { get; init; }

	public string? Location { get; init; }
}
