using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CRUDService.Models
{
    public class EmployeeContext:DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {

        }
       
        public DbSet<Employee> Employees { get; set; }

    }
    public class EmployeeContextFactory : IDesignTimeDbContextFactory<EmployeeContext>
    {
        public EmployeeContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EmployeeContext>();
            optionsBuilder.UseSqlServer("Server=LAPTOP-GOQGGO4C\\SQLEXPRESS;Database=myDataBase;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;");

            return new EmployeeContext(optionsBuilder.Options);
        }
    }
}
