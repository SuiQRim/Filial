﻿using Microsoft.AspNetCore.Mvc;
using PFilial.API.Requests;
using PFilial.API.Responses.V1;
using PFilial.BLL.Models;
using PFilial.BLL.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

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

	[HttpGet("collection")]
	public async Task<ActionResult<InstallationResponse[]>> GetCollectionAsync([FromQuery] int? filialId)
	{
		InstallationModel[] installations = await _installationsService.GetCollectionAsync(filialId);

		return Ok(installations
			.Select(x => new InstallationResponse(
				x.Id,
				x.Name,
				x.FilialId,
				x.PrintingDeviceId,
				x.IsDefault,
				x.Order))
			.ToArray());
	}

	[HttpGet]
	public async Task<ActionResult<InstallationResponse>> GetByIdAsync([Required][FromQuery] int id)
	{
		InstallationModel? instal = await _installationsService.GetAsync(id);

		if (instal == null)
		{
			return NotFound();
		}

		return Ok(new InstallationResponse(
				instal.Id,
				instal.Name,
				instal.FilialId,
				instal.PrintingDeviceId,
				instal.IsDefault,
				instal.Order));
	}

	[HttpPost]
	public async Task<ActionResult<int>> AddAsync(AddInstallationRequest request)
	{
		InstallationModel installation = new(
			0,
			request.Name,
			request.FilialId,
			request.DeviceId,
			request.IsDefault,
			request.Order ?? 0);

		int? id = await _installationsService.AddAsync(installation);

		if (id == null)
		{
			return NotFound();
		}

		return Ok(id);

	}


	[HttpDelete]
	public async Task<ActionResult<InstallationResponse>> Delete([Required][FromQuery] int id)
	{
		await _installationsService.RemoveAsync(id);
		return Ok();
	}
}
