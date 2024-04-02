using PrinterFil.Api.DataBase;

namespace PrinterFil.Api.Repositories.IRepositories;

public interface IPrintersRepository
{
	Task<IEnumerable<Printer>> ReadAsync();

	Task<IEnumerable<Printer>> ReadAsync<T>() where T : Printer;

	Task<bool> ExistAsync(int id);
}

