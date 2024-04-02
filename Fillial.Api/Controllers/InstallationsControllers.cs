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

			byte? order;
			if (installation.Order == null)
			{
				order = await _repository.GetOrderAsync(installation.FilialId);
				if (order == null)
					return BadRequest("Нет свободного порядкового номера");
			}
			else
			{
				if (await _repository.Exist(installation.FilialId, (byte)installation.Order))
					return BadRequest("Порядковый номер занят");
				order = installation.Order;
			}

			bool isDefault = installation.IsDefault;
			if (isDefault)
			{
				Installation? defaultInstallation = await _repository.ReadDefaultAsync(installation.FilialId);
				if (defaultInstallation != null)
				{
					defaultInstallation.IsDefault = false;
					await _repository.UpdateAsync(defaultInstallation.Id, defaultInstallation);
				}
					
			}
			else if (!await _repository.Exist(filialId: installation.FilialId))
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
				return BadRequest();

			return CreatedAtAction(nameof(Add), id);
		}

		/// <summary>
		/// Удаляет инсталляцию
		/// </summary>
		/// <param name="id">Идентификатор</param>
		/// <returns></returns>
		/// <response code="200">Удален</response>
		[ProducesResponseType(200)]
		[HttpDelete]
		public async Task<IActionResult> Delete(int id) 
		{ 
			Installation? installation = await _repository.ReadAsync(id);
			if (installation == null)
				return Ok();

			await _repository.DeleteAsync(id);

			if (installation.IsDefault)
			{
				Installation? newDefaultInstallation = await _repository.ReadFirstAsync(installation.FilialId);

				if (newDefaultInstallation != null)
				{
					newDefaultInstallation.IsDefault = true;
					await _repository.UpdateAsync(newDefaultInstallation.Id, newDefaultInstallation);
				}
			}

			return Ok();
		}
	}
}
