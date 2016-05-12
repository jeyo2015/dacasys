using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using DAgenda;
using NEventos;
using DataTableConverter;
using System.Data;

namespace NAgenda
{
    public class ABMHorario
    {
        #region VariableGlobales
        DAgendaDataContext gDc = new DAgendaDataContext();
        ControlBitacora gCb = new ControlBitacora();
        ControlLogErrores gLe = new ControlLogErrores() ;
        #endregion

        #region ABM_Horario

        /// <summary>
        /// Inserta un nuevo Horario de atencion del Doctor
        /// </summary>
        /// <param name="pIDHorario">ID del Horario</param>
        /// <param name="pNumHorario">Numero de horario del dia</param>
        /// <param name="pHoraIni">Hora inicio del horario</param>
        /// <param name="pHoraFin">Hora fin del horario del dia</param>
        /// <param name="pDia">Dia de la semana</param>
        /// <param name="pIDEmpresa">ID de la empresa</param>
        /// <param name="pIDUsuario">ID del usuario de sesion activa</param>
        public void Insertar(int pIDHorario, int pNumHorario, TimeSpan pHoraIni, TimeSpan pHoraFin, int pDia,
                             int pIDEmpresa, string pIDUsuario)
        {
          
            Horario vHorario = new Horario();
            
           
            vHorario.hora_fin = pHoraFin;
            vHorario.hora_inicio = pHoraIni;
            vHorario.iddia = pDia;
            vHorario.idempresa = pIDEmpresa;
            vHorario.num_horario = pNumHorario;
            vHorario.estado = true;
            try {
                gDc.Horario.InsertOnSubmit(vHorario);
                gDc.SubmitChanges();
               gCb.Insertar("Se inserto un horario", pIDUsuario);
            }
           catch (Exception ex) {
                gLe.Insertar("NAgenda", "ABMHorario", "Insertar", ex);
           }
            
        }

        /// <summary>
        /// Elimina fisicamente un horario
        /// </summary>
        /// <param name="pdia">ID del dia</param>
        /// <param name="pidEmpresa">ID Empresa</param>
        public void Delete_horario(int pdia, int pidEmpresa)
        {
            var sql =
            from h in gDc.Horario
             where h.iddia ==pdia && h.idempresa==pidEmpresa
            select h;

            foreach (var horario in sql)
            {
                gDc.Horario.DeleteOnSubmit(horario);
            }

            try
            {
                gDc.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Provide for exceptions.
            }
        }

        /// <summary>
        /// Modifica un Horario de atencion
        /// </summary>
        /// <param name="pIDHorario">Id del horario</param>
        /// <param name="pNumHorario">Numero del horario</param>
        /// <param name="pHoraIni">Hora inicial</param>
        /// <param name="pHoraFin">Hora Final</param>
        /// <param name="pDia">Dia del horario</param>
        /// <param name="pIDEmpresa">Id de la empresa</param>
        /// <param name="pIDUsuario">Id del usuario que realiza accion</param>
        public void Modificar(int pIDHorario, int pNumHorario, TimeSpan pHoraIni, TimeSpan pHoraFin,int pDia,
                             int pIDEmpresa, string pIDUsuario)
        {
            var sql = from h in gDc.Horario
                      where h.idhorario == pIDHorario && h.num_horario== pNumHorario
                      select h;

            if (sql.Count() > 0)
            {
                sql.First().hora_fin = pHoraFin;
                sql.First().hora_inicio = pHoraIni;
                sql.First().iddia = pDia;
                sql.First().idempresa = pIDEmpresa;
                try {
                    gDc.SubmitChanges();
                  gCb.Insertar("Se modifico el horario", pIDUsuario);
              }
              catch(Exception ex) {
             gLe.Insertar("NAgenda", "ABMHorario", "Modificar", ex);             
               }
               
            }
            else {
             gLe.Insertar("NAgenda", "ABMHorario", "Modificar, no se pudo obtener el horario", null);
            }
        }
        /// <summary>
        /// Elimina logicamente el horario con el id y numero de horario
        /// </summary>
        /// <param name="pIDHorario">Id del horario</param>
        /// <param name="pNumHorario">Numero de Horario</param>
        /// <param name="pIDUsuario">Id del usuario que realiza la accion</param>
        public void Eliminar(int pIDHorario, int pNumHorario, string pIDUsuario)
        {
            var sql = from h in gDc.Horario
                      where h.idhorario == pIDHorario && h.num_horario == pNumHorario
                      select h;

            if (sql.Count() > 0)
            {
                sql.First().estado = false;
                try
                {
                    gDc.SubmitChanges();
                    gCb.Insertar("Se elimino el horario", pIDUsuario);
                }
                catch (Exception ex)
                {
                    gLe.Insertar("NAgenda", "ABMHorario", "Eliminar", ex);
                }
            }
            else
            {
                gLe.Insertar("NAgenda", "ABMHorario", "Eliminar, no se pudo obtener el horario", null);
            }
        }
        #endregion
        #region Getter_Horarios
        /// <summary>
        /// Metodo privado que retorna los horarios de un consultorio segun el el dia
        /// </summary>
        /// <param name="pidDia">ID del dia</param>
        /// <param name="pidEmpresa">Id del consultorio</param>
        /// <returns>IEnumerable<Horario></returns>
        private IEnumerable<Horario> Get_Horarios(int pidDia,int pidEmpresa)
        {

            return from h in gDc.Horario
                   where h.iddia == pidDia && h.idempresa == pidEmpresa
                   orderby h.hora_inicio
                   select h ;

        }

      /// <summary>
      /// Devuelve todos los horarios de atencion del Dr
      /// </summary>
      /// <param name="pidEmpresa">ID de la empresa</param>
      /// <param name="pidDia">ID del Dia</param>
      /// <returns>DataTable con los horarios</returns>
        public DataTable Get_Horariosp(int pidDia, int ipdEmpresa)
        {
            return Converter<Horario>.Convert(Get_Horarios(pidDia, ipdEmpresa).ToList());
        }

       /// <summary>
       /// Metodo para verificar la entrada
       /// </summary>
       /// <param name="pDTgral"></param>
       /// <param name="pFila"></param>
       /// <returns></returns>
        public bool Verificar_fila(DataTable pDTgral, DataRow pFila)
        {
            foreach (DataRow fila in pDTgral.Rows)
            {
                if (((TimeSpan)pFila[2] >= (TimeSpan)fila[2] )&&
                    ((TimeSpan)pFila[3] < (TimeSpan)fila[2]))
                    return false;
                if (((TimeSpan)pFila[2] >= (TimeSpan)fila[2]) ||
                    ((TimeSpan)pFila[3] < (TimeSpan)fila[2]))
                    return false;
            }
            return true;
        }
        #endregion
    }
}
