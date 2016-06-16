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

        public static void ModificarCita(AgendaDto citaDto, string idUsuario)
        {
            var cita = (from c in dataContext.Cita
                        where c.idcita == citaDto.IdCita
                        select c).FirstOrDefault();
            if (cita != null)
            {
                var auxHora = citaDto.HoraInicioString.Split(':');
                cita.hora_inicio = new TimeSpan(Convert.ToInt32(auxHora[0]), Convert.ToInt32(auxHora[1]), 0);
                auxHora = citaDto.HoraFinString.Split(':');
                cita.hora_fin = new TimeSpan(Convert.ToInt32(auxHora[0]), Convert.ToInt32(auxHora[1]), 0);
                cita.id_cliente = citaDto.Paciente.LoginCliente;
                cita.idempresa = citaDto.IDConsultorio;
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se modifico la cita", idUsuario);
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NAgenda", "ABMCita", "Modificar", ex);
                }
            }
            else
            {
                ControlLogErrores.Insertar("NAgenda", "ABMCita", "Modificar, no se pudo obtener el horario", null);
            }
        }

        public static int EliminarCita(AgendaDto citaDto, string idUsuario, bool isLibre, string motivo)
        {
            var cita = (from c in dataContext.Cita
                        where c.idcita == citaDto.IdCita
                        select c).FirstOrDefault();
            if (cita != null)
            {
                if (!motivo.Equals(""))
                {
                    Enviar_Correo_Cancelacion(citaDto.Paciente, idUsuario, motivo);
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
        public static string Obtener_Codigo(int idCita, DateTime fechaCita, int idEmpresa)
        {
            Random rnd = new Random();
            string ran = rnd.Next(1, 100).ToString();
            string codigo = fechaCita.Year.ToString().Substring(2);
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
            Cita cita = new Cita();
            cita.idcita = Obtener_Codigo(citaDto.NumeroCita, fechaCita, citaDto.IDConsultorio);
            cita.idempresa = citaDto.IDConsultorio;
            cita.libre = false;
            cita.id_cliente = loginCliente;
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
                ControlLogErrores.Insertar("NAgenda", "ABMCita", "Actualizar_Atendido", ex);
                return false;
            }
        }

        public static List<AgendaDto> GetAgendaDelDia(DateTime fechaCita, int idConsultorio, int tiempoConsulta)
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
            var dia = (int)fechaCita.DayOfWeek;
            if (dia == -1)
                dia = 6;
            var timeOfDay = DateTime.Now.TimeOfDay;
            var horarioConsultorio = (from h in dataContext.Horario
                                      where h.iddia == dia
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
                        Paciente = cita != null ? GetPacienteCita(cita.LoginCliente) : null,
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
        /// Devuelve el parametro de sistema, la diferencia de hora con el servidor
        /// </summary>
        /// <returns></returns>
        public static int Get_DirefenciaHora()
        {
            var sql = from p in dataContext.ParametroSistemas
                      where p.Elemento == "DiferenciaHora"
                      select p;
            if (sql.Count() > 0)
            {
                return (int)sql.First().ValorI;
            }
            return 0;
        }

        /// <summary>
        /// Metodo que verifica si la fecha y hora estan disponibles, si no han pasado
        /// </summary>
        /// <param name="fechaCita">Fecha de la cita</param>
        /// <param name="horaInicio">Hora inicio de la cita</param>
        /// <returns> True - Si esta disponible
        /// False - No esta disponible</returns>
        public static bool Se_puede_Cliente(DateTime fechaCita, TimeSpan horaInicio)
        {
            DateTime aux = DateTime.Now.AddHours(Get_DirefenciaHora());
            if (aux.ToShortDateString().CompareTo(fechaCita.ToShortDateString()) == 0)
            {
                if (aux.TimeOfDay.CompareTo(horaInicio) <= 0)
                    return true;
            }
            if (aux.CompareTo(fechaCita) < 0)
                return true;
            return false;
        }

        /// <summary>
        /// Metodo que verifica si la fecha y hora estan disponibles, segun el tiempo
        /// </summary>
        /// <param name="fechaCita">Fecha para la cita</param>
        /// <param name="horaInicio">Hora incio de la cita</param>
        /// <returns> True - Si esta disponible
        /// False - No esta disponible</returns>
        public static bool Se_puede(DateTime fechaCita, TimeSpan horaInicio)
        {
            DateTime aux = DateTime.Now.AddHours(Get_DirefenciaHora());
            if (aux.ToShortDateString().CompareTo(fechaCita.ToShortDateString()) == 0)
            {
                return true;
            }
            if (aux.CompareTo(fechaCita) < 0)
                return true;
            return false;
        }

        /// <summary>
        /// Verifica si la cita esta a tiempo para ser atendida
        /// </summary>
        /// <param name="fechaInicio">Fecha de la cita</param>
        /// <param name="horaInicio">Hora incio de la cita</param>
        /// <returns>True - Si esta disponible
        /// False - No esta disponible</returns>
        public static bool Esta_a_Tiempo(DateTime fechaInicio, TimeSpan horaInicio)
        {
            DateTime aux = DateTime.Now.AddHours(Get_DirefenciaHora());
            if (aux.ToShortDateString().CompareTo(fechaInicio.ToShortDateString()) == 0)
            {
                TimeSpan horaFinal = new TimeSpan(horaInicio.Hours + 1, horaInicio.Minutes, 00);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Permite cambiar el estado de una cita a ATENDIDO
        /// </summary>
        /// <param name="idCita">Id de la Cita</param>
        /// <param name="idUsuario">ID del usuario que realiza accion</param>
        /// <returns> 0- No existe cita
        /// 1- Se actualizo Correctamente
        /// 2 - No se pudo actualizar</returns>
        public static int Actualizar_Atendido(string idCita, string idUsuario)
        {
            var cita = from c in dataContext.Cita
                       where c.idcita == idCita
                       select c;
            if (cita.Count() > 0)
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
                    ControlLogErrores.Insertar("NAgenda", "ABMCita", "Actualizar_Atendido", ex);
                    return 2;
                }
            } return 0;
        }

        /// <summary>
        /// Verifica si la cita no esta atendida
        /// </summary>
        /// <param name="pIDCita">Codigo de la cita</param>
        /// <returns>True - Si esta atendida
        /// False - No esta atendida</returns>
        public static bool No_es_Atendido(string pIDCita)
        {
            var sql = from c in dataContext.Cita
                      where c.idcita == pIDCita
                      select c;
            if (sql.Count() > 0)
            {
                return (bool)sql.First().atendido;
            }
            return false;
        }

        // TODO Fix this function.
        public static List<CitasDelClienteDto> ObtenerCitasPorCliente(string loginCliente)
        {
            return (from cita in dataContext.Cita
                    join empresa in dataContext.Empresa on cita.idempresa equals empresa.ID
                    join clinica in dataContext.Clinica on empresa.ID equals clinica.ID
                    join clinicaPaciente in dataContext.Cliente_Paciente on cita.id_cliente equals clinicaPaciente.id_usuariocliente
                    join paciente in dataContext.Paciente on clinicaPaciente.id_paciente equals paciente.id_paciente
                    where cita.estado == true && clinicaPaciente.id_usuariocliente == loginCliente
                    && empresa.Estado == true && paciente.estado == true //&& clinica.Estado == true && empresa.ID != 1
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

        private static PacienteDto GetPacienteCita(string idCliente)
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
        /// <param name="pIDcliente">ID del cliente</param>
        /// <param name="idUsuario">ID del usuario que cancela cita</param>
        /// <param name="motivo">Motivo de la anulacion de cita</param>
        private static void Enviar_Correo_Cancelacion(PacienteDto pacienteDto, string idUsuario, string motivo)
        {
            SMTP vSMTP = new SMTP();
            vSMTP.Datos_Mensaje(pacienteDto.Email, motivo, "Cita Cancelada -  ODONTOWEB");
            vSMTP.Enviar_Mail();


        }

        #endregion
    }
}