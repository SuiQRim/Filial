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
		/// Возвращает коллекцию инсталляций
		/// </summary>
		/// <param name="filialId">Фильтр по идентификатору филиала</param>
		/// <returns></returns>
		[HttpGet("collection")]
		public async Task<ActionResult<IEnumerable<InstallationResponseDTO>>> Get([FromQuery] int? filialId) =>
			Ok(await _repository.ReadAsync(filialId));

		/// <summary>
		/// Возвращает инсталляцию по идентификатору
		/// </summary>
		/// <param name="id">Идентификатор</param>
		/// <returns></returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<InstallationResponseDTO>>> Get([FromQuery] int id) =>
			Ok(await _repository.ReadAsync(id));

		/// <summary>
		/// Добавляет новую инсталляцию
		/// </summary>
		/// <param name="installation">Инсталляция</param>
		/// <returns>Идентификатор</returns>
		[HttpPost]
		public async Task<ActionResult<int>> Add(InstallationDTO installation) =>
			CreatedAtAction(nameof(Add), await _repository.CreateAsync(installation));


		/// <summary>
		/// Удаляет инсталляцию
		/// </summary>
		/// <param name="id">Идентификатор</param>
		/// <returns></returns>
		[HttpDelete]
		public async Task<IActionResult> Delete(int id) 
		{ 
			await _repository.DeleteAsync(id);
			return Ok();
		}
	}
}
