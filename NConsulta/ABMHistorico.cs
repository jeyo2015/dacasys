namespace NConsulta
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NEventos;
    using DataTableConverter;
    using System.Data;
    using Datos;
    using Herramientas;

    public class ABMHistorico
    {
        #region VariableGlobales

        readonly DataContext dataContext = new DataContext();
        readonly ControlBitacora controlBitacora = new ControlBitacora();
        readonly ControlLogErrores controlErrores = new ControlLogErrores();

        #endregion

        #region ABM_Historico

        public bool ABMHistoricoMetodo(HistoricoPacienteDto pHistoricoPaciente, string pIDusuario)
        {
            switch (pHistoricoPaciente.EstadoABM)
            {
                case 1:
                    return InsertarHistorico(pHistoricoPaciente, pIDusuario);
            }
            return false;
        }



        /// <summary>
        /// Inserta un Historico de un Paciente
        /// </summary>
        /// <param name="pIDEmpresa">ID de la empresa</param>
        /// <param name="pIDpaciente">ID del paciente</param>
        /// <param name="ticket">Numero de Historico</param>
        /// <param name="ticket_titulo">Titulo de consulta</param>
        /// <param name="fecha_creacion">Fecha de la creacion del historico</param>
        /// <param name="pcitas_estimadas">Citas estimadas</param>
        /// <param name="pcitas_realizasadas">Citas realizadas</param>
        public bool InsertarHistorico(HistoricoPacienteDto pHistoricoPaciente, string pIDusuario)
        {
            Historico_Paciente vHistorico = new Historico_Paciente();
            vHistorico.citas_realizadas = pHistoricoPaciente.CitasRealizadas;
            vHistorico.estado = true;
            vHistorico.estimacion_citas = pHistoricoPaciente.EstimacionCitas;
            vHistorico.fecha_creacion = pHistoricoPaciente.FechaCreacion;
            vHistorico.id_empresa = pHistoricoPaciente.IdConsultorio;
            vHistorico.id_paciente = pHistoricoPaciente.IdPaciente;
            vHistorico.numero = pHistoricoPaciente.NumeroHistorico;
            vHistorico.titulo_numero = pHistoricoPaciente.TituloHistorico;
            try
            {
                dataContext.Historico_Paciente.InsertOnSubmit(vHistorico);
                dataContext.SubmitChanges();
                controlBitacora.Insertar("Se insertó un nuevo histórico", pIDusuario);
                return true;
            }
            catch (Exception ex)
            {
                controlErrores.Insertar("NConsulta", "ABMHistorico", "Insertar", ex);
                return false;
            }


        }
        /// <summary>
        /// Inserta un Historico de un Paciente
        /// </summary>
        /// <param name="pIDEmpresa">ID de la empresa</param>
        /// <param name="pIDpaciente">ID del paciente</param>
        /// <param name="ticket">Numero de Historico</param>
        /// <param name="ticket_titulo">Titulo de consulta</param>
        /// <param name="fecha_creacion">Fecha de la creacion del historico</param>
        /// <param name="pcitas_estimadas">Citas estimadas</param>
        /// <param name="pcitas_realizasadas">Citas realizadas</param>
        public bool InsertarHistoricoDetalle(HistoricoDetallePacienteDto pHistoricoDetallePaciente, string pIDusuario)
        {
            Historico_Paciente_det vHistoricoDetalle = new Historico_Paciente_det();
            vHistoricoDetalle.fecha = DateTime.Now;
            vHistoricoDetalle.id_cita = pHistoricoDetallePaciente.IdCita;
            vHistoricoDetalle.id_empresa = pHistoricoDetallePaciente.IdConsultorio;
            vHistoricoDetalle.id_paciente = pHistoricoDetallePaciente.IdPaciente;
            vHistoricoDetalle.nro_detalle = pHistoricoDetallePaciente.NumeroDetalle;
            vHistoricoDetalle.numero = pHistoricoDetallePaciente.NumeroHistorico;
            vHistoricoDetalle.trabajo_a_realizar = pHistoricoDetallePaciente.TrabajoARealizar;
            vHistoricoDetalle.trabajo_realizado = pHistoricoDetallePaciente.TrabajoRealizado;
            if (pHistoricoDetallePaciente.CerrarHistorico)
            {
                var historico = (from h in dataContext.Historico_Paciente
                                 where h.numero == pHistoricoDetallePaciente.NumeroHistorico
                                 && h.id_paciente == pHistoricoDetallePaciente.IdPaciente
                                 && h.id_empresa == pHistoricoDetallePaciente.IdConsultorio
                                 select h).FirstOrDefault();
                if (historico != null)
                    historico.estado = false;
            }

            var citaAtendida = (from c in dataContext.Cita
                                where c.idcita == pHistoricoDetallePaciente.IdCita
                                select c).FirstOrDefault();
            if (citaAtendida != null)
                citaAtendida.atendido = true;
            try
            {
                dataContext.Historico_Paciente_det.InsertOnSubmit(vHistoricoDetalle);
                dataContext.SubmitChanges();
                controlBitacora.Insertar("Se insertó un nuevo histórico", pIDusuario);
                return true;
            }
            catch (Exception ex)
            {
                controlErrores.Insertar("NConsulta", "ABMHistorico", "Insertar", ex);
                return false;
            }


        }
        /// <summary>
        /// Elimina un historico
        /// </summary>
        /// <param name="pIDPaciente">Id del paciente</param>
        /// <param name="pIDEmpresa">ID de la empresa</param>
        /// <param name="pnumero">Numro de historico</param>
        public void Eliminar(int pIDPaciente, int pIDEmpresa, int pnumero)
        {
            var sql = from e in dataContext.Historico_Paciente
                      where e.id_paciente == pIDPaciente && e.id_empresa == pIDEmpresa
                      && e.numero == pnumero
                      select e;

            if (sql.Count() > 0)
            {

                sql.First().estado = false;

                dataContext.SubmitChanges();

            }


        }

        /// <summary>
        /// Modificar un historico
        /// </summary>
        /// <param name="pIDEmpresa">ID de la empresa</param>
        /// <param name="pIDpaciente">ID del paciente</param>
        /// <param name="ticket">Numero de Historico</param>
        /// <param name="ticket_titulo">Titulo de consulta</param>
        /// <param name="fecha_creacion">Fecha de la creacion del historico</param>
        /// <param name="pcitas_estimadas">Citas estimadas</param>
        /// <param name="pcitas_realizasadas">Citas realizadas</param>
        public void Modificar(int pIDEmpresa, int pIDpaciente, int ticket, string ticket_titulo, DateTime fecha_creacion,
                            int pcitas_estimadas, int pcitas_realizasadas)
        {

            var sql = from e in dataContext.Historico_Paciente
                      where e.id_paciente == pIDpaciente && e.id_empresa == pIDEmpresa
                      select e;

            if (sql.Count() > 0)
            {
                sql.First().citas_realizadas = pcitas_realizasadas;
                sql.First().estado = true;
                sql.First().estimacion_citas = pcitas_estimadas;
                sql.First().fecha_creacion = fecha_creacion;

                sql.First().numero = ticket;
                sql.First().titulo_numero = ticket_titulo;
                dataContext.SubmitChanges();
            }



        }
        #endregion

        #region Getter_Historicos

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
        /// <summary>
        /// Metodo privado que retorna historicos de un paciente
        /// </summary>
        /// <param name="pIDpaciente">ID del paciente</param>
        /// <param name="pIDEmpresa">ID del consultorio</param>
        /// <returns>IEnumerable<Historico_Paciente></returns>
        public List<HistoricoPacienteDto> GetHistoricoPaciente(int pIDpaciente, int pIDEmpresa)
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
                        DetalleHistorico = GetHistoricoDetalle(p.id_empresa, p.id_paciente, p.numero)
                    }).ToList();
        }


        /// <summary>
        /// Metodo privado que retorna historicos del paciente segun el numero de historico
        /// </summary>
        /// <param name="pIDpaciente">ID del paciente</param>
        /// <param name="pIDEmpresa">ID del consultorio</param>
        /// <param name="pNroHistorico">Nro de historico </param>
        /// <returns>IEnumerable<Historico_Paciente></returns>
        private IEnumerable<Historico_Paciente> Get_HistoricoNro(int pIDpaciente, int pIDEmpresa, int pNroHistorico)
        {
            return from p in dataContext.Historico_Paciente
                   where p.id_empresa == pIDEmpresa && p.id_paciente == pIDpaciente &&
                   p.estado == true && p.numero == pNroHistorico
                   select p;
        }
        /// <summary>
        /// Devuelve un DataTable con un historico de un paciente segun el número de historico
        /// </summary>
        /// <param name="pIDPaciente">ID del paciente</param>
        /// <param name="pIDEmpresa">ID de la Empresa</param>
        /// <param name="pNroHistorico">Numero de historico</param>
        /// <returns>DataTable</returns>
        //public DataTable Get_HistoricosNrop(int pIDPaciente, int pIDEmpresa, int pNroHistorico)
        //{
        //    return Converter<Historico_Paciente>.Convert(Get_HistoricoNro(pIDPaciente, pIDEmpresa, pNroHistorico).ToList());
        //}
        #endregion

        #region Metodos_Auxiliares
        /// <summary>
        /// Retorna el numero del ultimo historico de un paciente
        /// </summary>
        /// <param name="pIDempresa">ID consultorio</param>
        /// <param name="pIDPaciente">ID del paciente</param>
        /// <param name="pIDUsuario">ID el usuario de consulta</param>
        /// <returns>un INT , retorna 0 en caso de que no haya ningun historico</returns>
        public int Get_Nro_Historico(int pIDempresa, int pIDPaciente, string pIDUsuario)
        {
            var sql = from h in dataContext.Historico_Paciente
                      where h.id_empresa == pIDempresa &&
                        h.id_paciente == pIDPaciente
                      orderby h.numero descending
                      select h;
            if (sql.Count() > 0)
            {
                return (int)sql.First().numero;
            }
            return 0;
        }
        #endregion
    }
}