using Microsoft.AspNetCore.Mvc;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;
using PrinterFil.Api.Repositories.IRepositories;

namespace PrinterFil.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrintingDevicesController : ControllerBase
{
    private readonly IPrintingDevicesRepository _repository;

    public PrintingDevicesController(IPrintingDevicesRepository repository)
    {
        _repository = repository;
    }

	/// <summary>
	/// Предоставляет список печатных устройств
	/// </summary>
	/// <param name="connectionType">Тип подключения</param>
	/// <returns>Список печатных устройств</returns>
	/// <response code="200">Успешное предоставление</response>
	/// <response code="404">Какой-то параметр не прошел проверку на существование</response>
	[ProducesResponseType(typeof(IEnumerable<PrintingDeviceDTO>), 200)]
	[ProducesResponseType(404)]
	[HttpGet]
    public async Task<ActionResult<IEnumerable<PrintingDeviceDTO>>> GetPrintingDevices([FromQuery] int? connectionType) =>
        Ok(await _repository.ReadAsync(connectionType));
        

}

