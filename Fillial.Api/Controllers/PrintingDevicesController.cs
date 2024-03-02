using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;

namespace PrinterFil.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrintingDevicesController : ControllerBase
    {
        private readonly FiilalServerContext _context;

        public PrintingDevicesController(FiilalServerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Возвращает список печатных устройств
        /// </summary>
        /// <param name="connectionType">Тип подключения</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrintingDeviceDTO>>> GetPrintingDevices([FromQuery] int connectionType)
        {
            if (connectionType < 0)
                return BadRequest("connectionType incorrect");

            return Ok(await _context.PrintingDevices
                .Where(c => connectionType == 0 || c.ConnectionTypeId == connectionType)
                .Select(x => new PrintingDeviceDTO(x.Id, x.Name, x.ConnectionTypeId, x.MacAddress))
                .ToListAsync());
        }

    }
}
