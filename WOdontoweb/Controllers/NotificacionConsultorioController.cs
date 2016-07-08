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
            var result = ABMNotificacionesConsultorio.ObtenerNotificacionesPendientes(pIdConsultorio, pIDTipoNotificacion);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeshabilitarNuevasNotificaciones(int pIdConsultorio, int pIDTipoNotificacion)
        {
            var success = ABMNotificacionesConsultorio.DeshabilitarNuevasNotificaciones(pIdConsultorio, pIDTipoNotificacion);
            var result = new ResponseModel()
            {
                Message = string.Empty,
                Data = success ? ABMNotificacionesConsultorio.ObtenerNotificacionesPendientes(pIdConsultorio, pIDTipoNotificacion) : null,
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
                Data = success ? ABMNotificacionesConsultorio.ObtenerNotificacionesPendientes(pNotificacion.IDConsultorio, pNotificacion.IDNotificacion) : null,
                Success = success
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EnviarSolicitudConsultorio(NotificacionesConsultorioDto pNotificacion)
        {
            var success = ABMNotificacionesConsultorio.EnviarSolicitudConsultorio(pNotificacion);
            var result = new ResponseModel()
            {
                Message = success?"Se envio la solicitud":"No se pudo enviar la solicitud, por favor intente nuevamente",
                Data = success ,
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
                Data = success ? ABMNotificacionesConsultorio.ObtenerNotificacionesPendientes(pNotificacion.IDConsultorio, pNotificacion.IDNotificacion) : null,
                Success = success
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}