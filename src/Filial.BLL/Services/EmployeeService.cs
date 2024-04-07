using PFilial.BLL.Models;
using PFilial.BLL.Services.Interfaces;
using PFilial.DAL.Entities;
using PFilial.DAL.Repositories.Interfaces;


namespace PFilial.BLL.Services;

public class EmployeeService : IEmployeesService {


	private readonly IEmployeesRepository _employeeRepository;

	public EmployeeService(IEmployeesRepository employeeRepository)
	{
		_employeeRepository = employeeRepository;

	}
	public async Task<EmployeeModel[]> GetEmployees()
	{
		EmployeeEntity [] employees = await _employeeRepository.ReadAsync();

		return employees.Select(x => new EmployeeModel(x.Id, x.Name, x.FilialId)).ToArray();
	}
}
