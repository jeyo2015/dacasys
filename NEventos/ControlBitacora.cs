using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DLogin;
namespace NEventos
{
   public class ControlBitacora
    {
        #region VariablesGlobales
       DLoginLinqDataContext gDc = new DLoginLinqDataContext();
        #endregion

        #region ABM_LogErrores
        /// <summary>
        /// Inserta el evento generado por un usuario.
        /// </summary>
        /// <param name="pDescripcion">Descripcion del evento</param>
        /// <param name="pIDUsuario">Login del Usuario</param>
        public void Insertar(string pDescripcion, string pIDUsuario)
        {
            try
            {
                Bitacora vBitacora = new Bitacora();
                vBitacora.FechaHora = DateTime.Now.AddHours(Get_DirefenciaHora());
                vBitacora.Descripcion = pDescripcion;
                vBitacora.IDUsuario = pIDUsuario;
                gDc.Bitacora.InsertOnSubmit(vBitacora);
                gDc.SubmitChanges();
            }
            catch (Exception ex)
            {
                ControlLogErrores vLogError = new ControlLogErrores();
                vLogError.Insertar("NEventos", "ControlBitacora", "Insertar", ex);
            }
        }

        public int Get_DirefenciaHora()
        {
            var sql = from p in gDc.ParametroSistemas
                      where p.Elemento == "DiferenciaHora"
                      select p;
            if (sql.Count() > 0)
            {
                return (int)sql.First().ValorI;
            }
            return 0;
        }
        #endregion
    }
}
