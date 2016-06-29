﻿using System.Globalization;

namespace NAgenda
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NEventos;
    using NLogin;
    using Herramientas;
    using Datos;

    public class ABMCita
    {
        #region VariableGlobales

        readonly static DataContext dataContext = new DataContext();

        #endregion

        #region Metodos Publicos

        public static int EliminarCita(AgendaDto citaDto, string idUsuario, bool isLibre, string motivo)
        {
            var cita = (from c in dataContext.Cita
                        where c.idcita == citaDto.IdCita
                        select c).FirstOrDefault();
            if (cita != null)
            {
                if (!motivo.Equals(""))
                {
                    EnviarCorreoCancelacion(citaDto.Paciente, motivo);
                    cita.estado = false;
                    cita.libre = !isLibre;
                }
                else
                {
                    dataContext.Cita.DeleteOnSubmit(cita);
                }
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se elimino la cita", idUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NAgenda", "ABMCita", "Eliminar", ex);
                    return 0;
                }
            }
            else
                return 2;
        }

        public static int CancelarCita(string idCita, string idUsuario)
        {
            var cita = (from c in dataContext.Cita
                        where c.idcita == idCita
                        select c).FirstOrDefault();
            if (cita != null)
            {
                try
                {
                    cita.estado = false;
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se cancelo la cita", idUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NAgenda", "ABMCita", "Eliminar", ex);
                    return 0;
                }
            }
            else
                return 2;
        }

        /// <summary>
        /// Genera el codigo de la cita a reservar
        /// </summary>
        /// <param name="idCita">Nro de la cita del dia</param>
        /// <param name="fechaCita">Fecha en la que se realizara la reserva</param>
        /// <param name="idEmpresa">ID de la empresa </param>
        /// <returns>Codigo Generado segun los parametros recibidos</returns>
        public static string ObtenerCodigo(int idCita, DateTime fechaCita, int idEmpresa)
        {
            var rnd = new Random();
            var ran = rnd.Next(1, 100).ToString();
            var codigo = fechaCita.Year.ToString().Substring(2);
            if (fechaCita.Month < 10)
                codigo = codigo + "0" + fechaCita.Month.ToString();
            else
                codigo = codigo + fechaCita.Month.ToString();
            if (fechaCita.Day < 10)
                codigo = codigo + "0" + fechaCita.Day.ToString();
            else
                codigo = codigo + fechaCita.Day.ToString();
            codigo = codigo + Convert.ToString(idEmpresa);
            if (idCita < 10)
                codigo = codigo + "0" + Convert.ToString(idCita);
            else
                codigo = codigo + Convert.ToString(idCita);
            return codigo + ran;
        }

        public static bool InsertarCita(AgendaDto citaDto, string loginCliente, DateTime fechaCita, string idUsuario)
        {
            var cita = new Cita
            {
                idcita = ObtenerCodigo(citaDto.NumeroCita, fechaCita, citaDto.IDConsultorio),
                idempresa = citaDto.IDConsultorio,
                libre = false,
                id_cliente = loginCliente
            };
            var auxHora = citaDto.HoraInicioString.Split(':');
            cita.hora_inicio = new TimeSpan(Convert.ToInt32(auxHora[0]), Convert.ToInt32(auxHora[1]), 0);
            auxHora = citaDto.HoraFinString.Split(':');
            cita.hora_fin = new TimeSpan(Convert.ToInt32(auxHora[0]), Convert.ToInt32(auxHora[1]), 0);
            cita.fecha = fechaCita;
            cita.estado = true;
            cita.atendido = false;
            try
            {
                dataContext.Cita.InsertOnSubmit(cita);
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se actualizo corretamente el estado atendido", idUsuario);
                return true;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NAgenda", "ABMCita", "ActualizarAtendido", ex);
                return false;
            }
        }

        public static List<AgendaDto> ObtenerAgendaDelDia(DateTime fechaCita, int idConsultorio, int tiempoConsulta)
        {
            var query = (from c in dataContext.Cita
                         from cp in dataContext.Cliente_Paciente
                         from p in dataContext.Paciente
                         where c.idempresa == idConsultorio &&
                         c.fecha == fechaCita
                         && cp.id_usuariocliente == c.id_cliente
                         && cp.IsPrincipal == true
                         && p.id_paciente == cp.id_paciente
                         && c.estado == true
                         select new AgendaDto()
                         {
                             IdCita = c.idcita,
                             IDConsultorio = c.idempresa,
                             EstaAtendida = c.atendido ?? false,
                             HoraFin = c.hora_fin,
                             HoraInicio = c.hora_inicio,
                             LoginCliente = c.id_cliente,
                             Estalibre = c.libre
                         });

         
            var nombreDia = fechaCita.ToString("dddd",
                   new CultureInfo("es-ES"));

           
            var timeOfDay = DateTime.Now.TimeOfDay;
            var horarioConsultorio = (from h in dataContext.Horario
                                      from d in dataContext.Dia
                                      where d.iddia == h.iddia
                                      && d.descripcion.ToUpper() == nombreDia.ToUpper()
                                      && h.idempresa == idConsultorio
                                      && h.estado
                                      select h).OrderBy(o => o.hora_inicio);
            var listaRetorno = new List<AgendaDto>();
            var tiempoCita = new TimeSpan(0, tiempoConsulta, 0);
            var numeroCita = 1;
            foreach (var horario in horarioConsultorio)
            {
                var aux = horario.hora_inicio;
                while (aux < horario.hora_fin)
                {
                    var cita = query.Where(x => x.HoraInicio == aux).FirstOrDefault();
                    listaRetorno.Add(new AgendaDto()
                    {
                        HoraInicio = aux,
                        LoginCliente = cita != null ? cita.LoginCliente : "",
                        HoraFin = aux.Add(tiempoCita),
                        IDHorario = horario.idhorario,
                        IdCita = cita != null ? cita.IdCita : "",
                        IDConsultorio = idConsultorio,
                        EstaOcupada = cita != null ? cita.Estalibre ? false : true : false,
                        HoraInicioString = aux.ToString(),
                        HoraFinString = aux.Add(tiempoCita).ToString(),
                        Paciente = cita != null ? ObtenerPacienteCita(cita.LoginCliente) : null,
                        NumeroCita = numeroCita,
                        EstaAtendida = cita != null ? cita.EstaAtendida : false,
                        EsTarde = fechaCita <= DateTime.Now ? aux < timeOfDay ? true : false : false
                    });
                    aux = aux.Add(tiempoCita);
                    numeroCita++;
                }
            }
            return listaRetorno;
        }

        /// <summary>
        /// Permite cambiar el estado de una cita a ATENDIDO
        /// </summary>
        /// <param name="idCita">Id de la Cita</param>
        /// <param name="idUsuario">ID del usuario que realiza accion</param>
        /// <returns> 0- No existe cita
        /// 1- Se actualizo Correctamente
        /// 2 - No se pudo actualizar</returns>
        public static int ActualizarAtendido(string idCita, string idUsuario)
        {
            var cita = from c in dataContext.Cita
                       where c.idcita == idCita
                       select c;
            if (cita.Any())
            {
                cita.First().atendido = true;
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se actualizo corretamente el estado atendido", idUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NAgenda", "ABMCita", "ActualizarAtendido", ex);
                    return 2;
                }
            } return 0;
        }

        public static List<CitasDelClienteDto> ObtenerCitasPorCliente(string loginCliente)
        {
            return (from cita in dataContext.Cita
                    join empresa in dataContext.Empresa on cita.idempresa equals empresa.ID
                    join clinica in dataContext.Clinica on empresa.ID equals clinica.ID
                    join clinicaPaciente in dataContext.Cliente_Paciente on cita.id_cliente equals clinicaPaciente.id_usuariocliente
                    join paciente in dataContext.Paciente on clinicaPaciente.id_paciente equals paciente.id_paciente
                    where cita.estado == true && clinicaPaciente.id_usuariocliente == loginCliente
                    && empresa.Estado == true && paciente.estado == true && clinica.Estado == true && empresa.ID != 1
                    && cita.atendido == false && clinicaPaciente.IsPrincipal == true
                    select new CitasDelClienteDto()
                    {
                        LoginCliente = clinicaPaciente.id_usuariocliente,
                        NombreConsultorio = clinica.Nombre,
                        FechaString = cita.fecha.ToShortDateString(),
                        HoraInicioString = cita.hora_inicio.Hours + ":" + cita.hora_inicio.Minutes,
                        HoraFinString = cita.hora_fin.Hours + ":" + cita.hora_fin.Minutes,
                        IdCita = cita.idcita,
                        IDConsultorio = cita.idempresa,
                        NombrePaciente = paciente.nombre + " " + paciente.apellido
                    }).ToList();
        }

        #endregion

        #region Metodos Privados

        private static PacienteDto ObtenerPacienteCita(string idCliente)
        {
            return (from uc in dataContext.Cliente_Paciente
                    from p in dataContext.Paciente
                    where uc.id_usuariocliente == idCliente
                    && uc.id_paciente == p.id_paciente
                     && uc.IsPrincipal == true
                    select new PacienteDto()
                    {
                        Antecedentes = p.antecedente,
                        Ci = p.ci,
                        Direccion = p.direccion,
                        Email = p.email,
                        Estado = p.estado,
                        LoginCliente = idCliente,
                        NombrePaciente = p.nombre + " " + p.apellido,
                        Telefono = p.nro_telefono,
                        TipoSangre = p.tipo_sangre,
                        IdPaciente = p.id_paciente
                    }).FirstOrDefault();
        }

        /// <summary>
        /// Envia un correo al cliente explicando el motivo de cita cancelada
        /// </summary>
        /// <param name="pacienteDto">ID del cliente</param>
        /// <param name="motivo">Motivo de la anulacion de cita</param>
        private static void EnviarCorreoCancelacion(PacienteDto pacienteDto, string motivo)
        {
            SMTP vSMTP = new SMTP();
            vSMTP.Datos_Mensaje(pacienteDto.Email, motivo, "Cita Cancelada -  ODONTOWEB");
            vSMTP.Enviar_Mail();
        }

        #endregion
    }
}