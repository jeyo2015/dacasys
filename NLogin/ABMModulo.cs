using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DLogin;
using NEventos;
namespace NLogin
{
   public class ABMModulo
   {
       #region VariablesGlobales
       DLoginLinqDataContext gDc = new DLoginLinqDataContext();
       ControlBitacora gCb = new ControlBitacora();
       ControlLogErrores gCe = new ControlLogErrores();
       #endregion

       #region ABM_Modulo

       /// <summary>
       /// Inserta un nuevo Modulo
       /// </summary>
       /// <param name="pNombre"></param>
       /// <param name="pTexto"></param>
       /// <param name="pIDUsuario"></param>
       /// <returns></returns>
       public int Insertar(String pNombre, String pTexto, string pIDUsuario) { 
          Modulo vModulo = new Modulo();
          vModulo.Nombre = pNombre;
          vModulo.Texto = pTexto;

          try{
                  gDc.Modulo.InsertOnSubmit(vModulo);
                  gDc.SubmitChanges();
                  gCb.Insertar("Se inserto un Modulo", pIDUsuario);
                  return 1;
            }
              catch (Exception ex)
             {
                 gCe.Insertar("NLogin", "ABMModulo", "Insertar", ex);
                 return 2;
             }
       
       }

       #endregion

   }
}
