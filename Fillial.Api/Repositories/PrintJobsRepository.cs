using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Exceptions;
using PrinterFil.Api.Models;
using PrinterFil.Api.Repositories.IRepositories;
using System.Globalization;
using System.Text;

namespace PrinterFil.Api.Repositories;

public class PrintJobsRepository : IPrintJobsRepository
{
	private readonly FilialServerContext _context;

    public PrintJobsRepository(FilialServerContext context)
    {
		_context = context;
	}

	/// <inheritdoc/>
    public async Task<PrintJob> CreateAsync(PrintJobDTO printJobDTO)
	{
		PrintJob printJob = await CreatePrintJob(printJobDTO);
		await _context.PrintJobs.AddAsync(printJob);
		await _context.SaveChangesAsync();
		return printJob;
	}

	/// <inheritdoc/>
	public async Task<int> ImportAsync(IFormFile uploadedFile)
	{
		const int maxJobs = 100;

		PrintJobDTO [] records = ParsePrintJobsFromCSV(uploadedFile);
		PrintJob [] PrintJobs = new PrintJob[records.Length < 100 ? records.Length : maxJobs];

		int count = 0;
		foreach (PrintJobDTO printJob in records)
		{	
			PrintJobs[count] = await CreatePrintJob(printJob);
			count++;
			if (count == maxJobs)
				break;
		}

		await _context.PrintJobs.AddRangeAsync(PrintJobs);
		await _context.SaveChangesAsync();

		return count;
	}

	/// <inheritdoc/>
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

	private static PrintJobDTO [] ParsePrintJobsFromCSV(IFormFile uploadedFile)
	{
		try
		{
			PrintJobDTO[] records;

			using StreamReader streamReader = new(uploadedFile.OpenReadStream());
			CsvConfiguration csvConfig = new(CultureInfo.InvariantCulture)
			{
				Delimiter = ";",
				Encoding = Encoding.UTF8,
				HasHeaderRecord = false,
				ShouldSkipRecord = x => x.Row.Parser.Record?.Any(field => string.IsNullOrWhiteSpace(field)) ?? false
			};

			using CsvReader csvReader = new(streamReader, csvConfig);
			records = csvReader.GetRecords<PrintJobDTO>().ToArray();

			return records;
		}
		catch (HeaderValidationException)
		{
			throw new ParsingFileException("Print Jobs could not be parsed");
		}

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

