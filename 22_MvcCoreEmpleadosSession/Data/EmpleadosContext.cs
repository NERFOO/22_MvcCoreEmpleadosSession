using _22_MvcCoreEmpleadosSession.Models;
using Microsoft.EntityFrameworkCore;

namespace _22_MvcCoreEmpleadosSession.Data
{
    public class EmpleadosContext : DbContext
    {
        public EmpleadosContext(DbContextOptions<EmpleadosContext> options) : base(options) { }

        public DbSet<Empleado> Empleados { get; set; }
    }
}
