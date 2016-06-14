namespace WOdontoweb.Controllers
{
    using System.Web.Mvc;
    using Herramientas;
    using Models;
    using NLogin;

    public class EmpresaController : Controller
    {
        public JsonResult ObtenerClinicas()
        {
            var result = ABMEmpresa.ObtenerClinicas();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerConsultorioPorId(int idConsultorio)
        {
            var result = ABMEmpresa.ObtenerConsultorioPorId(idConsultorio);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerClinicasHabilitadas()
        {
            var result = ABMEmpresa.ObtenerClinicasHabilitadas();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerTrabajosClinica(int idClinica)
        {
            var result = ABMEmpresa.ObtenerTrabajosClinica(idClinica);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerIntervalosDeTiempo()
        {
            var result = ABMEmpresa.ObtenerIntervalosDeTiempo();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarNuevaClinica(ClinicaDto clinicaDto)
        {
            var insert = ABMEmpresa.InsertarClinica( clinicaDto, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se inserto correctamente la clinica" : "No se pudo insertar la clinica, intente de nuevo por favor",
                Data = insert,
                Success = insert==1
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ModificarClinica(ClinicaDto clinicaDto)
        {
            var insert = ABMEmpresa.ModificarClinica(clinicaDto, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se modifico correctamente la clinica" : "No se pudo modificar la clinica, intente de nuevo por favor",
                Data = insert,
                Success = insert == 1
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarClinica(int idClinica)
        {
            var insert = ABMEmpresa.EliminarClinica(idClinica, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se deshabilito correctamente la clinica" : "No se pudo deshabilitar la clinica, intente de nuevo por favor",
                Data = insert,
                Success = insert == 1
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarNuevoConsultorio(ConsultorioDto consultorioDto) {
            consultorioDto.IDUsuarioCreador = Session["loginusuario"].ToString();
            var insert = ABMEmpresa.InsertarConsultorio(consultorioDto);
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se inserto correctamente el consultorio" : "No se pudo insertar el consultorio, intente de nuevo por favor",
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ModificarConsultorio(ConsultorioDto consultorioDto)
        {
            var insert = ABMEmpresa.ModificarConsultorio(consultorioDto, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se modifico correctamente el consultorio" : "No se pudo modificar el consultorio, intente de nuevo por favor",
                Data = insert,
                Success = insert == 1
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}