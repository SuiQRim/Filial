using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Exceptions;
using PrinterFil.Api.Models;
using PrinterFil.Api.Repositories.IRepositories;

namespace PrinterFil.Api.Repositories;

public class InstallationsRepository : IInstallationsRepository
{
	private readonly FilialServerContext _context;

	public InstallationsRepository(FilialServerContext context)
	{
		_context = context;
	}

	/// <inheritdoc/>
	public async Task<IEnumerable<InstallationResponseDTO>> ReadAsync(int? filialId)
	{
		if (filialId != null && await _context.Filials.FindAsync(filialId) == null)
			throw new EntityNotFoundExceptions($"Filial with id [{filialId}] is not found");

		return await _context
			.Installations
			.Where(i => filialId == null || i.FillialId == filialId)
			.Include(i => i.Filials)
			.Select(i => new InstallationResponseDTO(
				i.Id, i.Name, i.FillialId, i.DeviceId,
				i.Filials.Single(f => f.Id == i.FillialId).DefaultInstallationId == i.Id,
				i.Order))
			.ToArrayAsync();
	}

	/// <inheritdoc/>
	public async Task<InstallationResponseDTO> ReadAsync(int id)
	{
		Installation? installation = await _context
			.Installations
			.Include(i => i.Fillial)
			.SingleOrDefaultAsync(x => x.Id == id)
			?? throw new EntityNotFoundExceptions(typeof(Installation), id.ToString());

		Installation i = installation;
		return new InstallationResponseDTO(
			i.Id, i.Name, i.FillialId, i.DeviceId,
			i.Fillial.DefaultInstallationId == i.Id,
			i.Order);
	}

	/// <inheritdoc/>

	public async Task<int> CreateAsync(InstallationDTO installation)
	{
		Filial? filial = await _context.Filials.FindAsync(installation.FilialId) ??
			throw new EntityNotFoundExceptions(typeof(Filial), installation.FilialId.ToString());

		if (!await _context.PrintingDevices.AnyAsync(p => p.Id == installation.PrintingDeviceId)) 
			throw new EntityNotFoundExceptions(typeof(PrintingDevice), installation.PrintingDeviceId.ToString());

		byte? order = await GetOrder(installation) ?? throw new BadHttpRequestException($"Order is busy");

		Installation newInstallation = new()
		{
			Name = installation.Name,
			DeviceId = installation.PrintingDeviceId,
			FillialId = installation.PrintingDeviceId,
			Order = (byte)order
		};

		if (installation.IsDefault)
		{
			filial.DefaultInstallation = newInstallation;
		}
		else
		{
			filial.Installations.Add(newInstallation);
		}

		await _context.SaveChangesAsync();

		return newInstallation.Id;
	}

	/// <inheritdoc/>
	public async Task DeleteAsync(int id)
	{
		Installation? installation = await _context.Installations.Include(i => i.Filials).SingleOrDefaultAsync(i => i.Id == id)
			?? throw new EntityNotFoundExceptions(typeof(Installation), id.ToString());

		Filial? filial = await _context.Filials.FindAsync(installation.FillialId)
			?? throw new EntityNotFoundExceptions(typeof(Filial), installation.FillialId.ToString());

		if (filial.DefaultInstallationId == installation.Id)
		{
			Installation? newDefaultInstallation =
				await _context.Installations
					.Where(i => i.FillialId == installation.FillialId)
					.Include(i => i.Filials)
					.Where(i => i.Fillial.DefaultInstallationId != i.Id)
					.OrderBy(i => i.Order).FirstOrDefaultAsync()
					?? throw new BadHttpRequestException($"Сannot delete the latest installation in the filial");

			filial.DefaultInstallation = newDefaultInstallation;
		}

		_context.Installations.Remove(installation);
		await _context.SaveChangesAsync();
	}

	
	private async Task<byte?> GetOrder(InstallationDTO installation)
	{
		if (installation.Order == null || installation.Order == 0)
		{
			return (byte)await _context.Installations.MaxAsync(x => x.Order + 1);
		}

		Installation? instalationFromDB = await _context.Installations
				.SingleOrDefaultAsync(x => x.FillialId == installation.FilialId && x.Order == installation.Order);

		return instalationFromDB == null ? installation.Order : null;
	}

}
