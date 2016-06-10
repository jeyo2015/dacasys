namespace WOdontoweb.Controllers
{
    using System.Web.Mvc;
    using Herramientas;
    using Models;
    using NLogin;

    public class NotificacionConsultorioController : Controller
    {
        public JsonResult GetSolicitudesPacientes(int pIdConsultorio, int pIDTipoNotificacion)
        {
            var result = ABMNotificacionesConsultorio.GetNotificacionesPendientes(pIdConsultorio, pIDTipoNotificacion);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeshabilitarNuevasNotificaciones(int pIdConsultorio, int pIDTipoNotificacion)
        {
            var success = ABMNotificacionesConsultorio.DeshabilitarNuevasNotificaciones(pIdConsultorio, pIDTipoNotificacion);
            var result = new ResponseModel()
            {
                Message = string.Empty,
                Data = success ? ABMNotificacionesConsultorio.GetNotificacionesPendientes(pIdConsultorio, pIDTipoNotificacion) : null,
                Success = success
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AceptarSolicitudPaciente(NotificacionesConsultorioDto pNotificacion)
        {
            var success = ABMNotificacionesConsultorio.AceptarSolicitudPaciente(pNotificacion);
            var result = new ResponseModel()
            {
                Message = string.Empty,
                Data = success ? ABMNotificacionesConsultorio.GetNotificacionesPendientes(pNotificacion.IDConsultorio, pNotificacion.IDNotificacion) : null,
                Success = success
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelarSolicitudPaciente(NotificacionesConsultorioDto pNotificacion)
        {
            var success = ABMNotificacionesConsultorio.CancelarSolicitudPaciente(pNotificacion);
            var result = new ResponseModel()
            {
                Message = string.Empty,
                Data = success ? ABMNotificacionesConsultorio.GetNotificacionesPendientes(pNotificacion.IDConsultorio, pNotificacion.IDNotificacion) : null,
                Success = success
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}