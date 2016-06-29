namespace NEventos
{
    using System;
    using System.Linq;
    using Datos;

    public class ControlBitacora
    {
        #region VariablesGlobales

        readonly static ContextoDataContext dataContext = new ContextoDataContext();

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Inserta el evento generado por un usuario.
        /// </summary>
        /// <param name="pDescripcion">Descripcion del evento</param>
        /// <param name="pIDUsuario">Login del Usuario</param>
        public static void Insertar(string pDescripcion, string pIDUsuario)
        {
            try
            {
                Bitacora vBitacora = new Bitacora();
                vBitacora.FechaHora = DateTime.Now.AddHours(ObtenerDirefenciaHora());
                vBitacora.Descripcion = pDescripcion;
                vBitacora.IDUsuario = pIDUsuario;
                dataContext.Bitacora.InsertOnSubmit(vBitacora);
                dataContext.SubmitChanges();
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NEventos", "ControlBitacora", "Insertar", ex);
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