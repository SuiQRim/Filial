using Microsoft.AspNetCore.Mvc;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;
using PrinterFil.Api.Repositories.IRepositories;
using System.ComponentModel.DataAnnotations;

namespace PrinterFil.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class InstallationsControllers : ControllerBase
	{
		private readonly IInstallationsRepository _repository;

		public InstallationsControllers(IInstallationsRepository repository)
		{
			_repository = repository;
		}

		/// <summary>
		/// Предоставляет список инсталляций
		/// </summary>
		/// <param name="filialId">Идентификатор филиала, к которому привязаны инсталляции</param>
		/// <returns>Список инсталляций</returns>
		/// <response code="200">Успешное предоставление</response>
		/// <response code="404">Какой-то параметр не прошел проверку на существование</response>
		[ProducesResponseType(typeof(IEnumerable<InstallationResponseDTO>), 200)]
		[ProducesResponseType(404)]
		[HttpGet("collection")]
		public async Task<ActionResult<IEnumerable<InstallationResponseDTO>>> Get([FromQuery] int? filialId)
		{
			IEnumerable<Installation> installations = await _repository.ReadAsync(filialId);

			IEnumerable<InstallationResponseDTO> installationsDTO = 
				installations.Select(i => new InstallationResponseDTO(
					i.Id, i.Name, i.FilialId, i.DeviceId, i.IsDefault, i.Order));

			return Ok(installationsDTO);
		}


		/// <summary>
		/// Предоставляет инсталляцию
		/// </summary>
		/// <param name="id">Идентификатор</param>
		/// <response code="200">Успешное предоставление</response>
		/// <response code="404">Какой-то параметр не прошел проверку на существование</response>
		[ProducesResponseType(typeof(InstallationResponseDTO), 200)]
		[ProducesResponseType(404)]
		[HttpGet]
		public async Task<ActionResult<InstallationResponseDTO>> Get([Required][FromQuery] int id)
		{
			Installation? install = await _repository.ReadAsync(id);
			if (install == null)
				return NotFound();

			InstallationResponseDTO installationDTO = new (install.Id, 
				install.Name, install.FilialId, install.DeviceId, 
				install.IsDefault, install.Order);

			return Ok(installationDTO);
		}


		/// <summary>
		/// Добавляет новую инсталляцию
		/// </summary>
		/// <param name="installation">Инсталляция</param>
		/// <returns>Идентификатор</returns>
		/// <response code="201">Успешное добавление</response>
		/// <response code="400">Плохой запрос</response>
		[ProducesResponseType(typeof(int), 201)]
		[ProducesResponseType(400)]
		[HttpPost]
		public async Task<ActionResult<int>> Add(InstallationDTO installation)
		{
			byte? maxOrder = await GetOrderAsync(installation.FilialId, installation.Order);

			if (maxOrder == null)
				return BadRequest();
			        
			Installation newInstallation = new()
			{
				FilialId = installation.FilialId,
				Name = installation.Name,
				DeviceId = installation.PrintingDeviceId,
				IsDefault = installation.IsDefault,
				Order = (byte)maxOrder
			};

			if (newInstallation.IsDefault)
			{
				Installation? defaultInstallation = await _repository.ReadDefaultAsync(installation.FilialId);
				if (defaultInstallation != null)
					defaultInstallation.IsDefault = false;
			}

			if (!await _repository.AnyInFilial(installation.FilialId))
			{
				newInstallation.IsDefault = true;
			}

			await _repository.CreateAsync(newInstallation);
			await _repository.SaveChangesAsync();

			return CreatedAtAction(nameof(Add), newInstallation.Id);
		}

		private async Task<byte?> GetOrderAsync(int filialId, byte? order)
		{
			if (order == null)
			{
				return (byte)(await _repository.GetOrderAsync(filialId) + 1);
			}
			else if (!await _repository.Exist(filialId, (byte)order))
			{
				return (byte)order;
			}
			return null;
		}


		/// <summary>
		/// Удаляет инсталляцию
		/// </summary>
		/// <param name="id">Идентификатор</param>
		/// <returns></returns>
		/// <response code="200">Успешное удаление</response>
		/// <response code="404">Какой-то параметр не прошел проверку на существование</response>
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		[HttpDelete]
		public async Task<IActionResult> Delete(int id) 
		{ 
			Installation? installation = await _repository.ReadAsync(id);
			if (installation == null)
				return NotFound();

			await _repository.DeleteAsync(id);

			if (installation.IsDefault)
			{
				Installation? newDefaultInstallation = await _repository.ReadFirstAsync(installation.FilialId);

				if (newDefaultInstallation != null)
				{
					newDefaultInstallation.IsDefault = true;
				}
			}

			await _repository.SaveChangesAsync();

			return Ok();
		}
	}
}
