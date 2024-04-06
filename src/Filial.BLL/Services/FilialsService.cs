using PFilial.BLL.Services.Interfaces;
using PFilial.DAL.Entities;
using PFilial.DAL.Repositories.Interfaces;
using PrinterFil.Api.Models;

namespace PFilial.BLL.Services;

public class FilialsService : IFilialsService
{
    private readonly IFilialsRepository _filialsRepository;
    public FilialsService(IFilialsRepository filialsRepository)
    {
		_filialsRepository = filialsRepository;

	}
    public async Task<FilialModel[]> GetFilials()
	{
		FilialEntity [] filials = await _filialsRepository.ReadAsync();

		return filials
			.Select(x => new FilialModel(
				x.Id,
				x.Name,
				x.Location))
			.ToArray();
	}
}
