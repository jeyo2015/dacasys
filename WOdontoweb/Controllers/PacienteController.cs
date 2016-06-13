namespace WOdontoweb.Controllers
{
    using System.Web.Mvc;
    using Herramientas;
    using Models;
    using NConsulta;
    using NLogin;

    public class PacienteController : Controller
    {
        public JsonResult ObtenerClientesPorEmpresa(int idConsultorio)
        {
            var result = ABMPaciente.ObtenerClientesPorEmpresa(idConsultorio);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerPacientesPorCliente(string loginCliente)
        {
            var result = ABMPaciente.ObtenerPacientesPorCliente(loginCliente);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarPaciente(PacienteDto pacienteDto)
        {
            var viel = ABMPaciente.Eliminar(pacienteDto.IdPaciente, pacienteDto.IsPrincipal, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = viel == true ? "Se elimino correctamente el paciente" : "No se pudo eliminar el paciente, intente de nuevo por favor.",
                Data = viel
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarNuevoPaciente(PacienteDto pacienteDto)
        {
            var insert = ABMPaciente.Insertar(pacienteDto, Session["loginusuario"].ToString());
            var message = string.Empty;
            switch (insert)
            {
                case 1:
                    message = "Se inserto correctamente el paciente";
                    break;
                case 2:
                    message = "Por favor ingrese un nombre valido";
                    break;
                case 3:
                    message = "Por favor ingrese un apellido valido";
                    break;
                case 4:
                    message = "Por favor ingrese el numero de identidad valido";
                    break;
                case 7:
                    message = "Por favor ingrese el numero de identidad valido";
                    break;
                case 5:
                    message = "Por favor ingrese la direccion valida";
                    break;
                case 6:
                    message = "Por favor ingrese tipo de sangre valida";
                    break;
                case 0:
                    message = "No se pudo registrar el paciente, intente de nuevo por favor";
                    break;
            }

            var result = new ResponseModel()
            {
                Message = message,
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ModificarPaciente(PacienteDto pacienteDto)
        {
            var insert = ABMPaciente.Modificar(pacienteDto, Session["loginusuario"].ToString());
            var message = string.Empty;
            switch (insert)
            {
                case 1:
                    message = "Se modifico correctamente el paciente";
                    break;
                case 2:
                    message = "Por favor ingrese un nombre valido";
                    break;
                case 3:
                    message = "Por favor ingrese un apellido valido";
                    break;
                case 4:
                    message = "Por favor ingrese el numero de identidad valido";
                    break;
                case 7:
                    message = "Por favor ingrese el numero de identidad valido";
                    break;
                case 5:
                    message = "Por favor ingrese la direccion valida";
                    break;
                case 6:
                    message = "Por favor ingrese tipo de sangre valida";
                    break;
                case 0:
                    message = "No se pudo modificar el paciente, intente de nuevo por favor";
                    break;
            }

            var result = new ResponseModel()
            {
                Message = message,
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarNuevoComentario(ComentarioDto comentarioDto)
        {
            var insert = ABMComentario.Insertar(comentarioDto, Session["loginusuario"].ToString());
            var message = string.Empty;
            switch (insert)
            {
                case 1:
                    message = "Se inserto correctamente el comentario";
                    break;                
            }

            var result = new ResponseModel()
            {
                Message = message,
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerComentariosPorEmpresa(int idConsultorio)
        {
            var result = ABMComentario.ObtenerComentarios(idConsultorio);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}