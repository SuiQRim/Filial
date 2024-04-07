using PFilial.BLL.Models;

namespace PFilial.BLL.Services.Interfaces;

public interface IPrintJobImporter
{
	public Task<PrintJobDTO[]> ParseAsync(Stream stream);
}
