namespace WOdontoweb.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Herramientas;
    using Models;
    using NLogin;

    public class RolesController : Controller
    {
        public JsonResult GetAllRolOfClinic(int idClinic)
        {
            var result = ABMRol.Get_Roles(idClinic);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetModulos(int pidrol)
        {
            var result = ABMRol.GetModulos(pidrol);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarRol(int idRol)
        {
            var viel = ABMRol.Eliminar(idRol, Session["loginusuario"].ToString());
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

        public JsonResult InsertarNuevoRol(string nombreRol, int pIdConsultorio)
        {
            var insert = ABMRol.Insertar(nombreRol, pIdConsultorio, Session["loginusuario"].ToString(), 0);
            var result = new ResponseModel()
            {
                Message = insert == 2 ? "No se pudo insertar el rol" : "Se inserto correctamente el Rol",
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ModificarPermisos(List<ModulosTree> modulos, int pIDRol)
        {
            var insert = ABMRol.ModificarPermisos(modulos, pIDRol);
            var result = new ResponseModel()
            {
                Message = insert ? "Se guardaron los cambios" : "Hubo un error, por favor intente mas tarde",
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}