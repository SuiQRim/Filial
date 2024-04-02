using PrinterFil.Api.DataBase;
namespace PrinterFil.Api.Repositories.IRepositories;

public interface IPrintJobsRepository
{
	Task<int?> CreateAsync(PrintJob printJob);

	Task CreateRangeAsync(IEnumerable<PrintJob> printJobs);
}
