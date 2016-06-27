namespace NLogin
{
    using System;
    using System.Linq;
    using NEventos;
    using Datos;

    public class ABMContador
    {
        #region VariablesGlogales

        readonly static DataContext dataContext = new DataContext();

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Controla la cantidad de personas que acceden al sistema
        /// </summary>
        /// <param name="fecha">Fecha del acceso</param>
        public void InsertarContador(DateTime fecha)
        {
            var con = from c in dataContext.Contador
                      where c.Fecha.Equals(fecha)
                      && c.Hora == fecha.TimeOfDay.Hours
                      select c;
            if (con.Any())
            {
                con.First().Cantidad = con.First().Cantidad + 1;
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Una persona ha ingresado a la portada", "-1");
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NLoging", "ABMContador", "InsertarModulo", ex);
                }
            }
            else
            {
                var vCont = new Contador {Fecha = fecha, Hora = fecha.TimeOfDay.Hours, Cantidad = 1};
                dataContext.Contador.InsertOnSubmit(vCont);
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Una persona ha ingresado a la portada", "-1");
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NLoging", "ABMContador", "InsertarModulo", ex);
                }
            }
        }

        #endregion
    }
}