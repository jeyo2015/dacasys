using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLogin;
using RMTools.UI.Models;
namespace WOdontoweb.Controllers
{
    public class RolesController : Controller
    {
        //
        // GET: /Login/

        #region Variables
        private readonly ABMUsuarioEmpleado gABMIUsuarioEmpleado;
        private readonly ABMRol gABMRol;
        #endregion

        #region Constructor
        public RolesController()
        {
            gABMRol = new ABMRol();
            gABMIUsuarioEmpleado = new ABMUsuarioEmpleado();
        }
        #endregion

        public JsonResult GetAllRolOfClinic(int idClinic)
        {
            var result = gABMRol.Get_Roles(idClinic);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetModulos(int pidrol)
        {
            var result = gABMRol.GetModulos(pidrol);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult EliminarRol(int idRol)
        {
            var viel = gABMRol.Eliminar(idRol, Session["loginusuario"].ToString());
            var message = "";
            switch (viel)
            {
                case 1:
                    message = "Se elimino correctamente el rol";
                    break;
                case 2:
                    message = "No se pudo eliminar el Rol";
                    break;
                case 4:
                    message = "No se puede eliminar el Rol ya que existen usuarios en este rol";
                    break;

            }

            var result = new ResponseModel()
            {
                Message = message,
                Data = viel
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarNuevoRol(string nombreRol)
        {
            var insert = gABMRol.Insertar(nombreRol, 1, Session["loginusuario"].ToString(), 0);
            var result = new ResponseModel()
            {
                Message = insert == 2 ? "No se pudo insertar el rol" : "Se inserto correctamente el Rol",
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
