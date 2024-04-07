namespace PFilial.API.Responses.V1;

public record Installation(
	int Id,
	string Name,
	int FilialId,
	int DeviceId,
	bool IsDefault,
	int Order);
