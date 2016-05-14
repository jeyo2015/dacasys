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
       

    }
}
