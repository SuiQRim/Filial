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
		/// Возвращает коллекцию инсталляций
		/// </summary>
		/// <param name="filialId">Фильтр по идентификатору филиала</param>
		/// <returns></returns>
		[HttpGet("collection")]
		public async Task<ActionResult<IEnumerable<InstallationResponceDTO>>> Get([FromQuery] int? filialId)
		{
			if (filialId != null && await _context.Filials.FindAsync(filialId) == null)
			{
				return NotFound($"Filial with id [{filialId}] is not found");
			}

			return Ok(
				await _context.Installations
					.Where(i => filialId == null || i.FillialId == filialId)
					.Include(i => i.Filials)
					.Select(i => new InstallationResponceDTO(
						i.Id, i.Name, i.FillialId, i.DeviceId,
						i.Filials.Single(f => f.Id == i.FillialId).DefaultInstallationId == i.Id,
						i.Order))
					.ToArrayAsync());
		}

		/// <summary>
		/// Возвращает инсталляцию по идентификатору
		/// </summary>
		/// <param name="id">Идентификатор</param>
		/// <returns></returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<InstallationResponceDTO>>> Get([FromQuery] int id)
		{
			Installation? installation = await _context.Installations.Include(i => i.Filials).SingleOrDefaultAsync(x => x.Id == id);
			if(installation is null)  
				return NotFound($"Installation with id [{id}] is not found");

			Installation i = installation;
			return Ok(new InstallationResponceDTO(
				i.Id, i.Name, i.FillialId, i.DeviceId,
				i.Fillial.DefaultInstallationId == i.Id,
				i.Order));
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

		/// <summary>
		/// Удаляет инсталляцию
		/// </summary>
		/// <param name="id">Идентификатор</param>
		/// <returns></returns>
		[HttpDelete]
		public async Task<IActionResult> Delete(int id)
		{
			Installation? installation = await _context.Installations.Include(i => i.Filials).SingleOrDefaultAsync(i => i.Id == id);
			if (installation == null)
				return NotFound($"Installation with id [{id}] is not found");

			Filial? filial = await _context.Filials.FindAsync(installation.FillialId);
			if (filial == null)
				return NotFound($"Filial with id [{installation.FillialId}] is not found");

			if (filial.DefaultInstallationId == installation.Id)
			{
				Installation? newDefaultInstallation =
					await _context.Installations
						.Where(i => i.FillialId == installation.FillialId)
						.Include(i => i.Filials)
						.Where(i => i.Fillial.DefaultInstallationId != i.Id)
						.OrderBy(i => i.Order).FirstOrDefaultAsync();

				if (newDefaultInstallation == null)
					return BadRequest("Сannot delete the latest installation in the filial");

				filial.DefaultInstallation = newDefaultInstallation;
			}

			_context.Installations.Remove(installation);
			await _context.SaveChangesAsync();

			return Ok();
		}


		private async Task<int?> GetOrder(InstallationDTO installation)
		{
			if (installation.Order == null || installation.Order == 0)
			{
				return _context.Installations.Max(x => x.Order + 1);
			}

			Installation? instalationFromDB = await _context.Installations
					.SingleOrDefaultAsync(x => x.FillialId == installation.FilialId && x.Order == installation.Order);

			return instalationFromDB == null ? installation.Order : null;
		}
	}
}
