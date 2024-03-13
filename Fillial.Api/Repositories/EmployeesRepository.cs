using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Repositories.IRepositories;

namespace PrinterFil.Api.Repositories;

public class EmployeesRepository : IEmployeesRepository
{
	private readonly FilialServerContext _context;

	public EmployeesRepository(FilialServerContext context)
	{
		_context = context;
	}

	/// <inheritdoc/>
	public async Task<IEnumerable<Employee>> ReadAsync()
	{
		return await _context
			.Employees
			.ToListAsync();
	}
}
