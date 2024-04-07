using PFilial.BLL.Models;
using PFilial.BLL.Services.Interfaces;
using PFilial.DAL.Entities;
using PFilial.DAL.Repositories.Interfaces;

namespace PFilial.BLL.Services;

public class PrintJobsService : IPrintJobsService
{
	private readonly IPrintJobImporter _printingJobImporter;

	private readonly IPrintJobsRepository _printJobsRepository;

	public PrintJobsService(IPrintJobImporter printingJobImporter, IPrintJobsRepository printJobsRepository)
    {
		_printingJobImporter = printingJobImporter;
		_printJobsRepository = printJobsRepository;
	}

    public async Task<int?> Add(PrintJobModel printJob)
	{
		//TODO: провалидировать все
		int? id = await _printJobsRepository.CreateAsync(new PrintJobEntity()
		{
			Id = 0,
			Name = printJob.Name,
			EmployeeId = printJob.EmployeeId,
			LayerCount = printJob.LayerCount,
			Order = printJob.InstallationOrder ?? 0,
			IsSuccessful = null,
		});

		return id;
	}

	public async Task<int> Import(Stream stream)
	{
		PrintJobDTO [] jobDTOs = await _printingJobImporter.ParseAsync(stream);

		int count = await _printJobsRepository.CreateRangeAsync(jobDTOs.Select(x => new PrintJobEntity 
		{
			Id = 0,
			Name = x.Name,
			EmployeeId = x.EmployeeId,
			LayerCount = x.LayerCount,
			Order = x.InstallationOrder,
			IsSuccessful = null,
		}).ToArray());

		return count;
	}
}
