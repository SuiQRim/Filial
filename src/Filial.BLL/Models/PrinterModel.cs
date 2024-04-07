namespace PrinterFil.Api.Models;

public record PrinterModel(
	int Id,
	string Name,
	string Type,
	string? MacAddress);
