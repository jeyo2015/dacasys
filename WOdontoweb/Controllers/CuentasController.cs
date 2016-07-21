namespace WOdontoweb.Controllers
{
    using System.Web.Mvc;
    using Herramientas;
    using Models;
    using NAgenda;

    public class CuentasController : Controller
    {
        public JsonResult ObtenerCuentasPorCobrarPorConsultorio(int IdConsultorio)
        {
            var result = ABMCuenta.ObtenerCuentasPorCobrarPorConsultorio(IdConsultorio);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTrabajosConsultorio(int IdConsultorio)
        {
            var result = ABMCuenta.GetTrabajosConsultorio(IdConsultorio);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EliminarPago(int pIdPago)
        {
            var viel = ABMCuenta.EliminarPago(pIdPago, Session["loginusuario"].ToString());
            var result = new ResponseModel()
          {
              Message = viel  ? "Se elimino correctamente el pago" : "No se pudo eliminar el pago, intente de nuevo por favor.",
              Data = viel
          };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarNuevoPago(CuentasPorCobrarDetalleDto pPago)
        {
            var insert = ABMCuenta.InsertarUnPago(pPago, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert? "Se inserto correctamente el pago" : "No se pudo insertar el pago, intente de nuevo por favor",
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertarNuevaCuenta(CuentasPorCobrarDto pCuenta)
        {
            var insert = ABMCuenta.InsertarUnaCuenta(pCuenta, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert ? "Se inserto correctamente la cuenta" : "No se pudo insertar la cuenta, intente de nuevo por favor",
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ModificarPago(CuentasPorCobrarDetalleDto pPago)
        {
            var insert = ABMCuenta.ModificarPago(pPago, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert ? "Se modifico correctamente el pago" : "No se pudo modificar el pago, intente de nuevo por favor",
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ModificarCuenta(CuentasPorCobrarDto pCuenta)
        {
            var insert = ABMCuenta.ModificarCuenta(pCuenta, Session["loginusuario"].ToString());
            var result = new ResponseModel()
            {
                Message = insert ? "Se modifico correctamente la cuenta" : "No se pudo modificar la cuenta, intente de nuevo por favor",
                Data = insert
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}