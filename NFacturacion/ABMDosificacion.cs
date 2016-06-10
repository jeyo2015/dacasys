namespace NFacturacion
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NEventos;
    using Datos;
    using System.Data;
    using DataTableConverter;

    public class ABMDosificacion
    {
        #region VariablesGlogales

        readonly DataContext dataContext = new DataContext();

        #endregion

        #region Metodos Publicos

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
                DateTime pFecha_inicio, DateTime pFecha_limite, string pIDUsuario)
        {
            Dosificacion vNewDosificacion = new Dosificacion();
            vNewDosificacion.estado = true;
            vNewDosificacion.fecha_inicio = pFecha_inicio;
            vNewDosificacion.fecha_limite = pFecha_limite;
            vNewDosificacion.fecha_registro = pFecha_Registro;
            vNewDosificacion.llave_dosificacion = pLlave_dosificacion;
            vNewDosificacion.NextFactura = pNextFactura;
            vNewDosificacion.nro_autorizacion = pNumero_Autorizacion;
            dataContext.Dosificacion.InsertOnSubmit(vNewDosificacion);
            try
            {
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se inserto una nueva dosificacion", pIDUsuario);
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NFacturacion", "ABMDosificacion", "Insertar", ex);
                return 2;
            }


        }

        /// <summary>
        /// Permite eliminar una dosificacion
        /// </summary>
        /// <param name="pFecha_registro">Fecha de registro de la dosificacion</param>
        /// <param name="pIDUsuario">ID del usuario que elimina la dosificacion</param>
        /// <returns></returns>
        public int Eliminar(DateTime pFecha_registro, string pIDUsuario)
        {

            var sql = from d in dataContext.Dosificacion
                      where d.fecha_registro == pFecha_registro
                      select d;
            if (sql.Count() > 0)
            {
                dataContext.Dosificacion.DeleteOnSubmit(sql.First());
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se elimino una dosificacion", pIDUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NFacturacion", "ABMDosificacion", "Eliminar", ex);
                    return 2;
                }
            } return
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
            var sql = from d in dataContext.Dosificacion
                      where d.fecha_registro == pFecha_Registro
                      select d;
            if (sql.Count() > 0)
            {
                sql.First().fecha_inicio = pFecha_inicio;
                sql.First().fecha_limite = pFecha_limite;
                sql.First().llave_dosificacion = pLlave_dosificacion;
                sql.First().NextFactura = pNextFactura;
                sql.First().nro_autorizacion = pNumero_Autorizacion;

                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se modifico la dosificacion con fecha de registro: " + pFecha_Registro.ToString(), pIDUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NFacturacion", "ABMDosificacion", "Modificar", ex);
                    return 2;
                }
            }

            return 0;
        }

        /// <summary>
        /// Metodo publico que retorna todas las dosificaciones registradas
        /// </summary>
        /// <returns>Datable de tipo Dosificacion</returns>
        public DataTable Get_Dosificacionesp()
        {
            return Converter<Dosificacion>.Convert(Get_Dosificaciones().ToList());
        }

        public DataTable Get_DosificacionHabilitadap()
        {
            return Converter<Dosificacion>.Convert(Get_DosificacionHabilitada().ToList());
        }

        /// <summary>
        /// Permite habilitar o desahabilitar alguna dosificacion
        /// </summary>
        /// <param name="pFecha_Registro">Fecha en la que se registro esa dosificacion</param>
        /// <param name="vEstado">Habilitar = true Deshabiltar = False</param>
        /// <param name="pIDUsuario">ID del usuario del que realiza la accion</param>
        /// <returns></returns>
        public int Habilitar_deshabilitarDosificacion(DateTime pFecha_Registro, bool vEstado, string pIDUsuario)
        {
            var sql = from d in dataContext.Dosificacion
                      where d.fecha_registro == pFecha_Registro
                      select d;
            if (sql.Count() > 0)
            {
                sql.First().estado = vEstado;


                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se modifico la dosificacion con fecha de registro: " + pFecha_Registro.ToString(), pIDUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NFacturacion", "ABMDosificacion", "Habilitar_deshabilitarDosificacion", ex);
                    return 2;
                }
            }

            return 0;
        }

        #endregion

        #region Metodos Privados

        /// <summary>
        /// metodo privado que devuelve todas las dosificaciones
        /// </summary>
        /// <returns> IEnumerable de tipo Dosificacion</returns>
        private IEnumerable<Dosificacion> Get_Dosificaciones()
        {

            return from d in dataContext.Dosificacion
                   select d;
        }

        /// <summary>
        /// Metodo privado que retorna la Dosificacion habilitada
        /// </summary>
        /// <returns>IEnurable de tipo Dosificacion</returns>
        private IEnumerable<Dosificacion> Get_DosificacionHabilitada()
        {

            return from d in dataContext.Dosificacion
                   where d.estado == true
                   select d;
        }

        #endregion
    }
}