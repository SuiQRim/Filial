﻿using Microsoft.AspNetCore.Mvc;
using PFilial.API.Responses.V1;
using PFilial.BLL.Models;
using PFilial.BLL.Services.Interfaces;

namespace PFilial.API.Controllers.V1;

[Route("api/v1/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
	private readonly IEmployeesService _employeeService;

	public EmployeesController(IEmployeesService employeeService)
	{
		_employeeService = employeeService;
	}

	[HttpGet]
	public async Task<ActionResult<EmployeeResponse[]>> GetEmployees()
	{
		EmployeeModel[] employees = await _employeeService.GetEmployees();

		return Ok(employees
			.Select(x => new EmployeeResponse(
				x.Id,
				x.Name,
				x.FilialId))
			.ToArray());
	}
}

