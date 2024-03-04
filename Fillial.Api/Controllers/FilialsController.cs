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
	/// Предоставляет список филиалов
	/// </summary>
	/// <returns>Список филиалов</returns>
	/// <response code="200">Успешное предоставление</response>
	[ProducesResponseType(typeof(IEnumerable<FilialDTO>), 200)]
	[HttpGet]
	public async Task<ActionResult<IEnumerable<FilialDTO>>> GetFilials() => 
		Ok(await _repository.ReadAsync());
	
}
