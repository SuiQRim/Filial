using PrinterFil.Api.Models;

namespace PrinterFil.Api.Repositories.IRepositories;

public interface IEmployeesRepository
{
	/// <summary>
	/// Возвращает список сотрудников компании
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<EmployeeDTO>> ReadAsync();
}

