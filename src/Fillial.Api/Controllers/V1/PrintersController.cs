using Microsoft.AspNetCore.Mvc;
using PFilial.API.Responses.V1;
using PFilial.BLL.Models.Enums;
using PFilial.BLL.Services.Interfaces;
using PrinterFil.Api.Models;

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
	public async Task<ActionResult<GetDevicesResponse>> GetDevices([FromQuery] int? connectionType = 0)
	{
		PrinterModel [] printers = await _printingDevicesService.GetPrinters((PrinterType)(connectionType ?? 0));
		return Ok(new GetDevicesResponse(
			printers
			.Select(x => new Printer(
				x.Id,
				x.Name,
				x.Type,
				x.MacAddress))
			.ToArray()));
	}
}
