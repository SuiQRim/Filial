using Microsoft.AspNetCore.Mvc;
using PFilial.API.Responses.V1;
using PFilial.BLL.Services.Interfaces;
using PrinterFil.Api.Models;

namespace PFilial.API.Controllers.V1;

[Route("api/v1/[controller]")]
[ApiController]
public class InstallationsController : ControllerBase
{
    private readonly IInstallationsService _installationsService;
	public InstallationsController(IInstallationsService installationsService)
    {
		_installationsService = installationsService;
	}

	[HttpGet]
	public async Task<ActionResult<Installation[]>> GetInstallations([FromQuery] int? filialId)
	{
		InstallationModel [] installations = await _installationsService.GetCollectionAsync(filialId);

		return Ok(installations
			.Select(x => new Installation (
				x.Id,
				x.Name,
				x.FilialId,
				x.PrintingDeviceId,
				x.IsDefault,
				x.Order))
			.ToArray());
	}
}
