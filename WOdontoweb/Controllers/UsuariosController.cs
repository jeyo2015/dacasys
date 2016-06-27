namespace WOdontoweb.Controllers
{
    using System.Web.Mvc;
    using Herramientas;
    using Models;
    using NLogin;

    public class UsuariosController : Controller
    {
        public JsonResult GetUsersOfClinic(int idClinic)
        {
            var result = ABMUsuarioEmpleado.ObtenerListaUsuario(idClinic);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuarioConsultorio(string pLogin, int pIDEmpresa)
        {
            var result = ABMUsuarioEmpleado.ObtenerUsuario(pLogin, pIDEmpresa);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarUsuario(UsuarioDto pUsuario)
        {
            var viel = ABMUsuarioEmpleado.Eliminar(pUsuario.Login, pUsuario.IDEmpresa, Session["loginusuario"].ToString());
            var result = new ResponseModel()
          {
              Message = viel == 1 ? "Se elimino correctamente el usuario" : "No se pudo eliminar el usuario, intente de nuevo por favor.",
              Data = viel
          };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarNuevoUsuario(UsuarioDto pUsuario)
        {
            var insert = ABMUsuarioEmpleado.Insertar(pUsuario.Nombre, pUsuario.Login, pUsuario.IDEmpresa, pUsuario.IDRol,
                pUsuario.Password, pUsuario.ConfirmPass, pUsuario.changepass, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se inserto correctamente el usuario" : "No se pudo insertar el usuario, intente de nuevo por favor",
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ModificarUsuario(UsuarioDto pUsuario)
        {
            var insert = ABMUsuarioEmpleado.Modificar(pUsuario.Nombre, pUsuario.Login, pUsuario.IDEmpresa, pUsuario.IDRol,
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