using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEventos;
using DFacturacion;
using System.Data;
using DataTableConverter;
namespace NFacturacion
{
   public class ABMDosificacion
    {
        #region VariablesGlogales
       DFacturacionLinqDataContext gDc = new DFacturacionLinqDataContext();
        ControlBitacora gCb = new ControlBitacora();
        ControlLogErrores gCe = new ControlLogErrores();

        #endregion

        #region ABMDosificacion
       /// <summary>
       /// Inserta una nueva Dosificacion, para poder emitir facturas
       /// </summary>
       /// <param name="pFecha_Registro">Fecha de registro de la dosificacion</param>
       /// <param name="pNumero_Autorizacion">Nro de autorizacion de la dosificacion</param>
       /// <param name="pLlave_dosificacion">Llave de la dosificacion</param>
       /// <param name="pNextFactura">Numero de inicio de las siguientes facturas</param>
       /// <param name="pFecha_inicio">Fecha de inicio de validez de la dosificacion</param>
       /// <param name="pFecha_limite">Fecha limite de validez de la dosificacion</param>
       /// <param name="pIDUsuario">ID del usuario que inserta la nueva dosificacion</param>
       /// <returns>2 - No se pudo insertar
       ///          1 - Se inserto correctamente</returns>
        public int Insertar(DateTime pFecha_Registro, string pNumero_Autorizacion, string pLlave_dosificacion, int pNextFactura,
                DateTime pFecha_inicio, DateTime pFecha_limite, string pIDUsuario) {
                    Dosificacion vNewDosificacion = new Dosificacion();
                    vNewDosificacion.estado = true;
                    vNewDosificacion.fecha_inicio = pFecha_inicio;
                    vNewDosificacion.fecha_limite = pFecha_limite;
                    vNewDosificacion.fecha_registro = pFecha_Registro;
                    vNewDosificacion.llave_dosificacion = pLlave_dosificacion;
                    vNewDosificacion.NextFactura = pNextFactura;
                    vNewDosificacion.nro_autorizacion = pNumero_Autorizacion;
                    gDc.Dosificacion.InsertOnSubmit(vNewDosificacion);
                    try {
                        gDc.SubmitChanges();
                        gCb.Insertar("Se inserto una nueva dosificacion", pIDUsuario);
                        return 1;
                    }
                    catch (Exception ex) {
                        gCe.Insertar("NFacturacion", "ABMDosificacion", "Insertar", ex);
                        return 2;
                    }

        
        }
       /// <summary>
       /// Permite eliminar una dosificacion
       /// </summary>
       /// <param name="pFecha_registro">Fecha de registro de la dosificacion</param>
       /// <param name="pIDUsuario">ID del usuario que elimina la dosificacion</param>
       /// <returns></returns>
        public int Eliminar(DateTime pFecha_registro, string pIDUsuario) {

            var sql = from d in gDc.Dosificacion
                      where d.fecha_registro == pFecha_registro
                      select d;
            if(sql.Count()>0){
                gDc.Dosificacion.DeleteOnSubmit(sql.First());
                try {
                    gDc.SubmitChanges();
                    gCb.Insertar("Se elimino una dosificacion", pIDUsuario);
                    return 1;
                }catch(Exception ex){
                    gCe.Insertar("NFacturacion", "ABMDosificacion", "Eliminar", ex);
                    return 2;
                }
            }return
                0;
        
        }
       /// <summary>
       /// Modifica una dosificacion segun su fecha de registro
       /// </summary>
        /// <param name="pFecha_Registro">Fecha de registro de la dosificacion</param>
        /// <param name="pNumero_Autorizacion">Nro de autorizacion de la dosificacion</param>
        /// <param name="pLlave_dosificacion">Llave de la dosificacion</param>
        /// <param name="pNextFactura">Numero de inicio de las siguientes facturas</param>
        /// <param name="pFecha_inicio">Fecha de inicio de validez de la dosificacion</param>
        /// <param name="pFecha_limite">Fecha limite de validez de la dosificacion</param>
        /// <param name="pIDUsuario">ID del usuario que inserta la nueva dosificacion</param>
       /// <returns>0 - No existe dosificacion
       /// 1 - Se modifico correctamente
       /// 2 - No se pudo modificar</returns>
        public int Modificar(DateTime pFecha_Registro, string pNumero_Autorizacion, string pLlave_dosificacion, int pNextFactura,
                DateTime pFecha_inicio, DateTime pFecha_limite, string pIDUsuario)
        {
            var sql = from d in gDc.Dosificacion
                      where d.fecha_registro == pFecha_Registro
                      select d;
            if(sql.Count()>0){
                sql.First().fecha_inicio = pFecha_inicio;
                sql.First().fecha_limite = pFecha_limite;
                sql.First().llave_dosificacion = pLlave_dosificacion;
                sql.First().NextFactura = pNextFactura;
                sql.First().nro_autorizacion = pNumero_Autorizacion;

                try {
                    gDc.SubmitChanges();
                    gCb.Insertar("Se modifico la dosificacion con fecha de registro: "+ pFecha_Registro.ToString(),pIDUsuario);
                    return 1;
                }catch(Exception ex){
                    gCe.Insertar("NFacturacion", "ABMDosificacion", "Modificar", ex);
                    return 2;
                }
            }

            return 0;
        }

        #endregion

        #region Getter
       /// <summary>
       /// metodo privado que devuelve todas las dosificaciones
       /// </summary>
       /// <returns> IEnumerable de tipo Dosificacion</returns>
        private IEnumerable<Dosificacion> Get_Dosificaciones()
        {

            return from d in gDc.Dosificacion
                   select d;
        }

       /// <summary>
       /// Metodo publico que retorna todas las dosificaciones registradas
       /// </summary>
       /// <returns>Datable de tipo Dosificacion</returns>
        public DataTable Get_Dosificacionesp()
        {
            return Converter<Dosificacion>.Convert(Get_Dosificaciones() .ToList());
        }
       /// <summary>
       /// Metodo privado que retorna la Dosificacion habilitada
       /// </summary>
       /// <returns>IEnurable de tipo Dosificacion</returns>
        private IEnumerable<Dosificacion> Get_DosificacionHabilitada()
        {

            return from d in gDc.Dosificacion
                   where d.estado == true
                   select d;
        }

        public DataTable Get_DosificacionHabilitadap() {
            return Converter<Dosificacion>.Convert(Get_DosificacionHabilitada().ToList());
        }

      

        #endregion
       /// <summary>
       /// Permite habilitar o desahabilitar alguna dosificacion
       /// </summary>
       /// <param name="pFecha_Registro">Fecha en la que se registro esa dosificacion</param>
       /// <param name="vEstado">Habilitar = true Deshabiltar = False</param>
       /// <param name="pIDUsuario">ID del usuario del que realiza la accion</param>
       /// <returns></returns>
        public int Habilitar_deshabilitarDosificacion(DateTime pFecha_Registro, bool vEstado, string pIDUsuario)
        {
            var sql = from d in gDc.Dosificacion
                      where d.fecha_registro == pFecha_Registro
                      select d;
            if (sql.Count() > 0)
            {
                sql.First().estado = vEstado;
               

                try
                {
                    gDc.SubmitChanges();
                    gCb.Insertar("Se modifico la dosificacion con fecha de registro: " + pFecha_Registro.ToString(), pIDUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    gCe.Insertar("NFacturacion", "ABMDosificacion", "Habilitar_deshabilitarDosificacion", ex);
                    return 2;
                }
            }

            return 0;
        }
    }
}
