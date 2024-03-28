using PrinterFil.Api.DataBase;

namespace PrinterFil.Api.Repositories.IRepositories;

public interface IPrintersRepository
{
	Task<IEnumerable<Printer>> ReadAsync();

	/// <summary>
	/// Возвращает список печатных установок
	/// </summary>
	/// <returns>Список печатных установок</returns>
	Task<IEnumerable<Printer>> ReadAsync<T>() where T : Printer;
}

