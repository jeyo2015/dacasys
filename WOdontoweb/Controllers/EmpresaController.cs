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
    public class EmpresaController : Controller
    {
        //
        // GET: /Login/

        #region Variables
        private readonly ABMEmpresa gABMEmpresa;
        private readonly ABMRol gABMRol;
        #endregion

        #region Constructor
        public EmpresaController()
        {
            gABMEmpresa = new ABMEmpresa();
            //gABMIUsuarioEmpleado = new ABMUsuarioEmpleado();
        }
        #endregion

        public JsonResult GetAllClinicas()
        {
            var result = gABMEmpresa.GetTodasClinicas();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllClinicasHabilitadas()
        {
            var result = gABMEmpresa.GetTodasClinicasHabilitadas();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTrabajosClinica(int pIdClinica)
        {
            var result = gABMEmpresa.GetTrabajosClinica(pIdClinica);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetIntervalosTiempo()
        {
            var result = gABMEmpresa.Get_Intervalosp();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarNuevaClinica(ClinicaDto pClinica)
        {
            var insert = gABMEmpresa.InsertarClinica( pClinica, Session["loginusuario"].ToString());
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
            var insert = gABMEmpresa.ModificarClinica(pClinica, Session["loginusuario"].ToString());
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
            var insert = gABMEmpresa.EliminarClinica(pIDClinica, Session["loginusuario"].ToString());
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
            var insert = gABMEmpresa.InsertarConsultorio(pConsultorio);
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se inserto correctamente el consultorio" : "No se pudo insertar el consultorio, intente de nuevo por favor",
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
