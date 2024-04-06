namespace PrinterFil.Api.Models;

public record InstallationModel(
	string Name,
	int FilialId,
	int PrintingDeviceId,
	bool IsDefault,
	byte? Order);
