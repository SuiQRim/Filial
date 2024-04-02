using PrinterFil.Api.DataBase;

namespace PrinterFil.Api.Repositories.IRepositories;

public interface IEmployeesRepository
{
	Task<IEnumerable<Employee>> ReadAsync();
}

