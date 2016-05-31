namespace NLogin
{
    using System;
    using System.Linq;
    using NEventos;
    using Datos;

    public class ABMContador
    {
        #region VariablesGlogales

        readonly DataContext dataContext = new DataContext();
        readonly ControlBitacora controlBitacora = new ControlBitacora();
        readonly ControlLogErrores controlErrores = new ControlLogErrores();

        #endregion

        #region ABM
        /// <summary>
        /// Controla la cantidad de personas que acceden al sistema
        /// </summary>
        /// <param name="pFecha">Fecha del acceso</param>
        public void Insertar_Contador(DateTime pFecha)
        {
            var con = from c in dataContext.Contador
                      where c.Fecha.Equals(pFecha)
                      && c.Hora == pFecha.TimeOfDay.Hours
                      select c;
            if (con.Count() > 0)
            {
                con.First().Cantidad = con.First().Cantidad + 1;
                try
                {
                    dataContext.SubmitChanges();
                    controlBitacora.Insertar("Una persona ha ingresado a la portada", "-1");
                }
                catch (Exception ex)
                {
                    controlErrores.Insertar("NLoging", "ABMContador", "Insertar", ex);
                }
            }
            else
            {
                Contador vCont = new Contador();
                vCont.Fecha = pFecha;
                vCont.Hora = pFecha.TimeOfDay.Hours;
                vCont.Cantidad = 1;
                dataContext.Contador.InsertOnSubmit(vCont);
                try
                {
                    dataContext.SubmitChanges();
                    controlBitacora.Insertar("Una persona ha ingresado a la portada", "-1");
                }
                catch (Exception ex)
                {
                    controlErrores.Insertar("NLoging", "ABMContador", "Insertar", ex);
                }

            }
        }
        #endregion
    }
}