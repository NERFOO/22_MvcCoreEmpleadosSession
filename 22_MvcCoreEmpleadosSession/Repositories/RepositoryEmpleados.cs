using _22_MvcCoreEmpleadosSession.Data;
using _22_MvcCoreEmpleadosSession.Models;

namespace _22_MvcCoreEmpleadosSession.Repositories
{
    public class RepositoryEmpleados
    {
        private EmpleadosContext context;

        public RepositoryEmpleados(EmpleadosContext context)
        {
            this.context = context;
        }

        public List<Empleado> GetEmpleados()
        {
            return this.context.Empleados.ToList();
        }

        public Empleado FindEmpleado(int idEmpleado)
        {
            return this.context.Empleados.FirstOrDefault(x => x.IdEmpleado == idEmpleado);
        }

        public List<Empleado> GetEmpleadosSession(List<int> ids)
        {
            //PARA REALIZAR UN IN() O UTILIZAR UNA CONSULTA OR MEDIANTE LINQ SE UTILIZA EL METODO CONTAINS DE LA COLECCION
            //SELECT * FROM EMP WHERE EMP_NO IN (999, 89, 88, 33, 444) --> ESTO SERIA EL WHERE
            var consulta = from datos in this.context.Empleados
                            where ids.Contains(datos.IdEmpleado)
                            select datos;
            if(consulta.Count() == 0)
            {
                return null;
            }
            return consulta.ToList();
        }
    }
}
