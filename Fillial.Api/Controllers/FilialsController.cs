using Microsoft.AspNetCore.Mvc;
using PrinterFil.Api.Models;
using PrinterFil.Api.Repositories.IRepositories;

namespace PrinterFil.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FilialsController : ControllerBase
{
	private readonly IFilialsRepository _repository;

	public FilialsController(IFilialsRepository repository)
	{
		_repository = repository;
	}

	/// <summary>
	/// Возвращает список всех филиалов
	/// </summary>
	/// <returns></returns>
	[HttpGet]
	public async Task<ActionResult<IEnumerable<FilialDTO>>> GetFilials() => 
		Ok(await _repository.ReadAsync());
	
}
