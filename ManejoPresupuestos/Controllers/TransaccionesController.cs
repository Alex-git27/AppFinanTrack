﻿using AutoMapper;
using ClosedXML.Excel;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Reflection;

namespace ManejoPresupuestos.Controllers
{
    [Authorize] //SE OBTIENE VISTA QUE SE DEFINE EN PROGRAM PARA USUARIOS NO REGISTRADOS

    public class TransaccionesController: Controller
    {
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IMapper mapper;
        private readonly IServicioReportes servicioReportes;

        public TransaccionesController(IServicioUsuarios servicioUsuarios, 
            IRepositorioCuentas repositorioCuentas,
            IRepositorioCategorias repositorioCategorias,
            IRepositorioTransacciones repositorioTransacciones,
            IMapper mapper, IServicioReportes servicioReportes)
        {
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioCategorias = repositorioCategorias;
            this.repositorioTransacciones = repositorioTransacciones;
            this.mapper = mapper;
            this.servicioReportes = servicioReportes;
        }

        //CONTROLAMOS ACCCIONES DE USUARIOS NO AUTENTICADOS

        [Authorize]
     

        //RETORNAMOS VISTA A INDEX
        public async Task<IActionResult> Index(int mes, int año) //AGREGAMOS PARAMETROS DE MES Y AÑO
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var modelo = await servicioReportes.ObtenerReporteTransaccionesDetalladas(usuarioId, mes, año, ViewBag);
            return View(modelo);
        }


        //VISTA DE REPORTE SEMANAL
        public async Task<IActionResult> Semanal(int mes, int año)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            IEnumerable<ResultadoObtenerPorSemana> transaccionesPorSemana =
                await servicioReportes.ObtenerReporteSemanal(usuarioId, mes, año, ViewBag);

            var agrupado = transaccionesPorSemana.GroupBy(x => x.Semana).Select(x => new ResultadoObtenerPorSemana()
            {
                Semana = x.Key, //REGISTRO PARA DATO DE LA SEMANA
                Ingresos = x.Where(x => x.TipoOperacionId == TipoOperacion.Ingreso).Select(x => x.Monto).FirstOrDefault(), //DATOS DE INGRESO
                Gastos = x.Where(x => x.TipoOperacionId == TipoOperacion.Gasto).Select(x => x.Monto).FirstOrDefault()  // DATOS DE GASTOS

            }).ToList();

            //COLOCAR FECHA DE INICIO Y FECHA FIN DE CADA SEMANA, TENIENDO EN CUENTA LOS DIAS DEL MES   

            if (año == 0 || mes == 0)
            {
                var hoy = DateTime.Today;
                año = hoy.Year;
                mes = hoy.Month;
            }
            //CREAMOS FECHA DE REFERENCIA
            var fechaReferencia = new DateTime(año, mes, 1);
            var diasDelMes = Enumerable.Range(1, fechaReferencia.AddMonths(1).AddDays(-1).Day); //ARREGLO CON NUMERO DEL 1 AL 28, FEBRERO, ETC

            var diasSegmentados = diasDelMes.Chunk(7).ToList(); //SEGMENTAMOS LOS DIAS DEL MES DE 7 EN 7 

            for (int i = 0; i < diasSegmentados.Count; i++)
            {
                var semana = i + 1;
                var fechaInicio = new DateTime(año, mes, diasSegmentados[i].First());
                var fechaFin = new DateTime(año, mes, diasSegmentados[i].Last()); //INICIO Y FIN DE CADA SEMANA 
                var grupoSemana = agrupado.FirstOrDefault(x => x.Semana == semana); //MOSTRAR SEMANAS EN LAS QUE NO RETORNE TRANSACCIONES

                if (grupoSemana is null)
                {
                    agrupado.Add(new ResultadoObtenerPorSemana()
                    {
                        Semana = semana,
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin
                    });
                }
                else
                {
                    grupoSemana.FechaInicio = fechaInicio;
                    grupoSemana.FechaFin = fechaFin;
                }
            }

            //ORGANIZAMOS POR SEMANA DE MANERA DESCENDENTE
            agrupado = agrupado.OrderByDescending( x=> x.Semana ).ToList();


            var modelo = new ReporteSemanalViewModel();
            modelo.TransaccionesPorSemana = agrupado;
            modelo.FechaReferencia = fechaReferencia;

