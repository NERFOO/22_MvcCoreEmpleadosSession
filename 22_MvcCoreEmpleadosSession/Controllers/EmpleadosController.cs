using _22_MvcCoreEmpleadosSession.Extensions;
using _22_MvcCoreEmpleadosSession.Models;
using _22_MvcCoreEmpleadosSession.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace _22_MvcCoreEmpleadosSession.Controllers
{
    public class EmpleadosController : Controller
    {

        private RepositoryEmpleados repo;
        private IMemoryCache memoryCache;

        public EmpleadosController(RepositoryEmpleados repo, IMemoryCache memoryCache)
        {
            this.repo = repo;
            this.memoryCache = memoryCache;
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

        //[ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client)]
        public IActionResult EmpleadosSessionOK(int? idEmpleado, int? idFavorito)
        {
            if(idFavorito != null)
            {
                List<Empleado> empleadosFavoritos;

                if(this.memoryCache.Get("FAVORITOS") == null)
                {
                    empleadosFavoritos = new List<Empleado>();
                }
                else
                {
                    empleadosFavoritos = this.memoryCache.Get<List<Empleado>>("FAVORITOS");
                }

                //BUSCAMOS AL EMPLEADO EN BBDD PARA ALMACENARLO EN CACHE
                Empleado empleado = this.repo.FindEmpleado(idFavorito.Value);

                empleadosFavoritos.Add(empleado);

                //ALMACENAMOS LOS DATOS EN CACHE
                this.memoryCache.Set("FAVORITOS", empleadosFavoritos);
            }

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

        public IActionResult EmpleadosAlmacenadosOK(int? idEliminar)
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
                if(idEliminar != null)
                {
                    //ELIMINAMOS EL ELEMENTO QUE NOS HAN SOLICITADO
                    idsEmpleados.Remove(idEliminar.Value);

                    if(idsEmpleados.Count == 0)
                    {
                        HttpContext.Session.Remove("IDSEMPLEADOS");
                    }
                    else
                    {
                        //DEBEMOS ACTUALIZAR DE NUEVO SESSION
                        HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                    }
                }

                List<Empleado> empleadosSession = this.repo.GetEmpleadosSession(idsEmpleados);

                return View(empleadosSession);
            }
        }

        public IActionResult FavoritosOK()
        {
            return View();
        }
    }
}
