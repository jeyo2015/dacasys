using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Herramientas;
using NLogin;
using RMTools.UI.Models;
namespace WOdontoweb.Controllers
{
    public class NotificacionConsultorioController : Controller
    {
        //
        // GET: /Login/

        #region Variables
        private readonly ABMNotificacionesConsultorio gABMNotificacionesConsultorio;

        #endregion

        #region Constructor
        public NotificacionConsultorioController()
        {
            gABMNotificacionesConsultorio = new ABMNotificacionesConsultorio();
        }
        #endregion

        public JsonResult GetSolicitudesPacientes(int pIdConsultorio, int pIDTipoNotificacion)
        {
            var result = gABMNotificacionesConsultorio.GetNotificacionesPendientes(pIdConsultorio, pIDTipoNotificacion);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeshabilitarNuevasNotificaciones(int pIdConsultorio, int pIDTipoNotificacion)
        {
            var success = gABMNotificacionesConsultorio.DeshabilitarNuevasNotificaciones(pIdConsultorio, pIDTipoNotificacion);
            var result = new ResponseModel()
            {
                Message = "",
                Data = success?gABMNotificacionesConsultorio.GetNotificacionesPendientes(pIdConsultorio, pIDTipoNotificacion):null,
                Success = success
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AceptarSolicitudPaciente(NotificacionesConsultorioDto pNotificacion)
        {
            var success = gABMNotificacionesConsultorio.AceptarSolicitudPaciente(pNotificacion);
            var result = new ResponseModel()
            {
                Message = "",
                Data = success ? gABMNotificacionesConsultorio.GetNotificacionesPendientes(pNotificacion.IDConsultorio,pNotificacion.IDNotificacion)  : null,
                Success = success
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CancelarSolicitudPaciente(NotificacionesConsultorioDto pNotificacion)
        {
            var success = gABMNotificacionesConsultorio.CancelarSolicitudPaciente(pNotificacion);
            var result = new ResponseModel()
            {
                Message = "",
                Data = success ? gABMNotificacionesConsultorio.GetNotificacionesPendientes(pNotificacion.IDConsultorio, pNotificacion.IDNotificacion) : null,
                Success = success
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
