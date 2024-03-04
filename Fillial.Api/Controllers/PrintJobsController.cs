using Microsoft.AspNetCore.Mvc;
using PrinterFil.Api.Exceptions;
using PrinterFil.Api.Models;
using PrinterFil.Api.Repositories.IRepositories;

namespace PrinterFil.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrintJobsController : ControllerBase
{
    private readonly IPrintJobsRepository _repository;

    public PrintJobsController(IPrintJobsRepository repository)
    {
		_repository = repository;
    }

	/// <summary>
	/// Регистрирует задание печати
	/// </summary>
	/// <param name="printJob">Задание печати</param>
	/// <returns>Итоговый статус задания</returns>
	/// <response code="200">Успешное добавление</response>
	/// <response code="404">Какой-то параметр не прошел проверку на существование</response>
	[ProducesResponseType(typeof(string), 200)]
	[ProducesResponseType(404)]
	[HttpPost]
	public async Task<ActionResult<string>> PostPrintJob(PrintJobDTO printJob) => 
		Ok((await _repository.CreateAsync(printJob)).Status.Name);


	/// <summary>
	/// Импортирует файл, регистрируя каждую запись
	/// </summary>
	/// <param name="uploadedFile">CSV файл с записями</param>
	/// <returns>Количество выполненных печатей</returns>
	/// <remarks>
	/// Разделитель знак запятой
	/// </remarks>
	/// <response code="200">Успешное добавление</response>
	/// <response code="404">Какой-то параметр не прошел проверку на существование</response>
	/// <response code="422">Есть проблемы с парсингом файла</response>
	[ProducesResponseType(typeof(int), 200)]
	[ProducesResponseType(404)]
	[ProducesResponseType(422)]
	[HttpPost("import")]
	public async Task<ActionResult<int>> ImportPrintJobCSV(IFormFile uploadedFile)
	{
		try
		{
			return Ok(await _repository.ImportAsync(uploadedFile));
		}
		catch (ParsingFileException)
		{
			return UnprocessableEntity();
		}
	}
		
		
}

