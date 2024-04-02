using PrinterFil.Api.DataBase;

namespace PrinterFil.Api.Repositories.IRepositories;

public interface IFilialsRepository
{
	Task<IEnumerable<Filial>> ReadAsync();

	Task<Filial?> ReadByEmployeeIdAsync(int employeeId);

	Task<bool> ExistAsync(int id);

}
