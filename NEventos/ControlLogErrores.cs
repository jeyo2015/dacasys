namespace NEventos
{
    using System;
    using System.Linq;
    using Datos;

    public class ControlLogErrores
    {
        #region VariablesGlobales

        readonly static DataContext dataContext = new DataContext();

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Inserta el error que saltó en un procedimiento de una clase específica en
        /// el log de errores.
        /// </summary>
        /// <param name="idProyecto">El proyecto donde se generó el error</param>
        /// <param name="idClase">La clase donde saltó el error</param>
        /// <param name="idProcedimiento">El procedimiento donde saltó el error</param>
        /// <param name="excepcion">La excepción que dió al momento del error</param>
        public static void Insertar(string idProyecto, string idClase, string idProcedimiento, Exception excepcion)
        {
            var vDescripcion = idClase + " - " + idProcedimiento + " - " + excepcion.Message;
            try
            {
                LogErrores vLogErrores = new LogErrores();
                vLogErrores.FechaHora = DateTime.Now.AddHours(ObtenerDirefenciaHora());
                vLogErrores.Descripcion = vDescripcion;
                dataContext.LogErrores.InsertOnSubmit(vLogErrores);
                dataContext.SubmitChanges();
            }
            catch (Exception)
            {
                //Blog de notas por empresa
                //Insertará en un blog de Notas
            }
        }
        
        public static int ObtenerDirefenciaHora()
        {
            var sql = from p in dataContext.ParametroSistemas
                      where p.Elemento == "DiferenciaHora"
                      select p;
            if (!sql.Any()) return 0;
            var valorI = sql.First().ValorI;
            if (valorI != null) return (int)valorI;
            return 0;
        }

        #endregion
    }
}