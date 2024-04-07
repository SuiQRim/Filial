using PFilial.DAL.Entities;
using PrinterFil.Api.Models;

namespace PFilial.BLL.Services.Interfaces;

public interface IInstallationsService
{
	Task<InstallationModel?> GetAsync(int id);
	Task<InstallationModel[]> GetCollectionAsync(int? filialId);

	Task<InstallationModel?> GetDefaultAsync(int filialId);

	Task<InstallationModel?> GetFirstAsync(int filialId);

	Task<InstallationModel?> GetByOrderAsync(int filialId, byte order);

	Task<int?> AddAsync(InstallationModel installation);

	Task RemoveAsync(int id);

	Task UpdateAsync(int id, InstallationModel installation);

	Task<byte?> GetOrderAsync(int filialId);

	Task<bool> DefaultExistAsync(int filialId);

	Task<bool> ExistByOrderAsync(int filialId, byte order);
}
