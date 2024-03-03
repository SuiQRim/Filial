namespace PrinterFil.Api.Models
{
	/// <summary>
	/// Печатный девайс
	/// </summary>
	/// <param name="Id">Идентификатор</param>
	/// <param name="Name">Название</param>
	/// <param name="ConnectionType">Тип подключения</param>
	/// <param name="MacAddress">Мак адрес</param>
	public record PrintingDeviceDTO(int Id, string Name, int ConnectionType, string? MacAddress);
}
