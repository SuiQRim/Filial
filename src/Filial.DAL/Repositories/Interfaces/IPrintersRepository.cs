using PFilial.DAL.Entities;

namespace PFilial.DAL.Repositories.Interfaces;

public interface IPrintersRepository
{
	Task<IEnumerable<PrinterEntity>> ReadAsync();

	Task<IEnumerable<PrinterEntity>> ReadLocalAsync();

	Task<IEnumerable<NetworkPrinterEntity>> ReadNetworkAsync();

	Task<bool> ExistAsync(int id);
}

