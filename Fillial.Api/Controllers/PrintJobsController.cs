using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Exceptions;
using PrinterFil.Api.Models;
using System.Globalization;
using System.Text;

namespace PrinterFil.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrintJobsController : ControllerBase
    {
        private readonly FilialServerContext _context;

        public PrintJobsController(FilialServerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Регистрирует печать
        /// </summary>
        /// <param name="printJob"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<PrintJob>> PostPrintJob(PrintJobDTO printJob)
        {
			var printTask = await CreatePrintJob(printJob);

			await _context.PrintJobs.AddAsync(printTask);
            await _context.SaveChangesAsync();

            return Ok(printTask.Status.Name);
		}

		/// <summary>
		/// Импортирует файл, добавляя записи в бд
		/// </summary>
		/// <param name="uploadedFile">CSV файл с записями</param>
		/// <returns></returns>
		[HttpPost("import")]
		public async Task<IActionResult> ImportPrintJobCSV(IFormFile uploadedFile) {

			PrintJobDTO[] records;

			using (StreamReader streamReader = new(uploadedFile.OpenReadStream()))
			{
				CsvConfiguration csvConfig = new (CultureInfo.InvariantCulture)
				{
					Delimiter = ";",
					Encoding = Encoding.UTF8
				};
				using CsvReader csvReader = new(streamReader, csvConfig);
				records = csvReader.GetRecords<PrintJobDTO>().ToArray();
			}

			List<PrintJob> PrintJobs = new();

			foreach (var printJob in records)
			{
				var printTask = await CreatePrintJob(printJob);
				PrintJobs.Add(printTask);
			}

			await _context.PrintJobs.AddRangeAsync(PrintJobs);
			await _context.SaveChangesAsync();

			return Ok();
		}

		private async Task<PrintJob> CreatePrintJob(PrintJobDTO printJobDTO)
		{
			Filial? filial = await _context
				.Filials
				.Include(f => f.Installations)
				.SingleOrDefaultAsync(f => f.Employees.Any(e => e.Id == printJobDTO.EmployeeId))
				?? throw new EntityNotFoundExceptions("The employee does not belong to any Filial or not Exist");

			Installation? installation = printJobDTO.InstallationOrder == null ?
				filial.DefaultInstallation :
				filial.Installations.FirstOrDefault(i => i.Order == printJobDTO.InstallationOrder)
				?? throw new EntityNotFoundExceptions("The installation was not found");

			PrintJob job = new()
			{
				Task = printJobDTO.Name,
				EmployeeId = printJobDTO.EmployeeId,
				Installation = installation,
				LayerCount = printJobDTO.LayerCount,
				Status = ImitateOfPrint()
			};
			return job;
		}

		private PrintStatus ImitateOfPrint(bool withTime = true)
		{
			Random rnd = new();
			if (withTime)
			{
				int time = rnd.Next(1000, 4000);
				Thread.Sleep(time);
			}
			
			return _context.PrintStatuses.Find(rnd.Next(2, 4)) ?? throw new ArgumentOutOfRangeException();
		}
	}
}
