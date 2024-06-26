﻿using Microsoft.AspNetCore.Mvc;
using PFilial.API.Responses.V1;
using PFilial.BLL.Models;
using PFilial.BLL.Services.Interfaces;

namespace PFilial.API.Controllers.V1;

[Route("api/v1/[controller]")]
[ApiController]
public class FilialsController : ControllerBase
{
	private readonly IFilialsService _filialsService;

	public FilialsController(IFilialsService filialsService)
	{
		_filialsService = filialsService;
	}

	[HttpGet]
	public async Task<ActionResult<FilialModel[]>> GetFilials()
	{
		FilialModel[] filials = await _filialsService.GetFilials();

		return Ok(filials
			.Select(x => new FilialResponse(
				x.Id,
				x.Name,
				x.Location ?? string.Empty))
			.ToArray());
	}


}
