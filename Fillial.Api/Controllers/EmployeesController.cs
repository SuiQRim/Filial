using Microsoft.AspNetCore.Mvc;
using PrinterFil.Api.DataBase;
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
	/// Предоставляет список всех сотрудников компании
	/// </summary>
	/// <returns>Список сотрудников компании</returns>
	/// <response code="200">Успешное предоставление</response>
	[ProducesResponseType(typeof(IEnumerable<EmployeeDTO>), 200)]
	[HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetEmployees()
	{
		IEnumerable<Employee> employees = await _repository.ReadAsync();

		IEnumerable<EmployeeDTO> EmployeeDTOs =
			employees.Select(x => new EmployeeDTO(x.Id, x.Name, x.FilialId));

		return Ok(EmployeeDTOs);
	}


}

