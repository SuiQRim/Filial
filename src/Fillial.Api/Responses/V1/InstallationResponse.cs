namespace PFilial.API.Responses.V1;

public record InstallationResponse(
	int Id,
	string Name,
	int FilialId,
	int DeviceId,
	bool IsDefault,
	int Order);
