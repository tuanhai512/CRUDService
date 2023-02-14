using CRUDService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace CRUDService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeContext _employeeContext;
        private readonly IHostingEnvironment _hostingEnvironment;

        public EmployeeController(EmployeeContext employeeContext, IHostingEnvironment hostingEnvironment)
        {
            _employeeContext = employeeContext;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            if(_employeeContext.Employees == null)
            {
                return NotFound();
            }
            return await _employeeContext.Employees.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            if (_employeeContext.Employees == null)
            {
                return NotFound();
            }
            var employee = await _employeeContext.Employees.FindAsync(id);
            if(employee == null)
            { return NotFound(); }
            return employee;
        }

        [HttpPost()]
        public async Task<ActionResult<Employee>> PostEmployee([FromQuery]Employee employee,IFormFile img)
        {
            
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "images", img.FileName);
            var streamImage = new FileStream(path,
                                             FileMode.Append);
            img.CopyTo(streamImage);
            employee.PathImage = path ;
            _employeeContext.Employees.Add(employee);
            await _employeeContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEmployee),new {id = employee.ID },employee);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Employee>> PutEmpoyee(int id ,Employee employee)
        {
            if(id != employee.ID)
            {
                return BadRequest();
            }
            _employeeContext.Entry(employee).State = EntityState.Modified;
            try
            {
                await _employeeContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            if(_employeeContext.Employees == null)
            {
                return NotFound();
            }
            var employee = _employeeContext.Employees.Find(id);
            if(employee == null)
            {
                return NotFound();
            }
            _employeeContext.Remove(employee);
            await _employeeContext.SaveChangesAsync();
            return Ok();
        }
    }
}
