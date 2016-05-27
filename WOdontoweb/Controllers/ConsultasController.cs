using System.Web.Mvc;
using NLogin;
using Herramientas;
using RMTools.UI.Models;
using NAgenda;
using System;
namespace WOdontoweb.Controllers
{
    public class ConsultasController : Controller
    {
        //
        // GET: /Login/

        #region Variables

        private readonly ABMCita gABMCita;

        #endregion

        #region Constructor

        public ConsultasController()
        {
            gABMCita = new ABMCita();
        }

        #endregion

        public JsonResult GetCitasDelDia(DateTime pfecha, int pIdConsultorio, int ptiempoConsulta)
        {
            var result = gABMCita.GetAgendaDelDia(pfecha, pIdConsultorio, ptiempoConsulta);
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