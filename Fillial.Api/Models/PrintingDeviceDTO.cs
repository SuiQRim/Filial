namespace PrinterFil.Api.Models
{
	public record PrintingDeviceDTO(int Id, string Name, int ConnectionType, string? MacAddress);
}
