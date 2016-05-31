namespace NEventos
{
    using System;
    using System.Linq;
    using Datos;

    public class ControlLogErrores
    {
        #region VariablesGlobales

        readonly DataContext dataContext = new DataContext();

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
                dataContext.LogErrores.InsertOnSubmit(vLogErrores);
                dataContext.SubmitChanges();
            }
            catch (Exception ex)
            {
                //Blog de notas por empresa
                //Insertará en un blog de Notas
            }
        }
        public int Get_DirefenciaHora()
        {
            var sql = from p in dataContext.ParametroSistemas
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