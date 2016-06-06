using System.Collections.Generic;

namespace WOdontoweb.Controllers
{
    using System.Web.Mvc;
using NAgenda;
using Herramientas;
using RMTools.UI.Models;

    public class HorarioController : Controller
    {
        #region Variables

        private readonly ABMHorario abmHorario;
        private readonly ABMDia abmDia;

        #endregion

        #region Constructor

        public HorarioController()
        {
            abmHorario = new ABMHorario();
            abmDia = new ABMDia();
        }

        #endregion

        public JsonResult ObtenerDias()
        {
            var result = abmDia.ObtenerDias();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerHorariosPorEmpresa(int idEmpresa)
        {
            var result = abmHorario.ObtenerHorariosPorEmpresa(idEmpresa);                
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarHorario(HorarioDto horarioDto)
        {
            var viel = abmHorario.Eliminar(horarioDto.IDHorario, horarioDto.NumHorario, Session["loginusuario"].ToString());
            var result = new ResponseModel()
          {
              Message = viel == true ? "Se elimino correctamente el horario" : "No se pudo eliminar el horario, intente de nuevo por favor.",
              Data = viel
          };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarNuevoHorario(HorarioDto horarioDto)
        {
            var insert = abmHorario.Insertar(horarioDto, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == true ? "Se inserto correctamente el horario" : "No se pudo insertar el horario, intente de nuevo por favor",
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ModificarUsuario(HorarioDto horarioDto)
        {
            var insert = abmHorario.Modificar(horarioDto, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == true ? "Se modifico correctamente el horario" : "No se pudo modificar el horario, intente de nuevo por favor",
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}