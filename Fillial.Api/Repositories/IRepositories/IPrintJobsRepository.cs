using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;

namespace PrinterFil.Api.Repositories.IRepositories
{
	public interface IPrintJobsRepository
	{
		Task<PrintJob> CreateAsync(PrintJobDTO printJobDTO);
		Task<int> ImportAsync(IFormFile uploadedFile);
	}
}
