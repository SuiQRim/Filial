namespace PFilial.API.Requests;

public record AddInstallationRequest(string Name, int FilialId, int DeviceId, bool IsDefault, byte? Order);