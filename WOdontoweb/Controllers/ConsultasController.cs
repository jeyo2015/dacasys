using System.Web.Mvc;
using NLogin;
using Herramientas;
using RMTools.UI.Models;
using NAgenda;
using System;
using NConsulta;
namespace WOdontoweb.Controllers
{
    public class ConsultasController : Controller
    {
        //
        // GET: /Login/

        #region Variables

        private readonly ABMCita gABMCita;
        private readonly ABMHistorico gABMHistorico;
        private readonly ABMHistoricoDet gABMHistoricoDetalle;
        #endregion

        #region Constructor

        public ConsultasController()
        {
            gABMCita = new ABMCita();
            gABMHistorico = new ABMHistorico();
            gABMHistoricoDetalle = new ABMHistoricoDet();
        }

        #endregion

        public JsonResult GetCitasDelDia(DateTime pfecha, int pIdConsultorio, int ptiempoConsulta)
        {
            var result = gABMCita.GetAgendaDelDia(pfecha, pIdConsultorio, ptiempoConsulta);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHistoricoPaciente(int pIdPaciente, int pIdConsultorio)
        {
            var result = gABMHistorico.GetHistoricoPaciente(pIdPaciente, pIdConsultorio);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetHistoricoDetalle(HistoricoPacienteDto pHistorico)
        {
            var result = gABMHistoricoDetalle.GetHistoricoDetalle(pHistorico.IdConsultorio, pHistorico.IdPaciente, pHistorico.NumeroHistorico);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InsertarCitaPaciente(AgendaDto pcita, DateTime pFecha, string pIdCliente)
        {
            var insert = gABMCita.InsertarCita(pcita, pIdCliente, pFecha, Session["loginusuario"].ToString()); ;
            var result = new ResponseModel()
            {
                Message = insert ? "Se agendo correctamente la cita" : "No se pudo agendar la cita, por favor intente de nuevo",
                Data = insert,
                Success = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ABMHistorico(HistoricoPacienteDto pHistorico)
        {
            pHistorico.FechaCreacion = DateTime.Now;
            var insert = gABMHistorico.ABMHistoricoMetodo(pHistorico, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert ? "Se registro el nuevo historico" : "No se pudo registrar el nuevo historico, por favor intente de nuevo",
                Data = insert,
                Success = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EliminarCitaPaciente(AgendaDto pcita, bool pLibre, string pMotivo)
        {
            var insert = gABMCita.EliminarCita(pcita, Session["loginusuario"].ToString(), pLibre, pMotivo);
            var vMessage = "";
            switch (insert) { 
                case 1:
                    vMessage = "Se cancelo la cita correctamente";
                    break;
                case 2 :
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
                Success = insert== 1
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}