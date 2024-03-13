using PrinterFil.Api.DataBase;

namespace PrinterFil.Api.Repositories.IRepositories;

public interface IEmployeesRepository
{
	/// <summary>
	/// Возвращает список сотрудников компании
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<Employee>> ReadAsync();
}