            return View(modelo);
        }

        //VISTA DE REPORTE MENSUAL
        public async Task<IActionResult> Mensual(int año)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            if(año == 0)
            {
                año = DateTime.Today.Year;
            }
            var transaccionesPorMes = await repositorioTransacciones.ObtenerPorMes(usuarioId, año);

            var transaccionesAgrupadas = transaccionesPorMes.GroupBy(x => x.Mes).Select(x=> new ResultadoObtenerPorMes()
            {
                Mes = x.Key,
                Ingreso = x.Where(x=> x.TipoOperacionId == TipoOperacion.Ingreso).Select(x => x.Monto).FirstOrDefault(),
                Gasto = x.Where(x => x.TipoOperacionId == TipoOperacion.Gasto).Select(x => x.Monto).FirstOrDefault()
            }).ToList();

            for (int mes = 1; mes <= 12; mes++)  //CONTAMOS LOS MESES DEL 1 AL 12
            {
                var transaccion = transaccionesAgrupadas.FirstOrDefault(x => x.Mes == mes);
                var fechaReferencia = new DateTime(año, mes, 1); //DEJAMOS EL DIA 1 DE CADA MES COMO REFERENCIA

                if(transaccion is null)
                {
                    transaccionesAgrupadas.Add(new ResultadoObtenerPorMes()
                    {
                        Mes = mes,
                        FechaReferencia = fechaReferencia
                    });
                }
                else
                {
                    transaccion.FechaReferencia = fechaReferencia;
                }
            }
            transaccionesAgrupadas = transaccionesAgrupadas.OrderByDescending(x => x.Mes).ToList();

            var modelo = new ReporteMensualViewModel();
            modelo.Año = año;
            modelo.TransaccionesPorMes = transaccionesAgrupadas;

            return View(modelo);
        }

        //VISTA DE REPORTE ExcelReporte
        public IActionResult ExcelReporte()
        {

            return View();
        }

        //METODO PARA GENERAL EXCEL POR MES
        [HttpGet]

        public async Task<FileResult> ExportarExcelPorMes(int mes, int año) //LLAMOS TAREA FILERESULT PARA DESCARGAR
        {
            var fechaInicio = new DateTime(año, mes, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(new ParametroObtenerTransaccionesPorUsuario
            {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });

            var nombreArchivo = $"Informe Manejo Presupuesto - {fechaInicio.ToString("MMM yyyy")}.xlsx";  //DEFINIMOS ACCION DEL EXCEL
            return GenerarExcel(nombreArchivo, transacciones);
        }

        //EXPORTAMOS EXCEL POR AÑO

        [HttpGet]
        public async Task<FileResult> ExportarExcelPorAño(int año)
        {
            var fechaInicio = new DateTime(año, 1, 1);
            var fechaFin = fechaInicio.AddYears(1).AddDays(-1);
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(new ParametroObtenerTransaccionesPorUsuario
            {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });

            var nombreArchivo = $"Informe Manejo Presupuesto - - {fechaInicio.ToString("yyyy")}.xlsx";
            return GenerarExcel(nombreArchivo, transacciones);
        }


        //ACCION DE TODO POR EXCEL
        [HttpGet]

        public async Task<FileResult> ExportarExcelTodo()
        {
            var fechaInicio = DateTime.Today.AddYears(-100);
            var fechaFin = DateTime.Today.AddYears(1000);
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();


            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(new ParametroObtenerTransaccionesPorUsuario
            {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });

            var nombreArchivo = $"Informe Manejo Presupuesto - - {DateTime.Today.ToString("dd-MM-yyyy")}.xlsx";

            return GenerarExcel(nombreArchivo, transacciones);
        }


        //GENERAMOS ARCHIVOS DE EXCEL

        private FileResult GenerarExcel(string nombreArchivo, IEnumerable<Transaccion> transacciones)
        {
            DataTable dataTable = new DataTable("Transacciones");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Fecha"), //DEFINIMOS LAS COLUMNAS DE NUESTRO ARCHI EXCEL DESCARGABLE
                new DataColumn("Cuenta"),
                new DataColumn("Categoria"),
                new DataColumn("Nota"),
                new DataColumn("Monto"),
                new DataColumn("Ingreso/Gasto"),
            });

            foreach (var transaccion in transacciones)
            {
                dataTable.Rows.Add(transaccion.FechaTransaccion,
                                    transaccion.Cuenta,
                                    transaccion.Categoria,
                                    transaccion.Nota,
                                    transaccion.Monto,
                                    transaccion.TipoOperacionId
                                    );
            }
            //METODO QUE NEGERA NUESTRO ARCHI DE EXCEL 
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);

                using(MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
                }
            }
        }


        //VISTA DE REPORTE Calendario
        public IActionResult Calendario()
        {
            return View();
        }

        //ACCION PARA INSERTAR NUESTRAS TRANSACCIONES DE DB EN EL CALENDARIO

        public async Task<JsonResult> ObtenerTransaccionesCalendario(DateTime start,  DateTime end)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(new ParametroObtenerTransaccionesPorUsuario
            {
                UsuarioId = usuarioId,
                FechaInicio = start.Date,
                FechaFin = end.Date
            });

            var eventosCalendario =  transacciones.Select(transaccion => new EventoCalendario()
            {
                Title = transaccion.Monto.ToString("N"),
                Start = transaccion.FechaTransaccion.ToString("yyyy-MM-dd"),
                End = transaccion.FechaTransaccion.ToString("yyyy-MM-dd"),
                Color = (transaccion.TipoOperacionId == TipoOperacion.Gasto) ? "Red": null
            });

            return Json(eventosCalendario);
        }

        //METODO PARA MOSTRAR INFORMACION CON CLICK

        public async Task<JsonResult> ObtenerTransaccionesPorFecha(DateTime fecha)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(new ParametroObtenerTransaccionesPorUsuario
            {
                UsuarioId = usuarioId,
                FechaInicio = fecha,
                FechaFin = fecha
            });

            return Json(transacciones);

        }
        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var modelo = new TransaccionCreacionViewModel();
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId); //Opcion de categorias en formulario de Transacciones
            return View(modelo);

        }


        [HttpPost]

        public async Task<IActionResult> Crear(TransaccionCreacionViewModel modelo)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                return View(modelo);

            }

            var cuenta = await repositorioCuentas.ObtenerPorId(modelo.CuentaId, usuarioId);

            if (cuenta == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await repositorioCategorias.ObtenerPorId(modelo.CategoriaId, usuarioId);

            if(categoria == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            modelo.UsuarioId = usuarioId;

            if (modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                modelo.Monto *= -1;
            }

            await repositorioTransacciones.Crear(modelo);
            return RedirectToAction("Index");

        }

        //Metodos de Actualizar transacciones

        [HttpGet]

        public async Task<IActionResult> Editar(int id, string urlRetorno = null)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var transaccion = await repositorioTransacciones.ObtenerPorId(id, usuarioId);

            if (transaccion is null)
                {
                    return RedirectToAction("NoEncontrado", "Home");
                }

            var modelo = mapper.Map<TransaccionActualizacionViewModel>(transaccion);

            //Si es un ingreso

            modelo.MontoAnterior = (int)(modelo.Monto * -1);



            //Si es un gasto 
            if (modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                modelo.MontoAnterior = (int)(modelo.Monto * -1); /////////////////////////////////////////////////////
            
            }

            modelo.CuentaAnteriorId = transaccion.CuentaId;
            modelo.Categorias = await ObtenerCategorias(usuarioId, transaccion.TipoOperacionId);
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.urlRetorno = urlRetorno;

            return View(modelo);
        }



        ///Metodo para editar la transaccion 

        [HttpPost]

        public async Task<IActionResult> Editar(TransaccionActualizacionViewModel modelo)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                return View(modelo);
            }

            var cuenta = await repositorioCuentas.ObtenerPorId(modelo.CuentaId, usuarioId);

            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }


            //Aplicamos el mismo procedimiento pero para categoria

            var categoria = await repositorioCategorias.ObtenerPorId(modelo.CategoriaId, usuarioId);

            if(categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            //Mepamos de transaccion a ActualizacionViewModel
            //RETORNAMOS A LA VISTA DONDE NOS ENCONTRABAMOS

            var transaccion = mapper.Map<Transaccion>(modelo);

            if (modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                transaccion.Monto *= -1;
            }

            await repositorioTransacciones.Actualizar(transaccion, modelo.MontoAnterior, modelo.CuentaAnteriorId);

            if (string.IsNullOrEmpty(modelo.urlRetorno)) 
            {
                return RedirectToAction("Index");

            }
            else
            {
                return LocalRedirect(modelo.urlRetorno);
            }            

        }
        //Accion para permitir al usuario Borrar la transaccion en la pantalla de editar transacciones
        [HttpPost]

        public async Task<IActionResult> Borrar(int id, string urlRetorno = null)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var transaccion = await repositorioTransacciones.ObtenerPorId(id, usuarioId);

            if (transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            //RETORNAMOS A LA VISTA DONDE NOS ENCONTRABAMOS
            await repositorioTransacciones.Borrar(id);

            if (string.IsNullOrEmpty(urlRetorno))
            {
                return RedirectToAction("Index");

            }
            else
            {
                return LocalRedirect(urlRetorno);
            }
        }



      ///

        private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioId)
        {
            var cuentas = await repositorioCuentas.Buscar(usuarioId);
            return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        //Acción que permite desglosar categorias segun la respuesta de usuario

        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int usuarioId, TipoOperacion tipoOperacion)
        {
            var categorias = await repositorioCategorias.Obtener(usuarioId, tipoOperacion);
            return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        [HttpPost]

        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var categorias = await ObtenerCategorias(usuarioId, tipoOperacion);
            return Ok(categorias); //Devolvemos respuesta exitosa, en este caso sería el listado de categorias 

        }

    }
}
