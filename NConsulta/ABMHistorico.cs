using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NEventos;
using DataTableConverter;
using System.Data;
using NLogin;
using DConsulta;
using Herramientas;

namespace NConsulta
{
    public class ABMHistorico
    {
        #region VariableGlobales
        DConsultaDataContext gDc = new DConsultaDataContext();

        ControlBitacora gCb = new ControlBitacora();
        ControlLogErrores gLe = new ControlLogErrores();

        #endregion

        #region ABM_Paciente

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
                gDc.Historico_Paciente.InsertOnSubmit(vHistorico);
                gDc.SubmitChanges();
                gCb.Insertar("Se insertó un nuevo histórico", pIDusuario);
                return true;
            }
            catch (Exception ex)
            {
                gLe.Insertar("NConsulta", "ABMHistorico", "Insertar", ex);
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
            var sql = from e in gDc.Historico_Paciente
                      where e.id_paciente == pIDPaciente && e.id_empresa == pIDEmpresa
                      && e.numero == pnumero
                      select e;

            if (sql.Count() > 0)
            {

                sql.First().estado = false;

                gDc.SubmitChanges();

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

            var sql = from e in gDc.Historico_Paciente
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
                gDc.SubmitChanges();
            }



        }
        #endregion
        #region Getter_Historicos
        /// <summary>
        /// Metodo privado que retorna historicos de un paciente
        /// </summary>
        /// <param name="pIDpaciente">ID del paciente</param>
        /// <param name="pIDEmpresa">ID del consultorio</param>
        /// <returns>IEnumerable<Historico_Paciente></returns>
        public List<HistoricoPacienteDto> GetHistoricoPaciente(int pIDpaciente, int pIDEmpresa)
        {
            return (from p in gDc.Historico_Paciente
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
                        EstadoABM = 0
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
            return from p in gDc.Historico_Paciente
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
        public DataTable Get_HistoricosNrop(int pIDPaciente, int pIDEmpresa, int pNroHistorico)
        {
            return Converter<Historico_Paciente>.Convert(Get_HistoricoNro(pIDPaciente, pIDEmpresa, pNroHistorico).ToList());
        }
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
            var sql = from h in gDc.Historico_Paciente
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
