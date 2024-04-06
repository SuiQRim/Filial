using PFilial.DAL.Entities;

namespace PFilial.DAL.Repositories.Interfaces;

public interface IPrintJobsRepository
{
	Task<int?> CreateAsync(PrintJobEntity printJob);

	Task<int> CreateRangeAsync(IEnumerable<PrintJobEntity> printJobs);
}
