namespace WOdontoweb.Controllers
{
    using System;
    using System.Web.Mvc;
    using Herramientas;
    using Models;
    using NAgenda;
    using NConsulta;

    public class ConsultasController : Controller
    {
        public JsonResult GetCitasDelDia(DateTime pfecha, int pIdConsultorio, int ptiempoConsulta)
        {
            var result = ABMCita.GetAgendaDelDia(pfecha, pIdConsultorio, ptiempoConsulta);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHistoricoPaciente(int pIdPaciente, int pIdConsultorio)
        {
            var result = ABMHistorico.GetHistoricoPaciente(pIdPaciente, pIdConsultorio);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHistoricoDetalle(HistoricoPacienteDto pHistorico)
        {
            var result = ABMHistoricoDet.GetHistoricoDetalle(pHistorico.IdConsultorio, pHistorico.IdPaciente, pHistorico.NumeroHistorico);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarCitaPaciente(AgendaDto pcita, DateTime pFecha, string pIdCliente)
        {
            var insert = ABMCita.InsertarCita(pcita, pIdCliente, pFecha, Session["loginusuario"].ToString()); ;
            var result = new ResponseModel()
            {
                Message = insert ? "Se agendo correctamente la cita" : "No se pudo agendar la cita, por favor intente de nuevo",
                Data = insert,
                Success = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GuardarHistorico(HistoricoPacienteDto pHistorico)
        {
            pHistorico.FechaCreacion = DateTime.Now;
            var insert = ABMHistorico.ABMHistoricoMetodo(pHistorico, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert ? "Se registro el nuevo historico" : "No se pudo registrar el nuevo historico, por favor intente de nuevo",
                Data = insert,
                Success = insert
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
    }
}