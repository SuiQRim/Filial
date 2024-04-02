using PrinterFil.Api.DataBase;

namespace PrinterFil.Api.Repositories.IRepositories;

public interface IPrintersRepository
{
	Task<IEnumerable<Printer>> ReadAsync();

	//TODO: Возможно избавится от дженерика и использовать разные методы для разных типов
	Task<IEnumerable<Printer>> ReadAsync<T>() where T : Printer;

	Task<bool> ExistAsync(int id);
}

