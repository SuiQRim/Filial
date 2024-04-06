using PFilial.DAL.Entities;

namespace PFilial.DAL.Repositories.Interfaces;

public interface IEmployeesRepository
{
	Task<EmployeeEntity[]> ReadAsync();
}

