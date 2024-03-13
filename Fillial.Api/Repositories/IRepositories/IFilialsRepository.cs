using PrinterFil.Api.DataBase;

namespace PrinterFil.Api.Repositories.IRepositories;

public interface IFilialsRepository
{
	/// <summary>
	/// Возвращает список филиалов
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<Filial>> ReadAsync();
}
