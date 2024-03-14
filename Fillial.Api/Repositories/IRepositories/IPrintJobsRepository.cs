using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;

namespace PrinterFil.Api.Repositories.IRepositories
{
	public interface IPrintJobsRepository
	{
		/// <summary>
		/// Добавляет задание печати
		/// </summary>
		/// <param name="printJob"></param>
		/// <returns></returns>
		Task CreateAsync(PrintJob printJob);

		/// <summary>
		/// Добавляет множество заданий печати
		/// </summary>
		/// <param name="printJobs"></param>
		/// <returns></returns>
		Task CreateRangeAsync(IEnumerable<PrintJob> printJobs);

		/// <summary>
		/// Предоставляет филиал, в котором будет выполнятся печать
		/// </summary>
		/// <param name="filialId">Идентификатор филиала</param>
		/// <returns></returns>
		Task<Filial?> GetRunningFilialAsync(int filialId);

		/// <summary>
		/// Сохраняет изменения
		/// </summary>
		/// <returns></returns>
		Task SaveChangesAsync();
	}
}
