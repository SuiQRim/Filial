using PFilial.BLL.Models;

namespace PFilial.BLL.Services.Interfaces;

public interface IEmployeesService
{
	Task<EmployeeModel []> GetEmployees();
}
