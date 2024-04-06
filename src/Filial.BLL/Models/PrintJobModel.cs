namespace PrinterFil.Api.Models;

public record PrintJobModel(
	string Name,
	int EmployeeId,
	byte? InstallationOrder,
	int LayerCount);
