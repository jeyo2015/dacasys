using System.Web.Mvc;
using NLogin;
using Herramientas;
using RMTools.UI.Models;
using NAgenda;
using System;
using NConsulta;
namespace WOdontoweb.Controllers
{
    public class PacientesController : Controller
    {
        //
        // GET: /Login/

        #region Variables

        private readonly ABMPaciente gABMPaciente;

        #endregion

        #region Constructor

        public PacientesController()
        {
            gABMPaciente = new ABMPaciente();
        }

        #endregion

        public JsonResult GetPacienteConsultorio( int pIdConsultorio)
        {
            var result = gABMPaciente.GetPacientesEmpresa(pIdConsultorio);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

       

        
    }
}