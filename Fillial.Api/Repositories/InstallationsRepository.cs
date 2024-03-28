using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Repositories.IRepositories;

namespace PrinterFil.Api.Repositories;

public class InstallationsRepository : IInstallationsRepository
{
	private readonly FilialServerContext _context;

	public InstallationsRepository(FilialServerContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<Installation>> ReadAsync(int? filialId)
	{
		return await _context
			.Installations
			.Where(i => filialId == null || i.FilialId == filialId)
			.ToArrayAsync();
	}
	public async Task<Installation?> ReadAsync(int id)
	{
		return await _context
			.Installations
			.SingleOrDefaultAsync(x => x.Id == id);
	}

	public async Task<byte> CreateAsync(Installation installation)
	{
		await _context.Installations.AddAsync(installation);
		return installation.Order;
	}

	public async Task UpdateDefaultInstallationAsync(Installation installation)
	{
		//Filial filial = await _context.Filials.SingleAsync(f => f.Id == installation.FilialId);
		//filial.DefaultInstallation = installation;
	}


	public async Task UpdateDefaultInstallationAsync(int filialId)
	{
		//Installation installation = await _context
		//	.Installations
		//	.Include(i => i.Filial)
		//	.SingleOrDefaultAsync(i => i.FilialId == filialId);

		//installation.Filial.DefaultInstallation = installation;
	}

	public async Task DeleteAsync(int id)
	{
		Installation? installation = await _context.Installations.SingleAsync(i => i.Id == id);
		_context.Installations.Remove(installation);
	}

	public async Task<byte?> GetOrderAsync(int filialId, byte? order)
	{
		if (order == null || order == 0)
		{
			return (byte)await _context.Installations.MaxAsync(x => x.Order + 1);
		}

		Installation? installation = await _context
			.Installations.SingleOrDefaultAsync(x => x.FilialId == filialId && x.Order == order);

		return installation == null ? order : null;
	}

	public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
