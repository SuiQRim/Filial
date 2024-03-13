using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Repositories.IRepositories;

namespace PrinterFil.Api.Repositories;

public class PrintingDevicesRepository : IPrintingDevicesRepository
{
	private readonly FilialServerContext _context;

	public PrintingDevicesRepository(FilialServerContext context)
	{
		_context = context;
	}

	/// <inheritdoc/>
    public async Task<IEnumerable<PrintingDevice>> ReadAsync(int? connectionType)
	{
		return await _context.PrintingDevices
			.Where(c => connectionType == null || c.ConnectionTypeId == connectionType)
			.ToListAsync();
	}
}

