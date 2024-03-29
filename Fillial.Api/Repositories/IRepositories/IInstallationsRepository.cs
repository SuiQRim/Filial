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
	/// Возвращает инсталляцию
	/// </summary>
	/// <param name="id">Идентификатор</param>
	/// <returns></returns>
	Task<Installation?> ReadAsync(int id);

	/// <summary>
	/// Возвращает инсталляцию по умолчанию
	/// </summary>
	/// <param name="filialId">Идентификатор филиала</param>
	/// <returns></returns>
	Task<Installation?> ReadDefaultAsync(int filialId);

	/// <summary>
	/// Возвращает первую инсталляцию
	/// </summary>
	/// <param name="filialId">Идентификатор филиала</param>
	/// <returns></returns>
	Task<Installation?> ReadFirstAsync(int filialId);

	/// <summary>
	/// Создает инсталляцию
	/// </summary>
	/// <param name="installation">Объект инсталляции</param>
	/// <returns></returns>
	Task CreateAsync(Installation installation);

	/// <summary>
	/// Удаляет инсталляцию
	/// </summary>
	/// <param name="id">Идентификатор</param>
	/// <returns></returns>
	Task DeleteAsync(int id);

	/// <summary>
	/// Возвращает доступный порядочный номер
	/// </summary>
	/// <param name="filialId">Идентификатор филиала</param>
	/// <returns></returns>
	Task<byte> GetOrderAsync(int filialId);

	/// <summary>
	/// Существует ли запись с определенными данными
	/// </summary>
	/// <param name="filialId">Идентификатор филиала</param>
	/// <param name="order">Порядковый номер</param>
	/// <returns></returns>
	Task<bool> Exist(int filialId, byte order);

	Task<bool> AnyInFilial(int filialId);

	/// <summary>
	/// Сохраняет изменения
	/// </summary>
	/// <returns></returns>
	Task SaveChangesAsync();
}
