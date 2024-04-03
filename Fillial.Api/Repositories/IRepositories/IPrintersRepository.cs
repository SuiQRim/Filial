using PrinterFil.Api.DataBase;

namespace PrinterFil.Api.Repositories.IRepositories;

public interface IPrintersRepository
{
	Task<IEnumerable<Printer>> ReadAsync();

	Task<IEnumerable<Printer>> ReadLocalAsync();
	Task<IEnumerable<NetworkPrinter>> ReadNetworkAsync();

	Task<bool> ExistAsync(int id);
}

