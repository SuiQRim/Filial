using PrinterFil.Api.DataBase;

namespace PrinterFil.Api.Repositories.IRepositories;

public interface IInstallationsRepository
{
	/// <summary>
	/// Возвращает список инсталляций
	/// </summary>
	/// <param name="filialId">Идентификатор филиала, в котором зарегистрирована инсталляция</param>
	/// <returns></returns>
	Task<IEnumerable<Installation>> ReadAsync(int? filialId);

	/// <summary>
	/// Возвращает инсталляций
	/// </summary>
	/// <param name="id">Идентификатор</param>
	/// <returns></returns>
	Task<Installation?> ReadAsync(int id);

	/// <summary>
	/// Создает инсталляцию
	/// </summary>
	/// <param name="installation">Объект инсталляции</param>
	/// <returns></returns>
	Task<byte> CreateAsync(Installation installation);

	/// <summary>
	/// Удаляет инсталляцию
	/// </summary>
	/// <param name="id">Идентификатор</param>
	/// <returns></returns>
	Task DeleteAsync(int id);

	/// <summary>
	/// Сохраняет изменения
	/// </summary>
	/// <returns></returns>
	Task SaveChangesAsync();

	/// <summary>
	/// Обновляет идентификатор инсталляции по умолчанию 
	/// </summary>
	/// <param name="installation">Филиал, который получит значение по умолчанию</param>
	/// <returns></returns>
	Task UpdateDefaultInstallationAsync(Installation installation);

	/// <summary>
	/// Обновляет инсталляцию по умолчанию из списка инсталляций внутри филиала
	/// </summary>
	/// <param name="filialId">Идентификатор филиала</param>
	/// <returns></returns>
	Task UpdateDefaultInstallationAsync(int filialId);

	/// <summary>
	/// Возвращает доступный порядочный номер
	/// </summary>
	/// <param name="filialId">Идентификатор филиала</param>
	/// <param name="order">Желаемый порядочный номер</param>
	/// <returns></returns>
	Task<byte?> GetOrderAsync(int filialId, byte? order);
}
