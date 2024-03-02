using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;

namespace PrinterFil.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilialsController : ControllerBase
    {
        private readonly FiilalServerContext _context;

        public FilialsController(FiilalServerContext context)
        {
            _context = context;
        }

		/// <summary>
		/// Возвращает список всех филиалов
		/// </summary>
		/// <returns></returns>
		[HttpGet]
        public async Task<ActionResult<IEnumerable<FilialDTO>>> GetFilials()
        {
			return await _context.Filials.Select(x => new FilialDTO(x.Id, x.Location, x.Name)).ToListAsync();
		}

    }
}
