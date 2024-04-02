using PrinterFil.Api.DataBase;

namespace PrinterFil.Api.Repositories.IRepositories;

public interface IInstallationsRepository
{
	Task<IEnumerable<Installation>> ReadAsync(int? filialId);

	Task<Installation?> ReadAsync(int id);

	Task<Installation?> ReadDefaultAsync(int filialId);

	Task<Installation?> ReadFirstAsync(int filialId);

	Task<int?> CreateAsync(Installation installation);

	Task DeleteAsync(int id);

	Task UpdateAsync(int id, Installation installation);

	Task<byte?> GetOrderAsync(int filialId);

	Task<bool> Exist(int? filialId = null, byte? order = null);
}
