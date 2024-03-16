using Microsoft.AspNetCore.Mvc;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;
using PrinterFil.Api.Repositories.IRepositories;

namespace PrinterFil.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrintersController : ControllerBase
{
	private readonly IPrintersRepository _repository;

	public PrintersController(IPrintersRepository repository)
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
	[ProducesResponseType(typeof(IEnumerable<PrinterDTO>), 200)]
	[ProducesResponseType(404)]
	[HttpGet]
	public async Task<ActionResult<IEnumerable<PrinterDTO>>> GetPrinters(int? connectionType)
	{
		IEnumerable<Printer>? printers = connectionType switch
		{
			null => await _repository.ReadAsync<Printer>(),
			1 => await _repository.ReadAsync<LocalPrinter>(),
			2 => await _repository.ReadAsync<NetworkPrinter>(),
			_ => null
		};

		if (printers == null)
			return NotFound("Принтер с таким типом подключения не найден");
		
		return Ok(printers.Select(CreatePrinterDTO));
	}

	private static PrinterDTO CreatePrinterDTO(Printer printer) =>
		printer switch
		{
			NetworkPrinter p => new PrinterDTO(p.Id, p.Name, "Network", p.MacAddress),
			LocalPrinter p => new PrinterDTO(p.Id, p.Name, "Local"),
			_ => throw new InvalidOperationException("Unknown printer type")
		};
}



