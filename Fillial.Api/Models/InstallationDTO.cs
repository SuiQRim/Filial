namespace PrinterFil.Api.Models
{
	public record InstallationDTO(string Name, int FilialId, int PrintingDeviceId, bool IsDefault, int? Order);
}
