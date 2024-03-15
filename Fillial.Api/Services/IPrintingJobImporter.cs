using PrinterFil.Api.Models;

namespace PrinterFil.Api.Services
{
	public interface IPrintingJobImporter
	{
		public IEnumerable<PrintJobDTO> Parse(IFormFile file);
	}
}
