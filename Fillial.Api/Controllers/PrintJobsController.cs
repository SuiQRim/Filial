using Microsoft.AspNetCore.Mvc;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;
using PrinterFil.Api.Repositories.IRepositories;
using System.ComponentModel.DataAnnotations;
using PrinterFil.Api.Services;

namespace PrinterFil.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrintJobsController : ControllerBase
{
    private readonly IPrintJobsRepository _repository;
	private readonly IFilialsRepository _filialsRepository;
	private readonly IPrintingJobImporter _printingJobImporter;
	private readonly IInstallationsRepository _installationsRepository;

	public PrintJobsController(IPrintJobsRepository repository, IPrintingJobImporter printingJobImporter,
		IFilialsRepository filialsRepository, IInstallationsRepository installationsRepository)
    {
		_repository = repository;
		_printingJobImporter = printingJobImporter;
		_filialsRepository = filialsRepository;
		_installationsRepository = installationsRepository;
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
		Filial? filial = await _filialsRepository.ReadByEmployeeIdAsync(printJob.EmployeeId);
		if (filial == null)
			return NotFound("В наших филиалах не найден такой сотрудник");

		Installation? installation;
		if (printJob.InstallationOrder == null)
		{
			installation = await _installationsRepository.ReadDefaultAsync(filial.Id);
			if (installation == null)
				return NotFound($"Сотрудник не может выполнить печать из-за отсутствия инсталляции по умолчанию");
		}
		else
		{
			installation = await _installationsRepository.ReadByOrderAsync(filial.Id, (byte)printJob.InstallationOrder);
			if (installation == null)
				return NotFound($"Инсталляция с предложенным порядковым номером не найдена");
		}

		PrintJob pj = new()
		{
			EmployeeId = printJob.EmployeeId,
			Name = printJob.Name,
			Order = installation.Order,
			LayerCount = printJob.LayerCount,
			IsSuccessful = await ImitateOfPrint()
		};

		await _repository.CreateAsync(pj);

		return Ok((bool)pj.IsSuccessful ? "Успех" : "Неудача");
	}

	private static async Task<bool> ImitateOfPrint()
	{
		Random rnd = new();

		int time = rnd.Next(1000, 4000);
		await Task.Delay(time);

		return rnd.Next(2) == 1;
	}


	/// <summary>
	/// Импортирует файл, регистрируя каждую запись
	/// </summary>
	/// <param name="uploadedFile">CSV файл с записями</param>
	/// <returns>Количество выполненных печатей</returns>
	/// <response code="200">Успешное добавление</response>
	/// <response code="422">Файл не соответствует требованиям</response>
	[ProducesResponseType(typeof(int), 200)]
	[ProducesResponseType(422)]
	[HttpPost("import")]
	public async Task<ActionResult<int>> ImportPrintJobCSV([Required]IFormFile uploadedFile)
	{
		IEnumerable<PrintJobDTO> jobDTOs;
		try
		{
			jobDTOs = _printingJobImporter.Parse(uploadedFile);
		}
		catch (Exception)
		{
			return UnprocessableEntity();
		}

		IEnumerable<PrintJob> jobs = jobDTOs.Select(j => new PrintJob()
		{
			Name = j.Name,
			LayerCount = j.LayerCount,
			EmployeeId = j.EmployeeId,
			Order = j.InstallationOrder ?? 0
		});

		await _repository.CreateRangeAsync(jobs);

		return Ok(jobs.Count());
	}

}

