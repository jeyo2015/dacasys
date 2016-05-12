using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLogin;
using Herramientas;
using RMTools.UI.Models;
namespace WOdontoweb.Controllers
{
    public class UsuariosController : Controller
    {
        //
        // GET: /Login/

        #region Variables
        private readonly ABMUsuarioEmpleado gABMIUsuarioEmpleado;
        private readonly ABMRol gABMRol;
        #endregion

        #region Constructor
        public UsuariosController()
        {
            gABMRol = new ABMRol();
            gABMIUsuarioEmpleado = new ABMUsuarioEmpleado();
        }
        #endregion

        public JsonResult GetUsersOfClinic(int idClinic)
        {
            var result = gABMIUsuarioEmpleado.Get_Usuarios(idClinic);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioConsultorio(string pLogin, int pIDEmpresa)
        {
            var result = gABMIUsuarioEmpleado.Get_Usuario(pLogin,pIDEmpresa);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EliminarUsuario(UsuarioDto pUsuario)
        {
            var viel = gABMIUsuarioEmpleado.Eliminar(pUsuario.Login, pUsuario.IDEmpresa,Session["loginusuario"].ToString());
            var result = new ResponseModel()
          {
              Message = viel == 1 ? "Se elimino correctamente el usuario" : "No se pudo eliminar el usuario, intente de nuevo por favor.",
              Data = viel
          };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarNuevoUsuario(UsuarioDto pUsuario)
        {
            var insert = gABMIUsuarioEmpleado.Insertar(pUsuario.Nombre, pUsuario.Login, pUsuario.IDEmpresa, pUsuario.IDRol,
                pUsuario.Password, pUsuario.ConfirmPass, pUsuario.changepass, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == 1 ?  "Se inserto correctamente el usuario":"No se pudo insertar el usuario, intente de nuevo por favor",
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ModificarUsuario(UsuarioDto pUsuario)
        {
            var insert = gABMIUsuarioEmpleado.Modificar(pUsuario.Nombre, pUsuario.Login, pUsuario.IDEmpresa, pUsuario.IDRol,
                pUsuario.Password, pUsuario.ConfirmPass, pUsuario.changepass, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se modifico correctamente el usuario" : "No se pudo modificar el usuario, intente de nuevo por favor",
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
