using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;
using PrinterFil.Api.Repositories.IRepositories;
using System.Collections.Generic;

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
					i.Id, i.Name, i.FilialId, i.DeviceId, 
					i.Filial.DefaultInstallationId == i.Id, i.Order));

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
		public async Task<ActionResult<InstallationResponseDTO>> Get([FromQuery] int id)
		{
			Installation? install = await _repository.ReadAsync(id);
			if (install == null)
				return NotFound();

			InstallationResponseDTO installationDTO = new (install.Id, 
				install.Name, install.FilialId, install.DeviceId, 
				install.Filial.DefaultInstallationId == install.Id, install.Order);

			return Ok(installationDTO);
		}


		/// <summary>
		/// Добавляет новую инсталляцию
		/// </summary>
		/// <param name="installation">Инсталляция</param>
		/// <returns>Идентификатор</returns>
		/// <response code="202">Успешное добавление</response>
		/// <response code="404">Какой-то параметр не прошел проверку на существование</response>
		/// <response code="400">Плохой запрос</response>
		[ProducesResponseType(typeof(int), 202)]
		[ProducesResponseType(404)]
		[ProducesResponseType(400)]
		[HttpPost]
		public async Task<ActionResult<int>> Add(InstallationDTO installation)
		{
			byte? order = await _repository.GetOrderAsync(installation.FilialId, installation.Order);
			if (order == null)
				return BadRequest("Предложенный порядковый номер не может быть использован");

			Installation installationEntity = new()
			{
				FilialId = installation.FilialId,
				Name = installation.Name,
				DeviceId = installation.PrintingDeviceId,
				Order = (byte)order
			};

			byte newOrder = await _repository.CreateAsync(installationEntity);

			if (installation.IsDefault)
			{
				await _repository.UpdateDefaultInstallationAsync(installationEntity);
			}
			await _repository.SaveChangesAsync();

			return CreatedAtAction(nameof(Add), newOrder);
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

			if(installation.Filial.DefaultInstallationId == installation.Id)
				await _repository.UpdateDefaultInstallationAsync(installation.FilialId);

			await _repository.DeleteAsync(id);
			await _repository.SaveChangesAsync();

			return Ok();
		}
	}
}
