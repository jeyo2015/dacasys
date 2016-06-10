﻿namespace NLogin
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
        /// <param name="pNombre"></param>
        /// <param name="pTexto"></param>
        /// <param name="pIDUsuario"></param>
        /// <returns></returns>
        public static int Insertar(String pNombre, String pTexto, string pIDUsuario)
        {
            Modulo vModulo = new Modulo();
            vModulo.Nombre = pNombre;
            vModulo.Texto = pTexto;

            try
            {
                dataContext.Modulo.InsertOnSubmit(vModulo);
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se inserto un Modulo", pIDUsuario);
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMModulo", "Insertar", ex);
                return 2;
            }
        }

        #endregion
    }
}