namespace NLogin
{
    using System;
    using Datos;
    using NEventos;

    public class ABMModulo
    {
        #region VariablesGlobales

        readonly DataContext dataContext = new DataContext();
        readonly ControlBitacora controlBitacora = new ControlBitacora();
        readonly ControlLogErrores controlErrores = new ControlLogErrores();

        #endregion

        #region ABM_Modulo

        /// <summary>
        /// Inserta un nuevo Modulo
        /// </summary>
        /// <param name="pNombre"></param>
        /// <param name="pTexto"></param>
        /// <param name="pIDUsuario"></param>
        /// <returns></returns>
        public int Insertar(String pNombre, String pTexto, string pIDUsuario)
        {
            Modulo vModulo = new Modulo();
            vModulo.Nombre = pNombre;
            vModulo.Texto = pTexto;

            try
            {
                dataContext.Modulo.InsertOnSubmit(vModulo);
                dataContext.SubmitChanges();
                controlBitacora.Insertar("Se inserto un Modulo", pIDUsuario);
                return 1;
            }
            catch (Exception ex)
            {
                controlErrores.Insertar("NLogin", "ABMModulo", "Insertar", ex);
                return 2;
            }

        }

        #endregion
    }
}