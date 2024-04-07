using Microsoft.AspNetCore.Mvc;
using PFilial.API.Responses.V1;
using PFilial.BLL.Models;
using PFilial.BLL.Models.Enums;
using PFilial.BLL.Services.Interfaces;

namespace PFilial.API.Controllers.V1;

[Route("api/v1/[controller]")]
[ApiController]
public class PrintersController : ControllerBase
{
	private readonly IPrintersService _printingDevicesService;

	public PrintersController(IPrintersService printingDevicesService)
	{
		_printingDevicesService = printingDevicesService;
	}

	[HttpGet]
	public async Task<ActionResult<PrinterResponse[]>> GetDevices([FromQuery] int? connectionType = 0)
	{
		PrinterModel[] printers = await _printingDevicesService.GetPrinters((PrinterType)(connectionType ?? 0));
		return Ok(printers
			.Select(x => new PrinterResponse(
				x.Id,
				x.Name,
				x.Type,
				x.MacAddress))
			.ToArray());
	}
}
