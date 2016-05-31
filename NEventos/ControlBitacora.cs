namespace NEventos
{
    using System;
    using System.Linq;
    using Datos;

    public class ControlBitacora
    {
        #region VariablesGlobales

        readonly DataContext dataContext = new DataContext();

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
                dataContext.Bitacora.InsertOnSubmit(vBitacora);
                dataContext.SubmitChanges();
            }
            catch (Exception ex)
            {
                ControlLogErrores vLogError = new ControlLogErrores();
                vLogError.Insertar("NEventos", "ControlBitacora", "Insertar", ex);
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