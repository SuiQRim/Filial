using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;

namespace PrinterFil.Api.Repositories.IRepositories
{
	public interface IPrintJobsRepository
	{
		/// <summary>
		/// Создает задание печати
		/// </summary>
		/// <param name="printJobDTO"></param>
		/// <returns></returns>
		Task<PrintJob> CreateAsync(PrintJobDTO printJobDTO);

		/// <summary>
		/// Создает множество заданий печати импортируя список из файла
		/// </summary>
		/// <param name="uploadedFile">Файл с заданиями печати</param>
		/// <returns></returns>
		Task<int> ImportAsync(IFormFile uploadedFile);
	}
}
