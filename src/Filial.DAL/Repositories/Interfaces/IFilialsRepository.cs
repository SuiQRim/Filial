using PFilial.DAL.Entities;

namespace PFilial.DAL.Repositories.Interfaces;

public interface IFilialsRepository
{
	Task<FilialEntity[]> ReadAsync();

	Task<FilialEntity?> ReadByEmployeeIdAsync(int employeeId);

	Task<bool> ExistAsync(int id);

}
