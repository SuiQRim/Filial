using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;
using PrinterFil.Api.Repositories.IRepositories;
using System.Linq;

namespace PrinterFil.Api.Repositories;

public class PrintJobsRepository : IPrintJobsRepository
{
	private readonly FilialServerContext _context;

    public PrintJobsRepository(FilialServerContext context)
    {
		_context = context;
	}

	/// <inheritdoc/>
    public async Task CreateAsync(PrintJob printJob)
	{
		await _context.PrintJobs.AddAsync(printJob);
	}

	/// <inheritdoc/>
	public async Task CreateRangeAsync(IEnumerable<PrintJob> printJobs)
	{
		await _context.PrintJobs.AddRangeAsync(printJobs);
	}

	/// <inheritdoc/>
	public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

	/// <inheritdoc/>
	public async Task<Filial?> GetRunningFilialAsync(int employeeId)
	{
		Filial? filial = await _context
			.Filials
			.Include(f => f.Employees)
			.Where(f => f
				.Employees
				.Any(e => e.Id == employeeId))
			.Include(f => f.Installations)
			.SingleOrDefaultAsync();

		return filial;
	}
}

