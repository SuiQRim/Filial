using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Repositories.IRepositories;

namespace PrinterFil.Api.Repositories;

public class PrintersRepository : IPrintersRepository
{
	private readonly FilialServerContext _context;

	public PrintersRepository(FilialServerContext context)
	{
		_context = context;
	}

	/// <inheritdoc/>
    public async Task<IEnumerable<Printer>> ReadAsync<T>() where T : Printer
	{
		return await _context.Printers.OfType<T>().ToListAsync();
	}

}

