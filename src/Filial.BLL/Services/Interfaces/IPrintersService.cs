using PFilial.BLL.Models;
using PFilial.BLL.Models.Enums;
using PrinterFil.Api.Models;

namespace PFilial.BLL.Services.Interfaces;

public interface IPrintersService
{
	Task<PrinterModel[]> GetPrinters(PrinterType type);
	Task<bool> ExistById(int id);
}
