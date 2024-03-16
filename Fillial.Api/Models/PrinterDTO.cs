namespace PrinterFil.Api.Models
{
	/// <summary>
	/// Печатный девайс
	/// </summary>
	/// <param name="Id">Идентификатор</param>
	/// <param name="Name">Название</param>
	/// <param name="Type">Тип устройства</param>
	/// <param name="MacAddress">Мак адрес</param>
	public record PrinterDTO(int Id, string Name, string Type, string? MacAddress = null);
}
