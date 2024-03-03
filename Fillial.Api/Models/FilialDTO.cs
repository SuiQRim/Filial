namespace PrinterFil.Api.Models
{
	/// <summary>
	/// Филиал
	/// </summary>
	/// <param name="Id">Идентификатор</param>
	/// <param name="Location">Локация</param>
	/// <param name="Name">Имя</param>
	public record FilialDTO(int Id, string? Location, string Name);
	
}
