using PFilial.DAL.Entities;
using PFilial.BLL.Models;

namespace PFilial.BLL.Mappers;

public static class InstallationMapper
{
	public static InstallationModel EntityToModel(InstallationEntity entity)
	{
		return new InstallationModel(
			entity.Id,
			entity.Name,
			entity.FilialId,
			entity.DeviceId,
			entity.IsDefault,
			entity.Order);
	}

	public static InstallationEntity ModelToEntity(InstallationModel entity)
	{
		return new InstallationEntity
		{
			Id = entity.Id,
			Name = entity.Name,
			FilialId = entity.FilialId,
			DeviceId = entity.PrintingDeviceId,
			IsDefault = entity.IsDefault,
			Order = entity.Order
		};
	}
}
