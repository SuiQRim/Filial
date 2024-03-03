using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Models;

namespace PrinterFil.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly FilialServerContext _context;

        public EmployeesController(FilialServerContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Возвращает список всех сотрудников компании
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetEmployees()
        {
			return Ok(await _context
                .Employees
                .Select(x => new EmployeeDTO(x.Id, x.Name, x.FillialId))
                .ToListAsync());
		}

    }
}
