using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace PrinterFil.Api.Models
{
	/// <summary>
	/// Задание печати
	/// </summary>
	/// <param name="Name">Название</param>
	/// <param name="EmployeeId">Идентификатор сотрудника, выполняющий печать</param>
	/// <param name="InstallationOrder">Порядковый номер инсталляции</param>
	/// <param name="LayerCount">Количество страниц</param>
	public record PrintJobDTO (
		string Name,
		int EmployeeId,
		byte? InstallationOrder,
		[Range(1, int.MaxValue, ErrorMessage = "Enter a value bigger than {0}")] int LayerCount
	);

}
