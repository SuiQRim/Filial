using PrinterFil.Api.Models;

namespace PrinterFil.Api.Services
{
	public interface IPrintingJobImporter
	{
		public Task<IEnumerable<PrintJobDTO>> ParseAsync(IFormFile file);
	}
}
