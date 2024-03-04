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
	/// Регистрирует печать
	/// </summary>
	/// <param name="printJob"></param>
	/// <returns>Результат выполнений печати</returns>
	[HttpPost]
	public async Task<ActionResult<string>> PostPrintJob(PrintJobDTO printJob) => 
		Ok((await _repository.CreateAsync(printJob)).Status.Name);
		

	/// <summary>
	/// Импортирует файл, добавляя записи в бд
	/// </summary>
	/// <param name="uploadedFile">CSV файл с записями</param>
	/// <returns>Количество выполненных печатей</returns>
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

