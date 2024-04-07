using PFilial.BLL.Models;
using PFilial.BLL.Models.Enums;

namespace PFilial.BLL.Services.Interfaces;

public interface IPrintersService
{
	Task<PrinterModel[]> GetPrinters(PrinterType type);
	Task<bool> ExistById(int id);
}
