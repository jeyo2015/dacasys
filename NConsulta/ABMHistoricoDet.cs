namespace NConsulta
{
    using System.Collections.Generic;
    using System.Linq;
    using Datos;
    using Herramientas;

    public class ABMHistoricoDet
    {
        #region VariableGlobales

        readonly static ContextoDataContext dataContext = new ContextoDataContext();

        #endregion

        #region Metodos Publicos

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

        #endregion
    }
}