using PrinterFil.Api.Models;

namespace PrinterFil.Api.Repositories.IRepositories;


public interface IInstallationsRepository
{
	/// <summary>
	/// Возвращает список инсталяций
	/// </summary>
	/// <param name="filialId">Идентификатор филиала, в котором зарегестрирована инсталяция</param>
	/// <returns></returns>
	Task<IEnumerable<InstallationResponseDTO>> ReadAsync(int? filialId);

	/// <summary>
	/// Возвращает инсталяцию
	/// </summary>
	/// <param name="id">Идентификатор</param>
	/// <returns></returns>
	Task<InstallationResponseDTO> ReadAsync(int id);

	/// <summary>
	/// Создает инсталяцию
	/// </summary>
	/// <param name="installation">Объект инсталяции</param>
	/// <returns></returns>
	Task<int> CreateAsync(InstallationDTO installation);

	/// <summary>
	/// Удаляет инсталяцию
	/// </summary>
	/// <param name="id">Идентификатор</param>
	/// <returns></returns>
	Task DeleteAsync(int id);
}
