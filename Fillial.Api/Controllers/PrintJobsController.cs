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
	private readonly IPrintingJobImporter _printingJobImporter;

	public PrintJobsController(IPrintJobsRepository repository, IPrintingJobImporter printingJobImporter)
    {
		_repository = repository;
		_printingJobImporter = printingJobImporter;
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
		//Filial? filial = await _repository.GetRunningFilialAsync(printJob.EmployeeId);
		//if (filial == null)
		//	return NotFound("В наших филиалах не найден такой сотрудник");

		//Installation? installation;
		//if (printJob.InstallationOrder == null)
		//{
		//	installation = filial.DefaultInstallation;

		//	if (installation == null)
		//		return NotFound($"Сотрудник не может выполнить печать из-за отсутствия инсталляций");
		//}
		//else
		//{
		//	installation = filial.Installations
		//		.Where(i => i.Order == printJob.InstallationOrder)
		//		.FirstOrDefault();

		//	if (installation == null)
		//		return NotFound($"Инсталляция с предложенным порядковым номером не найдена");
		//}

		//PrintJob pj = new()
		//{
		//	EmployeeId = printJob.EmployeeId,
		//	Name = printJob.Name,
		//	Order = installation.Order,
		//	LayerCount = printJob.LayerCount,
		//	IsSuccessful = await ImitateOfPrint()
		//};

		//await _repository.CreateAsync(pj);
		//await _repository.SaveChangesAsync();

		//return Ok((bool)pj.IsSuccessful ? "Accepted" : "Rejected");
		return Ok();
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
	/// <response code="404">Какой-то параметр не прошел проверку на существование</response>
	/// <response code="422">Не удалось считать файл</response>
	[ProducesResponseType(typeof(int), 200)]
	[ProducesResponseType(404)]
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
			Order = (byte)j.InstallationOrder!
		});

		await _repository.CreateRangeAsync(jobs);
		await _repository.SaveChangesAsync();

		return Ok(jobs.Count());
	}

}

