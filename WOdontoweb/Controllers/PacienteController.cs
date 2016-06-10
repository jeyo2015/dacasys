namespace WOdontoweb.Controllers
{
    using System.Web.Mvc;
    using Herramientas;
    using Models;
    using NConsulta;

    public class PacienteController : Controller
    {
        public JsonResult GetPacienteConsultorio( int pIdConsultorio)
        {
            var result = ABMPaciente.GetPacientesEmpresa(pIdConsultorio);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPacientesByCliente(string pLoginCliente)
        {
            var result = ABMPaciente.GetPacientesByCliente(pLoginCliente);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarPaciente(PacienteDto pacienteDto)
        {
            var viel = ABMPaciente.Eliminar(pacienteDto.IdPaciente, pacienteDto.IsPaciente, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = viel == true ? "Se elimino correctamente el horario" : "No se pudo eliminar el horario, intente de nuevo por favor.",
                Data = viel
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarNuevoHorario(PacienteDto pacienteDto)
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

        public JsonResult ModificarHorario(PacienteDto pacienteDto)
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
    }
}