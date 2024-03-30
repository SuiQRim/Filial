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

	public async Task<Installation?> ReadDefaultAsync(int filialId)
	{
		return await _context.Installations.SingleOrDefaultAsync(i => i.FilialId == filialId && i.IsDefault);
	}

	public async Task<Installation?> ReadFirstAsync(int filialId)
	{
		return await _context.Installations.Where(i => i.FilialId == filialId).FirstOrDefaultAsync();
	}

	public async Task CreateAsync(Installation installation)
	{
		await _context.Installations.AddAsync(installation);
	}

	public async Task DeleteAsync(int id)
	{
		Installation? installation = await _context.Installations.SingleOrDefaultAsync(i => i.Id == id);
		if (installation != null)
		{
			_context.Installations.Remove(installation);
		}	
	}

	public async Task<byte> GetOrderAsync(int filialId)
	{
		return await _context.Installations
			.Where(i => i.FilialId == filialId)
			.Select(p => p.Order)
			.DefaultIfEmpty()
			.MaxAsync();
	}
	public async Task<bool> Exist(int? filialId = null, byte? order = null)
	{
		return await _context.Installations.
			AnyAsync(x =>
				(filialId == null || x.FilialId == filialId) &&
				(order == null || x.Order == order)
			);
	}

	public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
