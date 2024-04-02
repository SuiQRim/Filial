using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;

namespace PrinterFil.Api.Repositories.IRepositories;

public interface IPrintJobsRepository
{
	Task CreateAsync(PrintJob printJob);

	Task CreateRangeAsync(IEnumerable<PrintJob> printJobs);

	Task<Filial?> GetRunningFilialAsync(int filialId);
}
