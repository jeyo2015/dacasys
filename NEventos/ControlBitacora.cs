namespace NEventos
{
    using System;
    using System.Linq;
    using Datos;

    public class ControlBitacora
    {
        #region VariablesGlobales

        readonly static DataContext dataContext = new DataContext();

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
                vBitacora.FechaHora = DateTime.Now.AddHours(Get_DirefenciaHora());
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

        public static int Get_DirefenciaHora()
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