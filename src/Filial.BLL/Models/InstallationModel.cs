namespace PrinterFil.Api.Models;

public record InstallationModel(
	int Id,
	string Name,
	int FilialId,
	int PrintingDeviceId,
	bool IsDefault,
	byte Order);
