using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;
using PrinterFil.Api.Repositories.IRepositories;
using System.Globalization;
using System.Text;
using CsvHelper.TypeConversion;
using System.ComponentModel.DataAnnotations;

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

		return Ok((bool)pj.IsSuccessful ? "Accepted" : "Rejected");
	}


	/// <summary>
	/// Импортирует файл, регистрируя каждую запись
	/// </summary>
	/// <param name="uploadedFile">CSV файл с записями</param>
	/// <returns>Количество выполненных печатей</returns>
	/// <response code="200">Успешное добавление</response>
	/// <response code="404">Какой-то параметр не прошел проверку на существование</response>
	/// <response code="422">Файл не валидный</response>
	[ProducesResponseType(typeof(int), 200)]
	[ProducesResponseType(404)]
	[ProducesResponseType(422)]
	[HttpPost("import")]
	public async Task<ActionResult<int>> ImportPrintJobCSV([Required]IFormFile uploadedFile)
	{
		string[] lines;
		using (var reader = new StreamReader(uploadedFile.OpenReadStream()))
		{
			lines = ReadFirstLines(reader, 100);
		}

		List<PrintJobDTO> printJobDTOs = new(lines.Length);

		using (var reader = new StringReader(string.Join(Environment.NewLine, lines)))
		using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) 
		{
			HasHeaderRecord = false,
			Delimiter = ";",
			Encoding = Encoding.UTF8
		}))
		{
			while (csv.Read())
			{
				try
				{

					PrintJobDTO pj = new(
						csv.GetField<string>(0),
						csv.GetField<int>(1),
						csv.GetField<byte>(2),
						csv.GetField<int>(3)
					);

					printJobDTOs.Add(pj);
				}
				catch (CsvHelperException ex) 
				when (ex is CsvHelper.ValidationException || ex is TypeConverterException || ex is CsvHelper.MissingFieldException)
				{
					continue;
				}
				catch (CsvHelperException)
				{
					return UnprocessableEntity();
				}

			}
		}

		IEnumerable<PrintJob> jobs = printJobDTOs.Select(j => new PrintJob()
		{
			Name = j.Name,
			LayerCount = j.LayerCount,
			EmployeeId = j.EmployeeId,
			Order = (byte)j.InstallationOrder!,
			IsSuccessful = true
		});

		await _repository.CreateRangeAsync(jobs);
		await _repository.SaveChangesAsync();

		return Ok(jobs.Count());
	}

	private static bool ImitateOfPrint()
	{
		Random rnd = new();

		int time = rnd.Next(1000, 4000);
		Task.Delay(time);
		
		return rnd.Next(2) == 1;
	}

	private string[] ReadFirstLines(StreamReader reader, int maxLines)
	{
		List<string> lines = new (maxLines);
		for (int i = 0; i < maxLines; i++)
		{
			var line = reader.ReadLine();

			if (line == null)
				break;
			
			lines.Add(line);
		}
		return lines.ToArray();
	}

}

