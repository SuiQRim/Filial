namespace PrinterFil.Api.Models
{
	/// <summary>
	/// Сотрудник компании
	/// </summary>
	/// <param name="Id">Идентификатор</param>
	/// <param name="Name">Имя</param>
	/// <param name="FilialId">Идентификатор филиала</param>
	public record EmployeeDTO(int Id, string Name, int? FilialId);
}
