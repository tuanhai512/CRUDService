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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeeController(EmployeeContext employeeContext, IWebHostEnvironment webHostEnvironment)
        {
            _employeeContext = employeeContext;
            this._webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            if(_employeeContext.Employees == null)
            {
                return NotFound();
            }
            return await _employeeContext.Employees.Select(x => new Employee()
            {
                ID = x.ID,
                Name = x.Name,
                Age = x.Age,
                ImageName = x.ImageName,
                IsActive = x.IsActive,
                ImageSrc = String.Format("{0}://{1}{2}/images/{3}",Request.Scheme,Request.Host,Request.PathBase,x.ImageName)
            }).ToListAsync();
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

        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee([FromForm] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            employee.ImageName = await SaveImage(employee.ImageFile);
            _employeeContext.Employees.Add(employee);
            await _employeeContext.SaveChangesAsync();
            return StatusCode(201);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Employee>> PutEmpoyee(int id , [FromForm] Employee employee)
        {
            if(id != employee.ID)
            {
                return BadRequest();
            }
            if(employee.ImageName != null)
            {
                DeleteImage(employee.ImageName);
                employee.ImageName = await SaveImage(employee.ImageFile);   
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

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile)
        {
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName));
            imageName =imageName+ Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath,"images",imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
               await imageFile.CopyToAsync(fileStream);
            }
            return imageName;
        }

        [NonAction]
        public void DeleteImage(string imageName)
        {
            var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "images", imageName);
            if (System.IO.File.Exists(imagePath))
            {
                System
                    .IO.File.Delete(imagePath);
            }
        }
    }
}
