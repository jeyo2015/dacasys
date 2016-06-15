namespace WOdontoweb.Controllers
{
    using System;
    using System.Web.Mvc;
    using Models;
    using NLogin;

    public class LoginController : Controller
    {
        public JsonResult GetSessionDto()
        {
            var sessionDto = new Herramientas.SessionDto();
            sessionDto.loginUsuario = Session["loginusuario"] == null ? "" : Session["loginusuario"].ToString();
            sessionDto.IDConsultorio = Session["IDConsultorio"] == null ? -1 : Convert.ToInt32(Session["IDConsultorio"].ToString());
            sessionDto.IDClinica = Session["IDClinica"] == null ? -1 : Convert.ToInt32(Session["IDClinica"].ToString());
            sessionDto.Nombre = Session["NombreUser"] == null ? "" : Session["NombreUser"].ToString();
            sessionDto.IDRol = Session["IDRol"] == null ? -1 : Convert.ToInt32(Session["IDRol"].ToString());
            sessionDto.IsDacasys = Session["IsDacasys"] == null ? false : Convert.ToBoolean(Session["IsDacasys"].ToString());
            sessionDto.ChangePass = Session["changePass"] == null ? false : Convert.ToBoolean(Session["changePass"].ToString());
            return Json(sessionDto, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RenovarContrasena(bool pIsAdmin, string loginUsuario, string pPass, string ploginConsultorio)
        {
            var renew = pIsAdmin ? ABMUsuarioEmpleado.Cambiar_pass(loginUsuario, pPass, ploginConsultorio) : ABMUsuarioCliente.Cambiar_pass(loginUsuario, pPass);
            var result = new ResponseModel()
            {
                Message = renew == 1 ? "Se actualizo su contrasena" : "No se pudo actualizar su contrasena",
                Data = renew
            };
            Session["changePass"] = renew == 0;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CerrarSesion()
        {
            Session["loginusuario"] = null;
            Session["IDConsultorio"] = null;
            Session["IDClinica"] = null;
            Session["NombreUser"] = null;
            Session["IsDacasys"] = null;
            Session["changePass"] = null;
            Session["IDRol"] = null;
            return GetSessionDto();
        }

        public JsonResult ForgotPass(string loginEmpresa, string loginUsuario)
        {
            var message = "";
            var userRenewPass = -1;
            if (loginEmpresa != "")//Si es admin?
            {

                userRenewPass = ABMUsuarioEmpleado.Resetear_Contrasena(loginUsuario, loginEmpresa);
                switch (userRenewPass)
                {
                    case 0:
                        message = "El consultorio no existe o esta desactivada";

                        break;
                    case 1:

                        message = "El usuario no existe";
                        break;
                    case 2:
                        message = "No se pudo Resetear Constrasena";
                        break;
                    case 3:
                        message = "Se le envió un mensaje al Email del Administrador del consultorio.";
                        break;
                }
            }
            else
            {//si es cliente
                userRenewPass = ABMUsuarioCliente.ResetearPass(loginUsuario);
                switch (userRenewPass)
                {
                    case 1:
                        message = "Usuario " + loginUsuario + " no existe";
                        break;
                    case 2:
                        message = "No se pudo resetear su contrasena";
                        break;
                    case 3:
                        message = "Se envio su nueva contrasena a su email";
                        break;
                }
            }
            var result = new ResponseModel()
            {
                Message = message,
                Data = userRenewPass
            };
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Ingresar(string nameEmpresa, string usuario, string pass)
        {
            var message = "";
            var sessionDto = new Herramientas.SessionDto();
            if (nameEmpresa != "")//Si es admin?
            {
                sessionDto = ABMUsuarioEmpleado.verificar_empleado(usuario.Trim(), pass.Trim(), nameEmpresa.Trim().ToUpper());//verifica al empleado
                switch (sessionDto.Verificar)
                {
                    case 0:
                        message = "Consultorio no existe o esta desactivado";
                        sessionDto.IDClinica = -1;
                        sessionDto.IDConsultorio = -1;
                        sessionDto.IDRol = -1;
                        break;
                    case 1:
                        message = "Su licencia vencio, algunas funciones están deshabilitada";
                        sessionDto.IDClinica = -1;
                        sessionDto.IDConsultorio = -1;
                        sessionDto.IDRol = -1;
                        break;
                    case 2:
                        message = "Usuario " + usuario + " no existe";
                        sessionDto.IDClinica = -1;
                        sessionDto.IDConsultorio = -1;
                        sessionDto.IDRol = -1;
                        break;
                    case 3:
                        message = "Bienvenido a Odontoweb";
                        Session["loginusuario"] = usuario;
                        Session["IDConsultorio"] = sessionDto.IDConsultorio;
                        Session["IDClinica"] = sessionDto.IDClinica;
                        Session["NombreUser"] = sessionDto.Nombre;
                        Session["IsDacasys"] = sessionDto.IsDacasys.ToString();
                        Session["changePass"] = sessionDto.ChangePass;
                        Session["IDRol"] = sessionDto.IDRol;
                        break;
                    case 4:
                        message = "Contrasena incorrecta";
                         sessionDto.IDClinica = -1;
                        sessionDto.IDConsultorio = -1;
                        sessionDto.IDRol = -1;
                        break;
                }
            }
            else
            {//si es cliente
                sessionDto = ABMUsuarioCliente.verificar_cliente(usuario, pass);//Verifica al cliente
                switch (sessionDto.Verificar)
                {
                    case 2:
                        message = "Usuario " + usuario + " no existe";
                        sessionDto.IDClinica = -1;
                        sessionDto.IDConsultorio = -1;
                        sessionDto.IDRol = -1;
                        break;
                    case 4:
                        message = "Contrasena incorrecta";
                        sessionDto.IDClinica = -1;
                        sessionDto.IDConsultorio = -1;
                        sessionDto.IDRol = -1;
                        break;
                    case 3:
                        Session["loginusuario"] = usuario;
                        Session["IDConsultorio"] = -1;
                        Session["IDClinica"] = sessionDto.IDClinica;
                        Session["NombreUser"] = sessionDto.Nombre;
                        Session["IsDacasys"] = sessionDto.IsDacasys.ToString();
                        message = "Bienvenido a Odontoweb";
                        break;
                }
            }
            var result = new ResponseModel()
            {
                Message = message,
                Data = sessionDto
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}