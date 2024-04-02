using Microsoft.AspNetCore.Mvc;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Repositories.IRepositories;

namespace PrinterFil.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
	private readonly IEmployeesRepository _repository;

	public EmployeesController(IEmployeesRepository repository)
	{
		_repository = repository;
	}

	/// <summary>
	/// Предоставляет список всех сотрудников компании
	/// </summary>
	/// <returns>Список сотрудников компании</returns>
	/// <response code="200">Успешное предоставление</response>
	[ProducesResponseType(typeof(IEnumerable<Employee>), 200)]
	[HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
	{
		return Ok(await _repository.ReadAsync());
	}


}

