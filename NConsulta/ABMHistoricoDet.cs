namespace NConsulta
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NEventos;
    using NAgenda;
    using Datos;
    using Herramientas;

    public class ABMHistoricoDet
    {
        #region VariableGlobales

        readonly DataContext dataContext = new DataContext();
        readonly ControlBitacora controlBitacora = new ControlBitacora();
        readonly ControlLogErrores controlErrores = new ControlLogErrores();

        #endregion

        #region ABM_HistoricoDet
        /// <summary>
        /// Permite Insetar un nuevo detalle, dentro de un historico
        /// </summary>
        /// <param name="pIDEmpresa">ID de la Empresa</param>
        /// <param name="pIDpaciente">ID del Paciente</param>
        /// <param name="ticket">Nro de Historico</param>
        /// <param name="pNro_detalle">Nro de detalle</param>
        /// <param name="trabajo_realizado">Trabajo realizado en la consulta</param>
        /// <param name="pTrabajo_a_realizar">Trabajo a realizar en la siguiente consulta</param>
        /// <param name="pFecha">Fecha de la consulta </param>
        /// <param name="pIDCita">ID de la cita</param>
        /// <param name="pFinalizado">True, no finalizado 
        ///                         False, finalizado</param>
        /// <param name="pIDUsuario">Usuario que realiza accion</param>
        /// <returns>1 - Insertado correctamente
        ///          2 - No se pudo insertar</returns>
        public int Insertar(int pIDEmpresa, int pIDpaciente, int ticket, int pNro_detalle, string trabajo_realizado,
                            string pTrabajo_a_realizar, DateTime pFecha, String pIDCita, bool pFinalizado, string pIDUsuario)
        {
            Historico_Paciente_det vHistoricoDet = new Historico_Paciente_det();
            vHistoricoDet.fecha = pFecha;
            vHistoricoDet.id_cita = pIDCita;
            vHistoricoDet.id_empresa = pIDEmpresa;
            vHistoricoDet.id_paciente = pIDpaciente;
            vHistoricoDet.nro_detalle = pNro_detalle;
            vHistoricoDet.numero = ticket;
            vHistoricoDet.trabajo_a_realizar = pTrabajo_a_realizar;
            vHistoricoDet.trabajo_realizado = trabajo_realizado;
            try
            {

                dataContext.Historico_Paciente_det.InsertOnSubmit(vHistoricoDet);
                dataContext.SubmitChanges();
                controlBitacora.Insertar("Se inserto Nuevo detalle de Historico", pIDUsuario);

                if (pFinalizado)
                    Actualizar_citas_realizadas(pIDEmpresa, pIDpaciente, ticket, pIDUsuario);

                int vAux = Actualizar_Cita_Atendido(pIDCita, pIDUsuario);

                return vAux;
            }
            catch (Exception ex)
            {
                controlErrores.Insertar("NConsulta", "ABMHistoricoDet", "Insertar", ex);
                return 2;
            }

        }

        public void Eliminar(int pIDEmpresa, int pIDpaciente, int ticket, int pNro_detalle)
        {
            var sql = from e in dataContext.Historico_Paciente_det
                      where e.id_paciente == pIDpaciente && e.id_empresa == pIDEmpresa
                      && e.numero == ticket && e.nro_detalle == pNro_detalle
                      select e;


        }

        /// <summary>
        /// Permite modificar el detalle de un historico
        /// </summary>
        /// <param name="pIDEmpresa">ID de consultorio</param>
        /// <param name="pIDpaciente">ID del paciente</param>
        /// <param name="ticket">Numero de Historico</param>
        /// <param name="pNro_detalle">Numero de detalle</param>
        /// <param name="trabajo_realizado">Trabajo realizado</param>
        /// <param name="pTrabajo_a_realizar">Trabajo a realizar</param>
        /// <param name="pFecha">Fecha del detalle</param>
        /// <param name="pIDCita">ID de la cita</param>
        public void Modificar(int pIDEmpresa, int pIDpaciente, int ticket, int pNro_detalle, string trabajo_realizado,
                            string pTrabajo_a_realizar, DateTime pFecha, String pIDCita)
        {

            var sql = from e in dataContext.Historico_Paciente_det
                      where e.id_paciente == pIDpaciente && e.id_empresa == pIDEmpresa
                      && e.numero == ticket && e.nro_detalle == pNro_detalle
                      select e;

            if (sql.Count() > 0)
            {
                sql.First().fecha = pFecha;
                sql.First().id_cita = pIDCita;

                sql.First().trabajo_a_realizar = pTrabajo_a_realizar;
                sql.First().trabajo_realizado = trabajo_realizado;
                dataContext.SubmitChanges();
            }



        }

        #endregion

        #region Getter_HistoricoDET
        /// <summary>
        /// Metodo Privado que Retorna los detalles de un historico
        /// </summary>
        /// <param name="pIDEmpresa">ID del consultorio</param>
        /// <param name="pIDpaciente">ID del paciente</param>
        /// <param name="ticket">Numero de Historico</param>
        /// <returns>IEnumerable Historico_Paciente_det</returns>
        public List<HistoricoDetallePacienteDto> GetHistoricoDetalle(int pIDEmpresa, int pIDpaciente, int ticket)
        {
            return (from p in dataContext.Historico_Paciente_det
                    where p.id_empresa == pIDEmpresa && p.id_paciente == pIDpaciente
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

        #region Metodos_Auxiliares
        /// <summary>
        /// Permite cambiar el estado de cita atendido a true
        /// </summary>
        /// <param name="pIDCita">ID de la cita</param>
        /// <param name="pIDUsuario">ID Del usuario que realiza accion</param>
        /// <returns> 1 - Se Actualizo correctamente
        ///    2 - no se pudo actualizar</returns>
        private int Actualizar_Cita_Atendido(string pIDCita, string pIDUsuario)
        {
            ABMCita vABMCita = new ABMCita();
            return vABMCita.Actualizar_Atendido(pIDCita, pIDUsuario);
        }

        /// <summary>
        /// Cerrar un historico
        /// </summary>
        /// <param name="pIDEmpresa">ID del Consultorio</param>
        /// <param name="pIDpaciente">ID del paciente</param>
        /// <param name="pTicket">Numero de Historico</param>
        /// <param name="pIDUsuario">ID del usuario que cierra historico</param>
        /// <returns> 1 - todo en orden
        ///     2 - No se pudo cerrar historico
        ///     0 - No se encuentra historico</returns>
        private int Actualizar_citas_realizadas(int pIDEmpresa, int pIDpaciente, int pTicket,
                                            string pIDUsuario)
        {
            var sql = from e in dataContext.Historico_Paciente
                      where e.id_paciente == pIDpaciente && e.id_empresa == pIDEmpresa
                      && e.numero == pTicket
                      select e;

            if (sql.Count() > 0)
            {

                sql.First().estado = false;
                try
                {
                    dataContext.SubmitChanges();
                    controlBitacora.Insertar("Se cerró un historico", pIDUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    controlErrores.Insertar("NConsulta", "ABMHistoricoDet", "Actualizar_Citas_Realizadas", ex);
                    return 2;
                }
            } return 0;
        }
        #endregion
    }
}