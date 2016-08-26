namespace NAgenda
{
    using System;
    using System.Collections.Generic;
    using NEventos;
    using System.Linq;
    using Herramientas;
    using Datos;

    public class ABMHorario
    {
        #region VariableGlobales

        readonly static ContextoDataContext dataContext = new ContextoDataContext();

        #endregion

        #region Metodos publicos

        /// <summary>
        /// InsertarModulo nuevo horario
        /// </summary>
        /// <param name="horarioDto"></param>
        /// <param name="idUsuario"></param>
        public static bool Insertar(HorarioDto horarioDto, string idUsuario)
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
                ControlLogErrores.Insertar("NAgenda", "ABMHorario", "InsertarModulo", ex);
                return false;
            }
        }

        /// <summary>
        /// Actualizar horarios
        /// </summary>
        /// <param name="horarioDto"></param>
        /// <param name="idUsuario"></param>
        public static bool Modificar(HorarioDto horarioDto, string idUsuario)
        {
            var sql = from h in dataContext.Horario
                      where h.idhorario == horarioDto.IDHorario
                      select h;

            if (!sql.Any()) return false;
            sql.First().hora_fin = TimeSpan.Parse(horarioDto.HoraFin);
            sql.First().hora_inicio = TimeSpan.Parse(horarioDto.HoraInicio);
            sql.First().iddia = horarioDto.IDDia;
            sql.First().num_horario = horarioDto.NumHorario;

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
            return false;
        }

        /// <summary>
        /// Elimina logicamente el horario con el id y numero de horario
        /// </summary>
        /// <param name="idHorario">Id del horario</param>
        /// <param name="numHorario">Numero de Horario</param>
        /// <param name="idUsuario">Id del usuario que realiza la accion</param>
        public static bool Eliminar(int idHorario, int numHorario, string idUsuario)
        {
            var sql = from h in dataContext.Horario
                      where h.idhorario == idHorario && h.num_horario == numHorario
                      select h;

            if (sql.Any())
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

        public static List<HorarioMapaDto> ObtenerHorarioParaMostrarMapa(int idConsultorio)
        {
            var horariosConsultorio = (from h in dataContext.Horario
                                       from d in dataContext.Dia
                                       where d.iddia == h.iddia
                                       && h.idempresa == idConsultorio
                                       && h.estado
                                       select new HorarioDto
                                       {
                                           Estado = h.estado,
                                           HoraFinSpan = h.hora_fin,
                                           HoraInicioSpan = h.hora_inicio,
                                           IDDia = h.iddia,
                                           HoraInicio = h.hora_inicio.Hours + ":" + h.hora_inicio.Minutes,
                                           HoraFin = h.hora_fin.Hours + ":" + h.hora_fin.Minutes,
                                           NombreDia = d.nombre_corto,
                                           IDEmpresa = idConsultorio
                                       }).OrderBy(o => o.IDDia).GroupBy(g => new { g.IDDia, g.NombreDia }).Select(
                                           gr => new HorarioMapa
                                           {
                                               IdDia = gr.Key.IDDia,
                                               NombreCorto = gr.Key.NombreDia,
                                               Horarios = gr.ToList()
                                           }).ToList();

            var xxx = horariosConsultorio.Count();
            List<HorarioMapaDto> listaRetornar = new List<HorarioMapaDto>();
            for (int i = 0; i < xxx; i++)
            {
                var horarioActual = horariosConsultorio[i];
                if (!horariosConsultorio[i].hasFriend)
                {
                    //List<DiaDto> dias = new List<DiaDto>(){ 
                    //     new DiaDto(){
                    //         IDDia=horarioActual.IdDia,
                    //     NombreCorto = horarioActual.NombreCorto                        
                    //     }
                    //    };
                    string dias = horarioActual.NombreCorto;
                    for (int j = i+1; j < xxx; j++)
                    {

                        if (compararHorarios(horarioActual, horariosConsultorio[j]))
                        {
                            horariosConsultorio[j].hasFriend = true;
                            dias = dias + "-" + horariosConsultorio[j].NombreCorto;
                            //dias.Add(new DiaDto
                            //{
                            //    IDDia = horariosConsultorio[j].IdDia,
                            //    NombreCorto = horariosConsultorio[j].NombreCorto
                            //});
                        }
                    }
                    listaRetornar.Add(new HorarioMapaDto()
                    {
                        Dias = dias,
                        Horarios = horarioActual.Horarios
                    });
                }
            }
            return listaRetornar;
        }

        private static bool compararHorarios(HorarioMapa horarioActual, HorarioMapa horarioSiguiente)
        {
            if (horarioActual.Horarios.Count() != horarioSiguiente.Horarios.Count())
                return false;
            var cantidadHorariosDia = horarioActual.Horarios.Count();
            for (int i = 0; i < cantidadHorariosDia; i++)
            {
                if (!horarioActual.Horarios[i].HoraInicioSpan.Equals(horarioSiguiente.Horarios[i].HoraInicioSpan))
                    return false;
                if (!horarioActual.Horarios[i].HoraFinSpan.Equals(horarioSiguiente.Horarios[i].HoraFinSpan))
                    return false;
            }
            return true;
        }
        public static List<HorarioDto> ObtenerHorariosPorEmpresa(int idEmpresa)
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

        #endregion
    }
}
