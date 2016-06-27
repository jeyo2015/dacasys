namespace NLogin
{
    using System;
    using Datos;
    using NEventos;

    public class ABMModulo
    {
        #region VariablesGlobales

        readonly static DataContext dataContext = new DataContext();

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Inserta un nuevo Modulo
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="texto"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public static int InsertarModulo(string nombre, string texto, string idUsuario)
        {
            Modulo vModulo = new Modulo();
            vModulo.Nombre = nombre;
            vModulo.Texto = texto;

            try
            {
                dataContext.Modulo.InsertOnSubmit(vModulo);
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se inserto un Modulo", idUsuario);
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMModulo", "InsertarModulo", ex);
                return 2;
            }
        }

        #endregion
    }
}