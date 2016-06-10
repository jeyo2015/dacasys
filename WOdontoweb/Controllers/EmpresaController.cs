namespace WOdontoweb.Controllers
{
    using System.Web.Mvc;
    using Herramientas;
    using Models;
    using NLogin;

    public class EmpresaController : Controller
    {
        public JsonResult GetAllClinicas()
        {
            var result = ABMEmpresa.GetTodasClinicas();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetConsultorioByID(int pIdConsultorio)
        {
            var result = ABMEmpresa.GetConsultorioByID(pIdConsultorio);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllClinicasHabilitadas()
        {
            var result = ABMEmpresa.GetTodasClinicasHabilitadas();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTrabajosClinica(int pIdClinica)
        {
            var result = ABMEmpresa.GetTrabajosClinica(pIdClinica);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetIntervalosTiempo()
        {
            var result = ABMEmpresa.Get_Intervalosp();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarNuevaClinica(ClinicaDto pClinica)
        {
            var insert = ABMEmpresa.InsertarClinica( pClinica, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se inserto correctamente la clinica" : "No se pudo insertar la clinica, intente de nuevo por favor",
                Data = insert,
                Success = insert==1
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ModificarClinica(ClinicaDto pClinica)
        {
            var insert = ABMEmpresa.ModificarClinica(pClinica, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se modifico correctamente la clinica" : "No se pudo modificar la clinica, intente de nuevo por favor",
                Data = insert,
                Success = insert == 1
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarClinica(int pIDClinica)
        {
            var insert = ABMEmpresa.EliminarClinica(pIDClinica, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se deshabilito correctamente la clinica" : "No se pudo deshabilitar la clinica, intente de nuevo por favor",
                Data = insert,
                Success = insert == 1
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarNuevoConsultorio(ConsultorioDto pConsultorio) {
            pConsultorio.IDUsuarioCreador = Session["loginusuario"].ToString();
            var insert = ABMEmpresa.InsertarConsultorio(pConsultorio);
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se inserto correctamente el consultorio" : "No se pudo insertar el consultorio, intente de nuevo por favor",
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}