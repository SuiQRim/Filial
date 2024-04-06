using PrinterFil.Api.Models;

namespace PFilial.BLL.Services.Interfaces;

public interface IFilialsService
{
	Task<FilialModel[]> GetFilials();
}
