using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEventos;
using DLogin;
namespace NLogin
{
   public class ABMContador
    {
        #region VariablesGlogales
        DLoginLinqDataContext gDc = new DLoginLinqDataContext();
        ControlBitacora gCb = new ControlBitacora();
        ControlLogErrores gCe = new ControlLogErrores();
       
        #endregion

        #region ABM
       /// <summary>
       /// Controla la cantidad de personas que acceden al sistema
       /// </summary>
       /// <param name="pFecha">Fecha del acceso</param>
        public void Insertar_Contador(DateTime pFecha) {
            var con = from c in gDc.Contador
                      where c.Fecha.Equals(pFecha)
                      && c.Hora==pFecha.TimeOfDay.Hours
                      select c;
            if (con.Count() > 0)
            {
                con.First().Cantidad = con.First().Cantidad + 1;
                try
                {
                    gDc.SubmitChanges();
                    gCb.Insertar("Una persona ha ingresado a la portada", "-1");
                }catch(Exception ex){
                    gCe.Insertar("NLoging", "ABMContador", "Insertar", ex);
                }
            }
            else {
                Contador vCont = new Contador();
                vCont.Fecha = pFecha;
                vCont.Hora = pFecha.TimeOfDay.Hours;
                vCont.Cantidad = 1;
                gDc.Contador.InsertOnSubmit(vCont);
                try
                {
                    gDc.SubmitChanges();
                    gCb.Insertar("Una persona ha ingresado a la portada", "-1");
                }
                catch (Exception ex)
                {
                    gCe.Insertar("NLoging", "ABMContador", "Insertar", ex);
                }

            }
        }
        #endregion
    }
}
