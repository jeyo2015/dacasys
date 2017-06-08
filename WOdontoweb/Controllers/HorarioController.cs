namespace WOdontoweb.Controllers
{
    using System.Web.Mvc;
    using Herramientas;
    using NAgenda;
    using Models;
    using System.Collections.Generic;
    public class HorarioController : Controller
    {
        public JsonResult ObtenerDias()
        {
            var result = ABMDia.ObtenerDias();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerHorariosPorEmpresa(int idEmpresa)
        {
            var result = ABMHorario.ObtenerHorariosPorEmpresa(idEmpresa);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarHorario(HorarioDto horarioDto)
        {
            var viel = ABMHorario.Eliminar(horarioDto.IDHorario, horarioDto.NumHorario, Session["loginusuario"].ToString());
            var result = new ResponseModel()
          {
              Message = viel == true ? "Se elimino correctamente el horario" : "No se pudo eliminar el horario, intente de nuevo por favor.",
              Data = viel
          };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarNuevoHorario(HorarioDto horarioDto, List<DiaDto> diasSeleccionados)
        {
            var insert = ABMHorario.Insertar(horarioDto, Session["loginusuario"].ToString(), diasSeleccionados);
            var result = new ResponseModel()
            {
                Message = insert == true ? "Se inserto correctamente el horario" : "No se pudo insertar el horario, intente de nuevo por favor",
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ModificarHorario(HorarioDto horarioDto)
        {
            var insert = ABMHorario.Modificar(horarioDto, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert == true ? "Se modifico correctamente el horario" : "No se pudo modificar el horario, intente de nuevo por favor",
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}