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
    public class ABMFactura
    {
        #region VariablesGlogales
        DFacturacionLinqDataContext gDc = new DFacturacionLinqDataContext();
        ControlBitacora gCb = new ControlBitacora();
        ControlLogErrores gCe = new ControlLogErrores();
        ABMDosificacion gABMDosificacion = new ABMDosificacion();
        #endregion

        #region ABMFactura
        /// <summary>
        /// Inserta una factura
        /// </summary>
        /// <param name="pnrofact">Nro de factura</param>
        /// <param name="pNombre_Cliente">Nombre del cliente</param>
        /// <param name="pFecha_emision">Fecha de emision de factura</param>
        /// <param name="pNit_clliente">Nit de cliente</param>
        /// <param name="pCodigo_control">Codigo Control</param>
        /// <param name="pDetalle_cantidad">Cantidad de licencias</param>
        /// <param name="pIDUsuario">ID usuario del que realiza accion</param>
        /// <returns>1 -  Se inserto correctamente
        ///         2 - No se inserto</returns>
        public int Insertar(int pnrofact,string pNombre_Cliente,DateTime pFecha_emision, string pNit_clliente, string pCodigo_control, int pDetalle_cantidad, string pIDUsuario)
        {
            factura_odontoweb vNewFac = new factura_odontoweb();
            DataTable vDTDosificacion_activa = Get_DosificacionActiva();
            vNewFac.codigo_control = pCodigo_control;
            vNewFac.detalle_cantidad = pDetalle_cantidad;
            vNewFac.fecha_emision =pFecha_emision;
            vNewFac.fecha_limite = (DateTime)vDTDosificacion_activa.Rows[0][5];
            vNewFac.nit_cliente = pNit_clliente;
            vNewFac.Nit_dacasys = Get_NitDacasys();
            vNewFac.nombre_cliente = pNombre_Cliente;
            vNewFac.nro_autorizacion = vDTDosificacion_activa.Rows[0][1].ToString();
            vNewFac.nro_factura = pnrofact;
            vNewFac.precio_unitario = Get_PrecioLic();
            vNewFac.total = pDetalle_cantidad*Get_PrecioLic();
            gDc.factura_odontoweb.InsertOnSubmit(vNewFac);

            try
            {
                gDc.SubmitChanges();
                gCb.Insertar("Se inserto una nueva factura", pIDUsuario);
                return 1;
            }
            catch (Exception ex)
            {
                gCe.Insertar("NFacturacion", "ABMFactura", "Insertar", ex);
                return 2;
            }


        }
        /// <summary>
        /// Modifica una factura
        /// </summary>
        /// <param name="pnrofact">Nro de factura</param>
        /// <param name="pNombre_Cliente">Nombre del cliente</param>
        /// <param name="pFecha_emision">Fecha de emision de factura</param>
        /// <param name="pNit_clliente">Nit de cliente</param>
        /// <param name="pCodigo_control">Codigo Control</param>
        /// <param name="pDetalle_cantidad">Cantidad de licencias</param>
        /// <param name="pIDUsuario">ID usuario del que realiza accion</param>
        /// <returns>1 -  Se inserto correctamente
        ///         2 - No se inserto
        ///         0 - No existe factura</returns>
        public int Modificar(int pnrofact, string pNombre_Cliente, DateTime pFecha_emision, string pNit_clliente, string pCodigo_control, int pDetalle_cantidad, string pIDUsuario)
        {

            var sql = from f in gDc.factura_odontoweb
                      where f.nro_factura == pnrofact
                      select f;
            if (sql.Count() > 0) {
               
                DataTable vDTDosificacion_activa = Get_DosificacionActiva();
                sql.First().codigo_control = pCodigo_control;
                sql.First().detalle_cantidad = pDetalle_cantidad;
                sql.First().fecha_emision = pFecha_emision;
                sql.First().fecha_limite = (DateTime)vDTDosificacion_activa.Rows[0][5];
                sql.First().nit_cliente = pNit_clliente;
                sql.First().Nit_dacasys = Get_NitDacasys();
                sql.First().nombre_cliente = pNombre_Cliente;
                sql.First().nro_autorizacion = vDTDosificacion_activa.Rows[0][1].ToString();
                sql.First().nro_factura = pnrofact;
                sql.First().precio_unitario = Get_PrecioLic();
                sql.First().total = pDetalle_cantidad * Get_PrecioLic();
                

                try
                {
                    gDc.SubmitChanges();
                    gCb.Insertar("Se modificó una nueva factura", pIDUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    gCe.Insertar("NFacturacion", "ABMFactura", "Modificar", ex);
                    return 2;
                }
            }
            return 0;
            


        }
        /// <summary>
        /// Elimina una factura fisicamente
        /// </summary>
        /// <param name="pnrofact">Nro de factura</param>
        /// <param name="pIDUsuario">ID del usuario que realiza accion</param>
        /// <returns> 0 - No se encuentra la factura
        ///             1 - Se elimino correctamente
        ///             2 - No se pudo eliminar</returns>
        public int Eliminar(int pnrofact, string pIDUsuario)
        {
            var sql = from f in gDc.factura_odontoweb
                      where f.nro_factura == pnrofact
                      select f;
            if (sql.Count() > 0)
            {
                gDc.factura_odontoweb.DeleteOnSubmit(sql.First());
              try
                {
                    gDc.SubmitChanges();
                    gCb.Insertar("Se eliminó una nueva factura ", pIDUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    gCe.Insertar("NFacturacion", "ABMFactura", "Eliminar", ex);
                    return 2;
                }
            }
            return 0;
        }
        /// <summary>
        /// Retorna el parametro de sistema que contiene la licencia
        /// </summary>
        /// <returns>0 - no hay parametro
        /// </returns>
        private int Get_PrecioLic()
        {
            var sql = from p in gDc.ParametroSistemas
                      where p.Elemento == "Licencia"
                      select p;
            if (sql.Count() > 0)
            {
                return (int)sql.First().ValorI;
            }
            return 0;
        }

        /// <summary>
        /// Retorna la dosificacion que se encuentra activa
        /// </summary>
        /// <returns></returns>
        private DataTable Get_DosificacionActiva()
        {
          return  gABMDosificacion.Get_DosificacionHabilitadap();
        }

        /// <summary>
        /// Retorna el numero de factura siguiente
        /// </summary>
        /// <returns></returns>
        public int Get_NumeroFactura()
        {
            var sql = from f in gDc.factura_odontoweb
                      orderby f.nro_factura descending
                      select f;
            if(sql.Count()>0){
                return (int)sql.First().nro_factura+1;
            }
            return 1;

        }

        /// <summary>
        /// Devuelve el parametro de sistema que contiene el nit de la empresa
        /// </summary>
        /// <returns></returns>
        private string Get_NitDacasys()
        {
            var sql = from p in gDc.ParametroSistemas
                      where p.Elemento == "NitDacasys"
                      select p;
            if (sql.Count() > 0) {
                return sql.First().ValorS;
            }
            return "";
                
        }
       
        
      

        #endregion

        #region Getter
        /// <summary>
        /// metodo privado que devuelve todas las facturas
        /// </summary>
        /// <returns> IEnumerable de tipo factura_odontoweb</returns>
        private IEnumerable<factura_odontoweb> Get_Facturas()
        {

            return from d in gDc.factura_odontoweb
                   select d;
        }

        /// <summary>
        /// Metodo publico que retorna todas las facturas
        /// </summary>
        /// <returns>Datable de tipo factura_odontoweb</returns>
        public DataTable Get_Facturasp()
        {
            return Converter<factura_odontoweb>.Convert(Get_Facturas().ToList());
        }
        /// <summary>
        /// Metodo privado que retorna la ultima factura
        /// </summary>
        /// <returns>IEnurable de tipo Factura</returns>
        private IEnumerable<factura_odontoweb> Get_FacturaLast()
        {

            return (from d in gDc.factura_odontoweb
                  orderby d.nro_factura descending
                   select d).Take(1);
        }

        public DataTable Get_FacturaLastp()
        {
            return Converter<factura_odontoweb>.Convert(Get_FacturaLast().ToList());
        }



        #endregion



       
    }
}
