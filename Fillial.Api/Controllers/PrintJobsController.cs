using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Exceptions;
using PrinterFil.Api.Models;
using PrinterFil.Api.Repositories.IRepositories;
using System.Globalization;
using System.Text;

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
	public async Task<ActionResult<string>> PostPrintJob(PrintJobDTO printJob)
	{
		Filial? filial = await _repository.GetRunningFilialAsync(printJob.EmployeeId);
		if (filial == null)
			return NotFound("В наших филиалах не найден такой сотрудник");

		Installation? installation;
		if (printJob.InstallationOrder == null)
		{
			installation = filial.DefaultInstallation;

			if (installation == null)
				return NotFound($"Сотрудник не может выполнить печать из-за отсутствия инсталляций");
		}
		else
		{
			installation = filial.Installations
				.Where(i => i.Order == printJob.InstallationOrder)
				.FirstOrDefault();

			if (installation == null)
				return NotFound($"Инсталляция с предложенным порядковым номером не найдена");
		}

		PrintJob pj = new()
		{
			EmployeeId = printJob.EmployeeId,
			Name = printJob.Name,
			Order = installation.Order,
			LayerCount = printJob.LayerCount,
			IsSuccessful = ImitateOfPrint()
		};

		await _repository.CreateAsync(pj);
		await _repository.SaveChangesAsync();

		return Ok(pj.IsSuccessful ? "Accepted" : "Rejected");
	}


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

		return Ok();

	}

	private static bool ImitateOfPrint()
	{
		Random rnd = new();

		int time = rnd.Next(1000, 4000);
		Task.Delay(time);
		
		return rnd.Next(2) == 1;
	}

	private static PrintJobDTO[] ParsePrintJobsFromCSV(IFormFile uploadedFile)
	{
		try
		{
			PrintJobDTO[] records;

			using StreamReader streamReader = new(uploadedFile.OpenReadStream());
			CsvConfiguration csvConfig = new(CultureInfo.InvariantCulture)
			{
				Delimiter = ";",
				Encoding = Encoding.UTF8,
				HasHeaderRecord = false,
				ShouldSkipRecord = x => x.Row.Parser.Record?.Any(field => string.IsNullOrWhiteSpace(field)) ?? false
			};

			using CsvReader csvReader = new(streamReader, csvConfig);
			records = csvReader.GetRecords<PrintJobDTO>().ToArray();

			return records;
		}
		catch (HeaderValidationException)
		{
			throw new ParsingFileException("Print Jobs could not be parsed");
		}

	}
}

