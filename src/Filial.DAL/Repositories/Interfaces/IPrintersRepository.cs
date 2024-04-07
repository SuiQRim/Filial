using PFilial.DAL.Entities;

namespace PFilial.DAL.Repositories.Interfaces;

public interface IPrintersRepository
{
	Task<PrinterEntity[]> ReadAsync();

	Task<PrinterEntity[]> ReadLocalAsync();

	Task<NetworkPrinterEntity[]> ReadNetworkAsync();

	Task<bool> ExistAsync(int id);
}

