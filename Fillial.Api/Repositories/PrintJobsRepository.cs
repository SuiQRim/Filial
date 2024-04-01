using PrinterFil.Api.DataBase;
using PrinterFil.Api.Repositories.IRepositories;
using System.Data.Common;

namespace PrinterFil.Api.Repositories;

public class PrintJobsRepository : IPrintJobsRepository
{
	public Task CreateAsync(PrintJob printJob)
	{
		throw new NotImplementedException();
	}

	public Task CreateRangeAsync(IEnumerable<PrintJob> printJobs)
	{
		throw new NotImplementedException();
	}

	public Task<Filial?> GetRunningFilialAsync(int filialId)
	{
		throw new NotImplementedException();
	}

	public Task SaveChangesAsync()
	{
		throw new NotImplementedException();
	}
}

