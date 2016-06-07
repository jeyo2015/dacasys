namespace NAgenda
{
    using System;
    using System.Collections.Generic;
    using NEventos;
    using System.Data;
    using System.Linq;
    using Herramientas;
    using Datos;

    public class ABMHorario
    {
        #region VariableGlobales

        readonly DataContext dataContext = new DataContext();

        #endregion

        #region ABM_Horario

        /// <summary>
        /// Insertar nuevo horario
        /// </summary>
        /// <param name="horarioDto"></param>
        /// <param name="idUsuario"></param>
        public bool Insertar(HorarioDto horarioDto, string idUsuario)
        {
            try
            {
                var vHorario = new Horario
                {
                    hora_fin = TimeSpan.Parse(horarioDto.HoraFin),
                    hora_inicio = TimeSpan.Parse(horarioDto.HoraInicio),
                    iddia = horarioDto.IDDia,
                    idempresa = horarioDto.IDEmpresa,
                    num_horario = horarioDto.NumHorario,
                    estado = true
                };

                dataContext.Horario.InsertOnSubmit(vHorario);
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se inserto un horario", idUsuario);
                return true;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NAgenda", "ABMHorario", "Insertar", ex);
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
            var sql = from h in dataContext.Horario
                      where h.idhorario == horarioDto.IDHorario && h.num_horario == horarioDto.NumHorario
                      select h;

            if (sql.Count() > 0)
            {
                sql.First().hora_fin = TimeSpan.Parse(horarioDto.HoraFin);
                sql.First().hora_inicio = TimeSpan.Parse(horarioDto.HoraInicio);
                sql.First().iddia = horarioDto.IDDia;
                sql.First().idempresa = horarioDto.IDEmpresa;
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se modifico el horario", idUsuario);
                    return true;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NAgenda", "ABMHorario", "Modificar", ex);
                }
            }
            else
            {
                ControlLogErrores.Insertar("NAgenda", "ABMHorario", "Modificar, no se pudo obtener el horario", null);
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
            var sql = from h in dataContext.Horario
                      where h.idhorario == idHorario && h.num_horario == numHorario
                      select h;

            if (sql.Count() > 0)
            {
                sql.First().estado = false;
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se elimino el horario", idUsuario);
                    return true;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NAgenda", "ABMHorario", "Eliminar", ex);
                }
            }
            else
            {
                ControlLogErrores.Insertar("NAgenda", "ABMHorario", "Eliminar, no se pudo obtener el horario", null);
            }
            return false;
        }

        #endregion

        #region Getter_Horarios

        public List<HorarioDto> ObtenerHorariosPorEmpresa(int idEmpresa)
        {
            return (from h in dataContext.Horario
                    join d in dataContext.Dia on h.iddia equals d.iddia
                    where h.idempresa == idEmpresa && h.estado == true
                    orderby h.iddia
                    select new HorarioDto()
                    {
                        HoraInicio = h.hora_inicio.Hours + ":" + h.hora_inicio.Minutes,
                        HoraFin = h.hora_fin.Hours + ":" + h.hora_fin.Minutes,
                        IDHorario = h.idhorario,
                        IDDia = h.iddia,
                        IDEmpresa = h.idempresa,
                        NumHorario = h.num_horario,
                        Estado = h.estado,
                        NombreDia = d.nombre_corto
                    }).ToList();
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
