namespace PFilial.BLL.Models;

public record InstallationModel(
	int Id,
	string Name,
	int FilialId,
	int PrintingDeviceId,
	bool IsDefault,
	byte Order);
