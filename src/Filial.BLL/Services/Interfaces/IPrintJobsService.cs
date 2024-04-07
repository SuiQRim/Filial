using PFilial.BLL.Models;

namespace PFilial.BLL.Services.Interfaces;

public interface IPrintJobsService
{
	Task<int?> Add(PrintJobModel printJob);

	Task<int> Import(Stream stream);
}
