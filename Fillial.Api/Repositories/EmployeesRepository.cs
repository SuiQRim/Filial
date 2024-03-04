using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;
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
	public async Task<IEnumerable<EmployeeDTO>> ReadAsync()
	{
		return await _context
			.Employees
			.Select(x => new EmployeeDTO(x.Id, x.Name, x.FillialId))
			.ToListAsync();
	}
}
