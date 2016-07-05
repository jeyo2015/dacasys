﻿

namespace WOdontoweb.Controllers
{
    using System;
    using System.IO;
    using System.Web;
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
            var imageByte = Session[clinicaDto.NombreArchivo];
            clinicaDto.logoImagen = (byte[])imageByte;
            var insert = ABMEmpresa.InsertarClinica(clinicaDto, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se inserto correctamente la clinica" : "No se pudo insertar la clinica, intente de nuevo por favor",
                Data = insert,
                Success = insert == 1
            };
            Session.Remove(clinicaDto.NombreArchivo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ModificarClinica(ClinicaDto clinicaDto)
        {
            var imageByte = Session[clinicaDto.NombreArchivo];
            clinicaDto.logoImagen = (byte[])imageByte;
            var insert = ABMEmpresa.ModificarClinica(clinicaDto, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se modifico correctamente la clinica" : "No se pudo modificar la clinica, intente de nuevo por favor",
                Data = insert,
                Success = insert == 1
            };
            Session.Remove(clinicaDto.NombreArchivo);
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

        public JsonResult HabilitarClinica(int idClinica)
        {
            var insert = ABMEmpresa.HabilitarClinica(idClinica, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se habilito correctamente la clinica" : "No se pudo habilitar la clinica, intente de nuevo por favor",
                Data = insert,
                Success = insert == 1
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarNuevoConsultorio(ConsultorioDto consultorioDto)
        {
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
        public JsonResult EliminarConsultorio(int idConsultorio)
        {
            var insert = ABMEmpresa.EliminarConsultorio(idConsultorio, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se deshabilito correctamente el consultorio" : "No se pudo deshabilitar el consultorio, intente de nuevo por favor",
                Data = insert,
                Success = insert == 1
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult HabilitarConsultorio(int idConsultorio)
        {
            var insert = ABMEmpresa.HabilitarConsultorio(idConsultorio, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == 1 ? "Se habilito correctamente el consultorio" : "No se pudo habilitar el consultorio, intente de nuevo por favor",
                Data = insert,
                Success = insert == 1
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerConsultoriosPorCliente(string loginCliente)
        {
            var result = ABMEmpresa.ObtenerConsultoriosPorCliente(loginCliente);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerConsultoriosPorClinica(int pIDClinica)
        {
            var result = ABMEmpresa.ObtenerConsultoriosPorClinica(pIDClinica);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ObtenerConsultorioConClinica(int pIdConsultorio)
        {
            var result = ABMEmpresa.ObtenerConsultorioConClinica(pIdConsultorio);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public virtual ActionResult UploadFiles()
        {
            var isUploaded = false;
            var message = "File upload failed";
            var nameFile = string.Empty;

            foreach (string file in Request.Files)
            {
                var fileBase = Request.Files[file] as HttpPostedFileBase;
                if (fileBase != null && fileBase.ContentLength == 0)
                    continue;

                if (fileBase == null || fileBase.ContentLength == 0) continue;
                var pathForSaving = Server.MapPath("~/Content/upload");
                if (!ABMEmpresa.CreateFolderIfNeeded(pathForSaving)) continue;
                try
                {
                    fileBase.SaveAs(Path.Combine(pathForSaving, fileBase.FileName));
                    isUploaded = true;
                    message = "File uploaded successfully!";
                    nameFile = fileBase.FileName;
                    var stream = fileBase.InputStream;
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    Session[nameFile] = buffer;
                }
                catch (Exception ex)
                {
                    message = string.Format("File upload failed: {0}", ex.Message);
                }
            }
            return Json(new { isUploaded = isUploaded, message = message, name = nameFile }, "text/html");
        }
    }
}