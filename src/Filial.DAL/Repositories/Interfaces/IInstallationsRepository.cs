using PFilial.DAL.Entities;

namespace PFilial.DAL.Repositories.Interfaces;

public interface IInstallationsRepository
{
	Task<InstallationEntity[]> ReadAsync(int? filialId);

	Task<InstallationEntity?> ReadAsync(int id);

	Task<InstallationEntity?> ReadDefaultAsync(int filialId);

	Task<InstallationEntity?> ReadFirstAsync(int filialId);

	Task<InstallationEntity?> ReadByOrderAsync(int filialId, byte order);

	Task<int?> CreateAsync(InstallationEntity installation);

	Task DeleteAsync(int id);

	Task UpdateAsync(int id, InstallationEntity installation);

	Task<byte?> GetOrderAsync(int filialId);
}
