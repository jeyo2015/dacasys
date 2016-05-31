namespace NAgenda
{
    using System;
    using System.Collections.Generic;
    using System.Collections;
    using System.Linq;
    using System.Text;
    using DAgenda;
    using NEventos;
    using DataTableConverter;
    using System.Data;
    using Herramientas;

    public class ABMHorario
    {
        #region VariableGlobales

        DAgendaDataContext agendaContext = new DAgendaDataContext();
        ControlBitacora controlBitacora = new ControlBitacora();
        ControlLogErrores controlErrores = new ControlLogErrores();

        #endregion

        #region ABM_Horario

        /// <summary>
        /// Insertar nuevo horario
        /// </summary>
        /// <param name="horarioDto"></param>
        /// <param name="idUsuario"></param>
        public bool Insertar(HorarioDto horarioDto, string idUsuario)
        {
            Horario vHorario = new Horario();
            vHorario.hora_fin = horarioDto.HoraFin;
            vHorario.hora_inicio = horarioDto.HoraInicio;
            vHorario.iddia = horarioDto.IDDia;
            vHorario.idempresa = horarioDto.IDEmpresa;
            vHorario.num_horario = horarioDto.NumHorario;
            vHorario.estado = true;
            try
            {
                agendaContext.Horario.InsertOnSubmit(vHorario);
                agendaContext.SubmitChanges();
                controlBitacora.Insertar("Se inserto un horario", idUsuario);
                return true;
            }
            catch (Exception ex)
            {
                controlErrores.Insertar("NAgenda", "ABMHorario", "Insertar", ex);
                return false;
            }
        }

        /// <summary>
        /// Actualizar horarios
        /// </summary>
        /// <param name="horarioDto"></param>
        /// <param name="idUsuario"></param>
        public bool Modificar(HorarioDto horarioDto, string idUsuario)
        {
            var sql = from h in agendaContext.Horario
                      where h.idhorario == horarioDto.IDHorario && h.num_horario == horarioDto.NumHorario
                      select h;

            if (sql.Count() > 0)
            {
                sql.First().hora_fin = horarioDto.HoraFin;
                sql.First().hora_inicio = horarioDto.HoraInicio;
                sql.First().iddia = horarioDto.IDDia;
                sql.First().idempresa = horarioDto.IDEmpresa;
                try
                {
                    agendaContext.SubmitChanges();
                    controlBitacora.Insertar("Se modifico el horario", idUsuario);
                    return true;
                }
                catch (Exception ex)
                {
                    controlErrores.Insertar("NAgenda", "ABMHorario", "Modificar", ex);
                }
            }
            else
            {
                controlErrores.Insertar("NAgenda", "ABMHorario", "Modificar, no se pudo obtener el horario", null);
            }
            return false;
        }

        /// <summary>
        /// Elimina logicamente el horario con el id y numero de horario
        /// </summary>
        /// <param name="idHorario">Id del horario</param>
        /// <param name="numHorario">Numero de Horario</param>
        /// <param name="idUsuario">Id del usuario que realiza la accion</param>
        public bool Eliminar(int idHorario, int numHorario, string idUsuario)
        {
            var sql = from h in agendaContext.Horario                      
                      where h.idhorario == idHorario && h.num_horario == numHorario
                      select h;

            if (sql.Count() > 0)
            {
                sql.First().estado = false;
                try
                {
                    agendaContext.SubmitChanges();
                    controlBitacora.Insertar("Se elimino el horario", idUsuario);
                    return true;
                }
                catch (Exception ex)
                {
                    controlErrores.Insertar("NAgenda", "ABMHorario", "Eliminar", ex);
                }
            }
            else
            {
                controlErrores.Insertar("NAgenda", "ABMHorario", "Eliminar, no se pudo obtener el horario", null);
            }
            return false;
        }

        #endregion

        #region Getter_Horarios

        /// <summary>
        /// Metodo privado que retorna los horarios de un consultorio segun el el dia
        /// </summary>
        /// <param name="pidDia">ID del dia</param>
        /// <param name="pidEmpresa">Id del consultorio</param>
        /// <returns>IEnumerable<Horario></returns>
        public List<HorarioDto> ObtenerHorariosPorDia(int idDia, int idEmpresa)
        {
            return (from h in agendaContext.Horario
                    join d in agendaContext.Dia on h.iddia equals d.iddia
                    where h.iddia == idDia && h.idempresa == idEmpresa && h.estado == true
                    orderby h.iddia
                    select new HorarioDto()
                    {
                        HoraInicio = h.hora_inicio,
                        HoraFin = h.hora_fin,
                        IDHorario = h.idhorario,
                        IDDia = h.iddia,
                        IDEmpresa = h.idempresa,
                        NumHorario = h.num_horario,
                        Estado = h.estado,
                        NombreDia = d.nombre_corto
                    }).ToList();
        }

        public List<HorarioDto> ObtenerHorariosPorEmpresa(int idEmpresa)
        {
            return (from h in agendaContext.Horario
                    join d in agendaContext.Dia on h.iddia equals d.iddia
                    where h.idempresa == idEmpresa && h.estado == true
                    orderby h.iddia
                    select new HorarioDto()
                    {
                        HoraInicio = h.hora_inicio,
                        HoraFin = h.hora_fin,
                        IDHorario = h.idhorario,
                        IDDia = h.iddia,
                        IDEmpresa = h.idempresa,
                        NumHorario = h.num_horario,
                        Estado = h.estado,
                        NombreDia = d.nombre_corto
                    }).ToList();
        }

        /// <summary>
        /// Devuelve el nombre corto del dia
        /// </summary>
        /// <param name="idDia"></param>
        /// <returns></returns>
        public string ObtenerNombreCortoDelDia(int idDia)
        {
            var sql = from d in agendaContext.Dia
                      where d.iddia == idDia
                      select d;
            if (sql.Count() > 0)
            {
                return sql.First().nombre_corto;
            }
            return "none";

        }

        /// <summary>
        /// Metodo para verificar la entrada
        /// </summary>
        /// <param name="pDTgral"></param>
        /// <param name="pFila"></param>
        /// <returns></returns>
        public bool VerificarFila(DataTable pDTgral, DataRow pFila)
        {
            foreach (DataRow fila in pDTgral.Rows)
            {
                if (((TimeSpan)pFila[2] >= (TimeSpan)fila[2]) &&
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
