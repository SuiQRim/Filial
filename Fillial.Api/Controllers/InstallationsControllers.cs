using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;
using PrinterFil.Api.Repositories.IRepositories;

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
		public async Task<ActionResult<IEnumerable<InstallationResponseDTO>>> Get([FromQuery] int? filialId) =>
			Ok(await _repository.ReadAsync(filialId));

		/// <summary>
		/// Предоставляет инсталляцию
		/// </summary>
		/// <param name="id">Идентификатор</param>
		/// <response code="200">Успешное предоставление</response>
		/// <response code="404">Какой-то параметр не прошел проверку на существование</response>
		[ProducesResponseType(typeof(InstallationResponseDTO), 200)]
		[ProducesResponseType(404)]
		[HttpGet]
		public async Task<ActionResult<InstallationResponseDTO>> Get([FromQuery] int id) =>
			Ok(await _repository.ReadAsync(id));

		/// <summary>
		/// Добавляет новую инсталляцию
		/// </summary>
		/// <param name="installation">Инсталляция</param>
		/// <returns>Идентификатор</returns>
		/// <response code="202">Успешное добавление</response>
		/// <response code="404">Какой-то параметр не прошел проверку на существование</response>
		[ProducesResponseType(typeof(int), 202)]
		[ProducesResponseType(404)]
		[HttpPost]
		public async Task<ActionResult<int>> Add(InstallationDTO installation) =>
			CreatedAtAction(nameof(Add), await _repository.CreateAsync(installation));

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
			await _repository.DeleteAsync(id);
			return Ok();
		}
	}
}
