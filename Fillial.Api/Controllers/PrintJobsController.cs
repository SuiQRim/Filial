using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;

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
			Filial? filial = await _context
				  .Filials
                  .Include(f => f.Installations)
                  .SingleOrDefaultAsync(f => f.Employees.Any(e => e.Id == printJob.EmployeeId));

			if (filial is null)
				return NotFound($"The employee does not belong to any Filial or not Exist");

			Installation? installation = printJob.InstallationOrder == null ?
				filial.DefaultInstallation :
				filial.Installations.FirstOrDefault(i => i.Order == printJob.InstallationOrder);

            if (installation is null)
				return NotFound($"The installation was not found");

			PrintJob job = new()
			{
				Task = printJob.Name,
				EmployeeId = printJob.EmployeeId,
				Installation = installation,
				LayerCount = printJob.LayerCount,
				Status = ImitateOfPrint()
			};

			await _context.PrintJobs.AddAsync(job);
            await _context.SaveChangesAsync();

            return Ok(job.Status.Name);
		}

		private PrintStatus ImitateOfPrint()
		{
			Random rnd = new();
			int time = rnd.Next(1000, 4000);
			Thread.Sleep(time);

			return _context.PrintStatuses.Find(rnd.Next(2, 4)) ?? throw new ArgumentOutOfRangeException();
		}
	}
}
