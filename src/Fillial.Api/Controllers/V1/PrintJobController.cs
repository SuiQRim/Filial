using Microsoft.AspNetCore.Mvc;
using PFilial.API.Requests;
using PFilial.BLL.Models;
using PFilial.BLL.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PFilial.API.Controllers.V1;

[Route("api/v1/[controller]")]
[ApiController]
public class PrintJobController : ControllerBase
{
	private readonly IPrintJobsService _printJobsService;
	public PrintJobController(IPrintJobsService printJobsService)
	{
		_printJobsService = printJobsService;

	}

	[HttpPost]
	public async Task<ActionResult> Add(PrintJobRequest printJob)
	{
		int? id = await _printJobsService.Add(new PrintJobModel(
			0,
			printJob.Name,
			printJob.EmployeeId,
			printJob.Order,
			printJob.LayerCount
		));

		if (id == null)
		{
			return BadRequest();
		}

		return Ok(id);
	}

	[HttpPost("import")]
	public async Task<ActionResult> Import([Required] IFormFile file)
	{
		// Пока так оставляю. Изучу материал и мб поменяю
		try
		{
			int count = await _printJobsService.Import(file.OpenReadStream());
			return Ok(count);
		}
		catch (Exception)
		{

			return UnprocessableEntity();
		}
	}
}
