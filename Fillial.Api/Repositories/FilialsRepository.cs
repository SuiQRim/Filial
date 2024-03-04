using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;
using PrinterFil.Api.Repositories.IRepositories;

namespace PrinterFil.Api.Repositories;

public class FilialsRepository : IFilialsRepository
{
	private readonly FilialServerContext _context;

	public FilialsRepository(FilialServerContext context)
	{
		_context = context;
	}

	/// <inheritdoc/>
	public async Task<IEnumerable<FilialDTO>> ReadAsync()
	{
		return await _context
			.Filials
			.Select(x => new FilialDTO(x.Id, x.Location, x.Name))
			.ToListAsync();
	}
}

