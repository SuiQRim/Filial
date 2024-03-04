using PrinterFil.Api.Models;

namespace PrinterFil.Api.Repositories.IRepositories;

public interface IPrintingDevicesRepository
{
	/// <summary>
	/// Возвращает список печатных установок
	/// </summary>
	/// <param name="connectionType">Тип подключения</param>
	/// <returns>Список печатных установок</returns>
	Task<IEnumerable<PrintingDeviceDTO>> ReadAsync(int? connectionType);
}

