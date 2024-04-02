using PrinterFil.Api.DataBase;

namespace PrinterFil.Api.Repositories.IRepositories;

public interface IFilialsRepository
{
	Task<IEnumerable<Filial>> ReadAsync();

	Task<bool> ExistAsync(int id);

}
