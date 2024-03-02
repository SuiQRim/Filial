using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;

namespace PrinterFil.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class InstallationsControllers : ControllerBase
	{
		private readonly FilialServerContext _context;

		public InstallationsControllers(FilialServerContext context)
		{
			_context = context;
		}
		
		/// <summary>
		/// Добавляет новую инсталляцию
		/// </summary>
		/// <param name="installation">Инсталляция</param>
		/// <returns>Идентификатор</returns>
		[HttpPost]
		public async Task<ActionResult<int>> Add(InstallationDTO installation)
		{
			Filial? filial = await _context.Filials.FindAsync(installation.FilialId);
			if (filial == null) 
				return NotFound($"Filial with id [{installation.FilialId}] is not found");

			PrintingDevice? printer = await _context.PrintingDevices.FindAsync(installation.PrintingDeviceId);
			if (printer == null)
				return NotFound($"Printing device with id [{installation.PrintingDeviceId}] is not found");


			int? order = await GetOrder(installation);
			if (order == null)
				return BadRequest($"Order is busy");


			Installation newInstallation = new()
			{
				Name = installation.Name,
				DeviceId = installation.PrintingDeviceId,
				FillialId = installation.PrintingDeviceId,
				Order = (int)order
			};

			//Если нужно выставить инсталляцию по умолчанию, то проще добавить через филиал
			if (installation.IsDefault)
			{
				filial.DefaultInstallation = newInstallation;
			}
			else
			{
				await _context.Installations.AddAsync(newInstallation);
			}

			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(Add), newInstallation.Id);
		}

		private async Task<int?> GetOrder(InstallationDTO installation)
		{
			if (installation.Order == null || installation.Order == 0)
			{
				return _context.Installations.Max(x => x.Order + 1);
			}

			Installation? instalationFromDB = await _context.Installations.SingleOrDefaultAsync(x => x.FillialId == installation.FilialId && x.Order == installation.Order);

			return instalationFromDB == null ? installation.Order : null;
		}
	}
}
