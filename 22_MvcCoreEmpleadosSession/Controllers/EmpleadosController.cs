using _22_MvcCoreEmpleadosSession.Extensions;
using _22_MvcCoreEmpleadosSession.Models;
using _22_MvcCoreEmpleadosSession.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace _22_MvcCoreEmpleadosSession.Controllers
{
    public class EmpleadosController : Controller
    {

        private RepositoryEmpleados repo;

        public EmpleadosController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }

        public IActionResult SessionSalarios(int? salario)
        {
            if(salario != null)
            {
                int sumaSalarial = 0;

                //PREGUNTAMOS SI YA TENEMOS DATOS ALMACENADOS EN SESSION
                if(HttpContext.Session.GetString("SUMASALARIAL") != null)
                {
                    //RECUPERAMOS LO QUE TENGAMOS ALMACENADO
                    sumaSalarial = int.Parse(HttpContext.Session.GetString("SUMASALARIAL"));
                }

                //SUMAMOS EL SALARIO RECIBIDO A NUESTRA VARIABLE
                sumaSalarial += salario.Value;

                //ALMACENAMOS EL NUEVO VALOR EN SESSION
                HttpContext.Session.SetString("SUMASALARIAL", sumaSalarial.ToString());

                ViewData["MENSAJE"] = "Salario almacenado: " + salario.Value;
            }

            List<Empleado> empleados = this.repo.GetEmpleados();
            return View(empleados);
        }

        public IActionResult SumaSalarios()
        {
            return View();
        }

        public IActionResult SessionEmpleados(int? idEmpleado)
        {
            if(idEmpleado != null)
            {
                //QUEREMOS ALMACENAR ALGO
                Empleado empleado = this.repo.FindEmpleado(idEmpleado.Value);

                //ALMACENAREMOS UNA COLECION DE EMPLEADOS
                List<Empleado> empleadosSession;

                if(HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS") != null)
                {
                    //TENEMOS EMPLEADOS ALMACENADOS
                    empleadosSession = HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS");
                }
                else
                {
                    //NO EXISTEN EMPLEADOS TODAVIA
                    empleadosSession = new List<Empleado>();
                }

                //AGREGAMOS EL NUEVO EMPLEADO A NUESTRA COLECCION
                empleadosSession.Add(empleado);

                //REFRESCAMOS LOS DATOS DE SESSION
                HttpContext.Session.SetObject("EMPLEADOS", empleadosSession);

                ViewData["MENSAJE"] = "Empleado: " + empleado.Apellido + " almacenado en Session.";
            }

            List<Empleado> empleados = this.repo.GetEmpleados();
            return View(empleados);
        }

        public IActionResult EmpleadosAlmacenados()
        {
            return View();
        }

        public IActionResult EmpleadosSessionOK(int? idEmpleado)
        {
            if(idEmpleado != null)
            {
                List<int> idsEmpleado;
                if(HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") == null)
                {
                    //CREAMOS LA LISTA PARA LOS IDS
                    idsEmpleado = new List<int>();
                }
                else
                {
                    //REUPERAMOS LOS IDS ALMACENADOS PREVIAMENTE
                    idsEmpleado = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }

                idsEmpleado.Add(idEmpleado.Value);

                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleado);

                ViewData["MENSAJE"] = "Empleados almacenados:" + idsEmpleado.Count;
            }

            List<Empleado> empleados = this.repo.GetEmpleados();
            return View(empleados);
        }

        public IActionResult EmpleadosAlmacenadosOK()
        {
            //RECUPERAMOS LOS DATOS DE SESSION
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");

            if(idsEmpleados == null)
            {
                //NO HAY NADA EN SESSION
                ViewData["MENSAJE"] = "No existen empleados almacenados";

                //DEVOLVEMOS LA VISTA SIN EL MODEL
                return View();
            }
            else
            {
                List<Empleado> empleadosSession = this.repo.GetEmpleadosSession(idsEmpleados);

                return View(empleadosSession);
            }

            return View();
        }
    }
}
