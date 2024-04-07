using PFilial.BLL.Mappers;
using PFilial.DAL.Entities;
using PFilial.DAL.Repositories.Interfaces;
using PFilial.BLL.Models;

namespace PFilial.BLL.Services.Interfaces;

public class InstallationsService : IInstallationsService
{
	private readonly IInstallationsRepository _installationsRepository;
    public InstallationsService(IInstallationsRepository installationsRepository)
    {
		_installationsRepository = installationsRepository;
	}

	public async Task<int?> AddAsync(InstallationModel installation)
	{
		InstallationEntity entity = InstallationMapper.ModelToEntity(installation);
		return await _installationsRepository.CreateAsync(entity);
	}

	public async Task<bool> DefaultExistAsync(int filialId)
	{
		return await GetDefaultAsync(filialId) != null;
	}

	public async Task<bool> ExistByOrderAsync(int filialId, byte order)
	{
		return await GetByOrderAsync(filialId, order) != null;
	}

	public async Task<InstallationModel?> GetAsync(int id)
	{
		InstallationEntity? instal = await _installationsRepository.ReadAsync(id);

		if (instal == null)
		{
			return null;
		}

		return InstallationMapper.EntityToModel(instal);
	}

	public async Task<InstallationModel?> GetByOrderAsync(int filialId, byte order)
	{
		InstallationEntity? instal = await _installationsRepository.ReadByOrderAsync(filialId, order);

		if (instal == null)
		{
			return null;
		}

		return InstallationMapper.EntityToModel(instal);
	}

	public async Task<InstallationModel[]> GetCollectionAsync(int? filialId)
	{
		InstallationEntity[] entities = await _installationsRepository.ReadAsync(filialId);
		return entities.Select(InstallationMapper.EntityToModel).ToArray();
	}

	public async Task<InstallationModel?> GetDefaultAsync(int filialId)
	{
		InstallationEntity? instal = await _installationsRepository.ReadDefaultAsync(filialId);

		if (instal == null)
		{
			return null;
		}

		return InstallationMapper.EntityToModel(instal);
	}

	public async Task<InstallationModel?> GetFirstAsync(int filialId)
	{
		InstallationEntity? instal = await _installationsRepository.ReadFirstAsync(filialId);

		if (instal == null)
		{
			return null;
		}

		return InstallationMapper.EntityToModel(instal);
	}

	public async Task<byte?> GetOrderAsync(int filialId)
	{
		return await _installationsRepository.GetOrderAsync(filialId);
	}

	public async Task RemoveAsync(int id)
	{
		await _installationsRepository.DeleteAsync(id);
	}

	public async Task UpdateAsync(int id, InstallationModel installation)
	{
		await _installationsRepository.UpdateAsync(id, InstallationMapper.ModelToEntity(installation));
	}
}
