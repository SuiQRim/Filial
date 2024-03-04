using Microsoft.AspNetCore.Mvc;
using PrinterFil.Api.Models;
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
	/// Возвращает список всех сотрудников компании
	/// </summary>
	/// <returns></returns>
	[HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetEmployees() =>
		Ok(await _repository.ReadAsync());

}

