using PFilial.BLL.Models;
using PFilial.BLL.Models.Enums;
using PFilial.BLL.Services.Interfaces;
using PFilial.DAL.Entities;
using PFilial.DAL.Repositories.Interfaces;

namespace PFilial.BLL.Services;

public class PrintersService : IPrintersService
{
	private readonly IPrintersRepository _printerService;
	public PrintersService(IPrintersRepository printersRepository)
	{
		_printerService = printersRepository;
	}

	public async Task<bool> ExistById(int id)
	{
		return await _printerService.ExistAsync(id);
	}

	public async Task<PrinterModel[]> GetPrinters(PrinterType type)
	{
		PrinterModel[] printerModels;
		
		if (type == PrinterType.Undefined)
		{
			PrinterEntity[] printers = await _printerService.ReadAsync();
			printerModels = printers.Select(x => new PrinterModel(x.Id, x.Name, x.Type, null)).ToArray();
		}

		else if (type == PrinterType.Local)
		{
			PrinterEntity[] printers = await _printerService.ReadLocalAsync();
			printerModels = printers.Select(x => new PrinterModel(x.Id, x.Name, x.Type, null)).ToArray();
		}

		else if (type == PrinterType.Network)
		{
			NetworkPrinterEntity[] printers = await _printerService.ReadNetworkAsync();
			printerModels = printers.Select(x => new PrinterModel(x.Id, x.Name, x.Type, x.MacAddress)).ToArray();
		}
		else throw new NotImplementedException();

		return printerModels;
	}
}
