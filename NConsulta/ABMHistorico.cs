namespace NConsulta
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NEventos;
    using Datos;
    using Herramientas;

    public class ABMHistorico
    {
        #region VariableGlobales

        readonly static ContextoDataContext dataContext = new ContextoDataContext();

        #endregion

        #region Metodos Publicos

        public static bool GuadrarHistorico(HistoricoPacienteDto historicoPacienteDto, string idUsuario)
        {
            switch (historicoPacienteDto.EstadoABM)
            {
                case 1:
                    return InsertarHistorico(historicoPacienteDto, idUsuario);
            }
            return false;
        }
    
        /// <summary>
        /// Inserta un Historico de un Paciente
        /// </summary>
        /// <param name="historicoPaciente">historico dto</param>
        /// <param name="idUsuario">ID del usuario</param>
        public static bool InsertarHistoricoDetalle(HistoricoDetallePacienteDto historicoPaciente, string idUsuario)
        {
            var vHistoricoDetalle = new Historico_Paciente_det
            {
                fecha = DateTime.Now,
                id_cita = historicoPaciente.IdCita,
                id_empresa = historicoPaciente.IdConsultorio,
                id_paciente = historicoPaciente.IdPaciente,
                nro_detalle = historicoPaciente.NumeroDetalle,
                numero = historicoPaciente.NumeroHistorico,
                trabajo_a_realizar = historicoPaciente.TrabajoARealizar,
                trabajo_realizado = historicoPaciente.TrabajoRealizado
            };
            if (historicoPaciente.CerrarHistorico)
            {
                var historico = (from h in dataContext.Historico_Paciente
                                 where h.numero == historicoPaciente.NumeroHistorico
                                 && h.id_paciente == historicoPaciente.IdPaciente
                                 && h.id_empresa == historicoPaciente.IdConsultorio
                                 select h).FirstOrDefault();
                if (historico != null)
                    historico.estado = false;
            }

            var citaAtendida = (from c in dataContext.Cita
                                where c.idcita == historicoPaciente.IdCita
                                select c).FirstOrDefault();
            if (citaAtendida != null)
                citaAtendida.atendido = true;
            try
            {
                dataContext.Historico_Paciente_det.InsertOnSubmit(vHistoricoDetalle);
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se insertó un nuevo histórico", idUsuario);
                return true;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NConsulta", "ABMHistorico", "InsertarModulo", ex);
                return false;
            }
        }
        
        /// <summary>
        /// Metodo Privado que Retorna los detalles de un historico
        /// </summary>
        /// <param name="idEmpresa">ID del consultorio</param>
        /// <param name="idPaciente">ID del paciente</param>
        /// <param name="ticket">Numero de Historico</param>
        /// <returns>IEnumerable Historico_Paciente_det</returns>
        public static List<HistoricoDetallePacienteDto> ObtenerHistoricoDetalle(int idEmpresa, int idPaciente, int ticket)
        {
            return (from p in dataContext.Historico_Paciente_det
                    where p.id_empresa == idEmpresa && p.id_paciente == idPaciente
                    && p.numero == ticket
                    select new HistoricoDetallePacienteDto()
                    {
                        FechaCreacion = p.fecha,
                        IdCita = p.id_cita,
                        IdConsultorio = p.id_empresa,
                        IdPaciente = p.id_paciente,
                        NumeroDetalle = p.nro_detalle,
                        NumeroHistorico = p.numero,
                        TrabajoARealizar = p.trabajo_a_realizar,
                        TrabajoRealizado = p.trabajo_realizado,
                        CerrarHistorico = false
                    }).ToList();
        }

        /// <summary>
        /// Metodo privado que retorna historicos de un paciente
        /// </summary>
        /// <param name="pIDpaciente">ID del paciente</param>
        /// <param name="pIDEmpresa">ID del consultorio</param>
        /// <returns>IEnumerable<Historico_Paciente></returns>
        public static List<HistoricoPacienteDto> ObtenerHistoricoPaciente(int pIDpaciente, int pIDEmpresa)
        {
            return (from p in dataContext.Historico_Paciente
                    where p.id_empresa == pIDEmpresa && p.id_paciente == pIDpaciente &&
                    p.estado == true
                    select new HistoricoPacienteDto()
                    {
                        CitasRealizadas = p.citas_realizadas,
                        Estado = p.estado,
                        EstimacionCitas = p.estimacion_citas,
                        FechaCreacion = p.fecha_creacion,
                        IdConsultorio = p.id_empresa,
                        IdPaciente = p.id_paciente,
                        NumeroHistorico = p.numero,
                        TituloHistorico = p.titulo_numero,
                        EstadoABM = 0,
                        DetalleHistorico = ObtenerHistoricoDetalle(p.id_empresa, p.id_paciente, p.numero)
                    }).ToList();
        }

        #endregion

        #region Metodos Privados

        /// <summary>
        /// Inserta un Historico de un Paciente
        /// </summary>
        /// <param name="historicoPacienteDto">historico dto</param>
        /// <param name="idUsuario">ID del usuario</param>
        private static bool InsertarHistorico(HistoricoPacienteDto historicoPacienteDto, string idUsuario)
        {
            Historico_Paciente vHistorico = new Historico_Paciente();
            vHistorico.citas_realizadas = historicoPacienteDto.CitasRealizadas;
            vHistorico.estado = true;
            vHistorico.estimacion_citas = historicoPacienteDto.EstimacionCitas;
            vHistorico.fecha_creacion = historicoPacienteDto.FechaCreacion;
            vHistorico.id_empresa = historicoPacienteDto.IdConsultorio;
            vHistorico.id_paciente = historicoPacienteDto.IdPaciente;
            vHistorico.numero = historicoPacienteDto.NumeroHistorico;
            vHistorico.titulo_numero = historicoPacienteDto.TituloHistorico;
            try
            {
                dataContext.Historico_Paciente.InsertOnSubmit(vHistorico);
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se insertó un nuevo histórico", idUsuario);
                return true;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NConsulta", "ABMHistorico", "InsertarModulo", ex);
                return false;
            }
        }

        #endregion
    }
}