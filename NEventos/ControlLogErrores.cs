using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEventos;
namespace NEventos
{
  public  class ControlLogErrores
    {
        #region VariablesGlobales
        private DEventosLinqDataContext gDc = new DEventosLinqDataContext();
        #endregion



        #region ABM_LogErrores

        /// <summary>
        /// Inserta el error que saltó en un procedimiento de una clase específica en
        /// el log de errores.
        /// </summary>
        /// <param name="pProyecto">El proyecto donde se generó el error</param>
        /// <param name="pClase">La clase donde saltó el error</param>
        /// <param name="pProcedimiento">El procedimiento donde saltó el error</param>
        /// <param name="pEx">La excepción que dió al momento del error</param>
        public void Insertar(string pProyecto, string pClase, string pProcedimiento, Exception pEx)
        {
            string vDescripcion = pClase + " - " + pProcedimiento + " - " + pEx.Message.ToString();
            try
            {
                LogErrores vLogErrores = new LogErrores();
                vLogErrores.FechaHora = DateTime.Now.AddHours(Get_DirefenciaHora());
                vLogErrores.Descripcion = vDescripcion;
                gDc.LogErrores.InsertOnSubmit(vLogErrores);
                gDc.SubmitChanges();
            }
            catch (Exception ex)
            {
                //Blog de notas por empresa
                //Insertará en un blog de Notas
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
