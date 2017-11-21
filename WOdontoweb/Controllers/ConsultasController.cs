namespace WOdontoweb.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Herramientas;
    using Models;
    using NAgenda;
    using NConsulta;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class ConsultasController : Controller
    {
        public JsonResult GetCitasDelDia(string pfecha, int pIdConsultorio, int ptiempoConsulta)
        {
            var splitFecha = pfecha.Split('/');
            var result = ABMCita.ObtenerAgendaDelDia(new DateTime(Convert.ToInt16(splitFecha[2]), Convert.ToInt16(splitFecha[1]), Convert.ToInt16(splitFecha[0])), pIdConsultorio, ptiempoConsulta);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObtenerHorarioParaMostrarMapa(int pIdConsultorio)
        {

            var result = ABMHorario.ObtenerHorarioParaMostrarMapa(pIdConsultorio);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult ObtenerHistoricoConFichas(int pIdPaciente, int pIdConsultorio, string citaSeleccionada)
        //{
        //    Session["IDPacienteSeleccionado"] = pIdPaciente;
        //    Session["IDCitaSeleccionada"] = citaSeleccionada;
        //    var result = ABMHistoricoOdontograma.ObtenerHistoricoPaciente(pIdConsultorio, pIdPaciente);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult ObtenerHistoricoFichaDetalle(int pIdHistoricoFichaCab)
        {
            var result = ABMHistoricoOdontograma.ObtenerHistoricoFichaDetalle(pIdHistoricoFichaCab);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerHistoricoFichaCabs(int idPaciente, int idConsultorio, string citaSeleccionada)
        {
            Session["IDPacienteSeleccionado"] = idPaciente;
               Session["IDCitaSeleccionada"] = citaSeleccionada;
            var result = ABMHistoricoOdontograma.ObtenerHistoricoFichaCabs(idPaciente, idConsultorio);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerHistoricoFichaTrabajo(int pIdHistoricoFichaCab)
        {
            var result = ABMHistoricoOdontograma.ObtenerHistoricoFichaTrabajo(pIdHistoricoFichaCab);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObtenerCriterios()
        {
            var result = ABMHistoricoOdontograma.ObtenerCriterios();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObtenerValoresPeriodontal()
        {
            var result = ABMHistoricoOdontograma.ObtenerValoresPeriodontal();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObtenerHistoricoFichaCab(int pIdHistoricoFichaCab)
        {
            Session["IDHistorico"] = pIdHistoricoFichaCab;
            var result = ABMHistoricoOdontograma.ObtenerHistoricoFichaCab(pIdHistoricoFichaCab);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetHistoricoPaciente(int pIdPaciente, int pIdConsultorio)
        {
            var result = ABMHistorico.ObtenerHistoricoPaciente(pIdPaciente, pIdConsultorio);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult VerificarClienteEnConsultorio(int pIdConsultorio, string pLoginCliente)
        {
            var result = ABMCita.VerificarClienteEnConsultorio(pIdConsultorio, pLoginCliente);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHistoricoDetalle(HistoricoPacienteDto pHistorico)
        {
            var result = ABMHistoricoDet.ObtenerHistoricoDetalle(pHistorico.IdConsultorio, pHistorico.IdPaciente, pHistorico.NumeroHistorico);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarCitaPaciente(AgendaDto pcita, string pFechastring, string pIdCliente)
        {
            var splitFecha = pFechastring.Split('/');
            var pFecha = new DateTime(Convert.ToInt16(splitFecha[2]), Convert.ToInt16(splitFecha[1]),
                Convert.ToInt16(splitFecha[0]));
            var insert = ABMCita.InsertarCita(pcita, pIdCliente, pFecha, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert ? "Se agendo correctamente la cita" : "No se pudo agendar la cita, por favor intente de nuevo",
                Data = insert,
                Success = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult HabilitarHora(AgendaDto pcita)
        {

            var insert = ABMCita.HabilitarHora(pcita, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se habilito el horario correctamente" : "No se pudo habilitar la hora, por favor intente de nuevo",
                Data = insert,
                Success = insert == 1
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GuardarHistorico(HistoricoPacienteDto pHistorico)
        {
            pHistorico.FechaCreacion = DateTime.Now;
            var insert = ABMHistorico.GuadrarHistorico(pHistorico, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert ? "Se registro el nuevo historico" : "No se pudo registrar el nuevo historico, por favor intente de nuevo",
                Data = insert,
                Success = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CerrarFichaHistorico(int idFichaHistorico, bool citaAtendida)
        {
           
            var insert = ABMHistoricoOdontograma.CerrarFichaHistorico(idFichaHistorico,citaAtendida, Session["IDCitaSeleccionada"].ToString(), Session["loginusuario"].ToString());
            if (citaAtendida && insert)
                Session["IDCitaSeleccionada"] = "";
          
            var result = new ResponseModel()
            {
                Message = insert ? "Se cerro la ficha seleccionada" : "No se pudo cerrar la ficha seleccionada, por favor intente de nuevo",
                Data = insert,
                Success = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GuardarHistoricoCompleto(string pHistoricoFichaString)
        {
            HistoricoFichaCabDto pHistoricoFicha = JsonConvert.DeserializeObject<HistoricoFichaCabDto>(pHistoricoFichaString, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
            var insert = ABMHistoricoOdontograma.GuardarHistoricoCompleto(pHistoricoFicha, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert ? "Se registro los cambios de la ficha" : "No se pudo registrar los cambios, por favor intente de nuevo",
                Data = insert,
                Success = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GuardarHistoricoFichaDetalle(string fichaDetalle, bool citaAtendida)
        {
            List<HistoricoFichaDetalleDto> detalles = JsonConvert.DeserializeObject<List<HistoricoFichaDetalleDto>>(fichaDetalle, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
            var insert = ABMHistoricoOdontograma.GuardarTrabajosRealizadosHistorico(detalles, Session["loginusuario"].ToString(), citaAtendida, Session["IDCitaSeleccionada"].ToString());
            if (citaAtendida && insert)
                Session["IDCitaSeleccionada"] = "";
            var result = new ResponseModel()
            {
                Message = insert ? "Se registro los cambios de la ficha" : "No se pudo registrar los cambios, por favor intente de nuevo",
                Data = insert,
                Success = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GuardarHistoricoFichaTrabajo(string fichaTrabajo)
        {
            List<HistoricoFichaTrabajoDto> trabajos = JsonConvert.DeserializeObject<List<HistoricoFichaTrabajoDto>>(fichaTrabajo, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

            var insert = ABMHistoricoOdontograma.GuardarTratamientoHistorico(trabajos, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert ? "Se registro los cambios de la ficha" : "No se pudo registrar los cambios, por favor intente de nuevo",
                Data = insert,
                Success = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GuardarHistoricoPaciente(HistoricoPacienteOdontogramaDto pHistorico)
        {
            pHistorico.FechaCreacion = DateTime.Now;
            var insert = ABMHistoricoOdontograma.GuardarHistoricoPaciente(pHistorico, Session["loginusuario"].ToString());

            var result = new ResponseModel()
            {
                Message = insert ? "Se registro el nuevo historico" : "No se pudo registrar el nuevo historico, por favor intente de nuevo",
                Data = insert,
                Success = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GuardarHistoricoFichaPaciente(HistoricoFichaCabDto pfichaHistorico)
        {
            pfichaHistorico.Fecha = DateTime.Now;
            var insert = ABMHistoricoOdontograma.GuardarHistoricoFichaPaciente(pfichaHistorico, Session["loginusuario"].ToString());
            Session["IDHistorico"] = insert;
            var result = new ResponseModel()
            {
                Message = insert != -1 ? "Se registro el nuevo historico" : "No se pudo registrar el nuevo historico, por favor intente de nuevo",
                Data = insert,
                Success = insert != -1
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarHistoricoDetalle(HistoricoDetallePacienteDto pHistoricoDetalle)
        {
            var insert = ABMHistorico.InsertarHistoricoDetalle(pHistoricoDetalle, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert ? "Se registro en historico " : "No se pudo registrar en el historico, por favor intente de nuevo",
                Data = insert,
                Success = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarCitaPaciente(AgendaDto pcita, bool pLibre, string pMotivo)
        {
            var insert = ABMCita.EliminarCita(pcita, Session["loginusuario"].ToString(), pLibre, pMotivo);
            var vMessage = "";
            switch (insert)
            {
                case 1:
                    vMessage = "Se cancelo la cita correctamente";
                    break;
                case 2:
                    vMessage = "No existe cita en ese horario";
                    break;
                case 0:
                    vMessage = "No se pudo cancelar la cita, por favor intente de nuevo";
                    break;

            }
            var result = new ResponseModel()
            {
                Message = vMessage,
                Data = insert,
                Success = insert == 1
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerCitasPorCliente(string loginCliente)
        {
            var result = ABMCita.ObtenerCitasPorCliente(loginCliente);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelarCitaPaciente(string idCita)
        {
            var insert = ABMCita.CancelarCita(idCita, Session["loginusuario"].ToString());
            var vMessage = "";
            switch (insert)
            {
                case 1:
                    vMessage = "Se cancelo la cita correctamente";
                    break;
                case 2:
                    vMessage = "No existe cita en ese horario";
                    break;
                case 0:
                    vMessage = "No se pudo cancelar la cita, por favor intente de nuevo";
                    break;

            }
            var result = new ResponseModel()
            {
                Message = vMessage,
                Data = insert,
                Success = insert == 1
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}