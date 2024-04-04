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
		private readonly IFilialsRepository _filialsRepository;
		private readonly IPrintersRepository _printersRepository;

		public InstallationsControllers(IInstallationsRepository repository,
			IFilialsRepository filialsRepository, IPrintersRepository printersRepository)
		{
			_repository = repository;
			_filialsRepository = filialsRepository;
			_printersRepository = printersRepository;
		}

		/// <summary>
		/// Предоставляет список инсталляций
		/// </summary>
		/// <param name="filialId">Идентификатор филиала, к которому привязаны инсталляции</param>
		/// <returns>Список инсталляций</returns>
		/// <response code="200">Успешное предоставление</response>
		[ProducesResponseType(typeof(IEnumerable<Installation>), 200)]
		[HttpGet("collection")]
		public async Task<ActionResult<IEnumerable<Installation>>> Get([FromQuery] int? filialId)
		{
			return Ok(await _repository.ReadAsync(filialId));
		}


		/// <summary>
		/// Предоставляет инсталляцию
		/// </summary>
		/// <param name="id">Идентификатор</param>
		/// <response code="200">Успешное предоставление</response>
		/// <response code="404">Какой-то параметр не прошел проверку на существование</response>
		[ProducesResponseType(typeof(Installation), 200)]
		[ProducesResponseType(404)]
		[HttpGet]
		public async Task<ActionResult<Installation>> Get([Required][FromQuery] int id)
		{
			Installation? install = await _repository.ReadAsync(id);

			if (install == null)
				return NotFound();

			return Ok(install);
		}


		/// <summary>
		/// Добавляет новую инсталляцию
		/// </summary>
		/// <param name="installation">Инсталляция</param>
		/// <response code="201">Добавлено</response>
		/// <response code="404">Какой-то параметр не прошел проверку на существование</response>
		/// <response code="401">Некорректные данные</response>
		/// <returns>Идентификатор</returns>
		[ProducesResponseType(typeof(int), 201)]
		[ProducesResponseType(404)]
		[ProducesResponseType(401)]
		[HttpPost]
		public async Task<ActionResult<int>> Add(InstallationDTO installation)
		{
			if (!await _filialsRepository.ExistAsync(installation.FilialId))
				return NotFound("Филиал не существует");

			if (!await _printersRepository.ExistAsync(installation.PrintingDeviceId))
				return NotFound("Печатное устройство не существует");

			byte? order = await CalculateOrder(installation.Order, installation.FilialId);
			if (order == null)
				return BadRequest("Невозможно сгенерировать или использовать предложенный порядковый номер");

			bool isDefault = installation.IsDefault;
			if (isDefault)
			{
				await ResetDefaultInstallationAsync(installation.FilialId);
			}
			else if (!await _repository.DefaultExistAsync(installation.FilialId))
			{
				isDefault = true;
			}

			Installation newInstallation = new()
			{
				FilialId = installation.FilialId,
				Name = installation.Name,
				DeviceId = installation.PrintingDeviceId,
				IsDefault = isDefault,
				Order = (byte)order
			};


			int? id = await _repository.CreateAsync(newInstallation);

			if (id == null)
				return BadRequest("Не удалось добавить инсталляцию");

			return CreatedAtAction(nameof(Add), id);
		}

		private async Task<byte?> CalculateOrder(byte? order, int filialId)
		{
			if (order == null)
			{
				order = await _repository.GetOrderAsync(filialId);
			}
			else
			{
				order = await _repository.ExistByOrderAsync(filialId, (byte)order) ? null : order;
			}
			return order;
		}

		private async Task ResetDefaultInstallationAsync(int filialId)
		{
			Installation? defaultInstallation = await _repository.ReadDefaultAsync(filialId);
			if (defaultInstallation != null)
			{
				defaultInstallation.IsDefault = false;
				await _repository.UpdateAsync(defaultInstallation.Id, defaultInstallation);
			}
		}

		/// <summary>
		/// Удаляет инсталляцию
		/// </summary>
		/// <param name="id">Идентификатор</param>
		/// <returns></returns>
		/// <response code="200">Удален</response>
		[ProducesResponseType(200)]
		[HttpDelete]
		public async Task<IActionResult> Delete([Required] int id) 
		{ 
			Installation? installation = await _repository.ReadAsync(id);
			if (installation == null)
				return Ok();

			await _repository.DeleteAsync(id);

			if (installation.IsDefault)
			{
				await UpdateDefaultInstallationAsync(installation.FilialId);
			}

			return Ok();
		}

		private async Task UpdateDefaultInstallationAsync(int filialId)
		{
			Installation? newDefaultInstallation = await _repository.ReadFirstAsync(filialId);

			if (newDefaultInstallation != null)
			{
				newDefaultInstallation.IsDefault = true;
				await _repository.UpdateAsync(newDefaultInstallation.Id, newDefaultInstallation);
			}
		}

	}
}
