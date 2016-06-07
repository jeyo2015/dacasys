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

        readonly DataContext dataContext = new DataContext();

        #endregion

        #region ABM_Cita


        public void ModificarCita(AgendaDto pCita, string pIDUsuario)
        {
            var vCita = (from c in dataContext.Cita
                         where c.idcita == pCita.IdCita
                         select c).FirstOrDefault();
            if (vCita != null)
            {
                var auxHora = pCita.HoraInicioString.Split(':');
                vCita.hora_inicio = new TimeSpan(Convert.ToInt32(auxHora[0]), Convert.ToInt32(auxHora[1]), 0);
                auxHora = pCita.HoraFinString.Split(':');
                vCita.hora_fin = new TimeSpan(Convert.ToInt32(auxHora[0]), Convert.ToInt32(auxHora[1]), 0);
                vCita.id_cliente = pCita.Paciente.LoginCliente;
                vCita.idempresa = pCita.IDConsultorio;
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se modifico la cita", pIDUsuario);
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

        public int EliminarCita(AgendaDto pCita, string pIDUsuario, bool plibre, String pMotivo)
        {
            var vCita = (from c in dataContext.Cita
                         where c.idcita == pCita.IdCita
                         select c).FirstOrDefault();
            if (vCita != null)
            {

                if (!pMotivo.Equals(""))
                {
                    Enviar_Correo_Cancelacion(pCita.Paciente, pIDUsuario, pMotivo);
                    vCita.estado = false;
                    vCita.libre = !plibre;


                }
                else
                {
                    dataContext.Cita.DeleteOnSubmit(vCita);
                }
                try
                {

                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se elimino la cita", pIDUsuario);

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

        #endregion

        #region Getter_Citas

        /// <summary>
        /// Genera el codigo de la cita a reservar
        /// </summary>
        /// <param name="pNro_cita">Nro de la cita del dia</param>
        /// <param name="pFechaCita">Fecha en la que se realizara la reserva</param>
        /// <param name="pIDEmpresa">ID de la empresa </param>
        /// <returns>Codigo Generado segun los parametros recibidos</returns>
        public string Obtener_Codigo(int pNro_cita, DateTime pFechaCita, int pIDEmpresa)
        {
            Random rnd = new Random();
            string ran = rnd.Next(1, 100).ToString();
            string codigo = pFechaCita.Year.ToString().Substring(2);
            if (pFechaCita.Month < 10)
                codigo = codigo + "0" + pFechaCita.Month.ToString();
            else
                codigo = codigo + pFechaCita.Month.ToString();
            if (pFechaCita.Day < 10)
                codigo = codigo + "0" + pFechaCita.Day.ToString();
            else
                codigo = codigo + pFechaCita.Day.ToString();
            codigo = codigo + Convert.ToString(pIDEmpresa);
            if (pNro_cita < 10)
                codigo = codigo + "0" + Convert.ToString(pNro_cita);
            else
                codigo = codigo + Convert.ToString(pNro_cita);
            return codigo + ran;
        }

        public bool InsertarCita(AgendaDto cita, string pLogingCliente, DateTime pfechaCita, string pIDUsuario)
        {
            Cita vCita = new Cita();
            vCita.idcita = Obtener_Codigo(cita.NumeroCita, pfechaCita, cita.IDConsultorio);
            vCita.idempresa = cita.IDConsultorio;
            vCita.libre = false;
            vCita.id_cliente = pLogingCliente;
            var auxHora = cita.HoraInicioString.Split(':');
            vCita.hora_inicio = new TimeSpan(Convert.ToInt32(auxHora[0]), Convert.ToInt32(auxHora[1]), 0);
            auxHora = cita.HoraFinString.Split(':');
            vCita.hora_fin = new TimeSpan(Convert.ToInt32(auxHora[0]), Convert.ToInt32(auxHora[1]), 0);
            vCita.fecha = pfechaCita;
            vCita.estado = true;
            vCita.atendido = false;
            try
            {
                dataContext.Cita.InsertOnSubmit(vCita);
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se actualizo corretamente el estado atendido", pIDUsuario);
                return true;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NAgenda", "ABMCita", "Actualizar_Atendido", ex);
                return false;
            }
        }


        public List<AgendaDto> GetAgendaDelDia(DateTime pFecha, int pIDConsultorio, int tiempoConsulta)
        {

            var query = (from c in dataContext.Cita
                         from cp in dataContext.Cliente_Paciente
                         from p in dataContext.Paciente
                         where c.idempresa == pIDConsultorio &&
                         c.fecha == pFecha
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
            var vDia = (int)pFecha.DayOfWeek ;
            if (vDia == -1)
                vDia = 6;
            var timeOfDay = DateTime.Now.TimeOfDay;
            var vHorarioConsultorio = (from h in dataContext.Horario
                                       where h.iddia == vDia
                                       && h.idempresa == pIDConsultorio
                                       && h.estado 
                                       select h).OrderBy(o => o.hora_inicio);
            var listaRetorno = new List<AgendaDto>();
            var tiempoCita = new TimeSpan(0, tiempoConsulta, 0);
            var numeroCita = 1;
            foreach (var horario in vHorarioConsultorio)
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
                        IDConsultorio = pIDConsultorio,
                        EstaOcupada = cita != null ? cita.Estalibre ? false : true : false,
                        HoraInicioString = aux.ToString(),
                        HoraFinString = aux.Add(tiempoCita).ToString(),
                        Paciente = cita != null ?  GetPacienteCita(cita.LoginCliente) : null,
                        NumeroCita = numeroCita,
                        EstaAtendida = cita != null ? cita.EstaAtendida : false,
                        EsTarde = pFecha <= DateTime.Now ? aux < timeOfDay ? true : false : false
                    });
                    aux = aux.Add(tiempoCita);
                    numeroCita++;
                }
            }

            return listaRetorno;

        }

        private PacienteDto GetPacienteCita(string pIDCliente)
        {
            return (from uc in dataContext.Cliente_Paciente
                    from p in dataContext.Paciente
                    where uc.id_usuariocliente == pIDCliente
                    && uc.id_paciente == p.id_paciente
                     && uc.IsPrincipal == true
                    select new PacienteDto()
                    {
                        Antecedentes = p.antecedente,
                        Ci = p.ci,
                        Direccion = p.direccion,
                        Email = p.email,
                        Estado = p.estado,
                        LoginCliente = pIDCliente,
                        NombrePaciente = p.nombre + " " + p.apellido,
                        Telefono = p.nro_telefono,
                        TipoSangre = p.tipo_sangre,
                        IdPaciente = p.id_paciente
                    }).FirstOrDefault();
        }
        #endregion

        #region Metodos_Auxiliares
        /// <summary>
        /// Envia un correo al cliente explicando el motivo de cita cancelada
        /// </summary>
        /// <param name="pIDcliente">ID del cliente</param>
        /// <param name="pIDUsuario">ID del usuario que cancela cita</param>
        /// <param name="pMotivo">Motivo de la anulacion de cita</param>
        private void Enviar_Correo_Cancelacion(PacienteDto pPaciente, string pIDUsuario, String pMotivo)
        {
            SMTP vSMTP = new SMTP();
            vSMTP.Datos_Mensaje(pPaciente.Email, pMotivo, "Cita Cancelada -  ODONTOWEB");
            vSMTP.Enviar_Mail();


        }
        /// <summary>
        /// Devuelve el parametro de sistema, la diferencia de hora con el servidor
        /// </summary>
        /// <returns></returns>
        public int Get_DirefenciaHora()
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
        /// <param name="pfecha">Fecha de la cita</param>
        /// <param name="phorai">Hora inicio de la cita</param>
        /// <returns> True - Si esta disponible
        /// False - No esta disponible</returns>
        public bool Se_puede_Cliente(DateTime pfecha, TimeSpan phorai)
        {
            DateTime aux = DateTime.Now.AddHours(Get_DirefenciaHora());
            if (aux.ToShortDateString().CompareTo(pfecha.ToShortDateString()) == 0)
            {
                if (aux.TimeOfDay.CompareTo(phorai) <= 0)
                    return true;
            }
            if (aux.CompareTo(pfecha) < 0)
                return true;
            return false;
        }
        /// <summary>
        /// Metodo que verifica si la fecha y hora estan disponibles, segun el tiempo
        /// </summary>
        /// <param name="pfecha">Fecha para la cita</param>
        /// <param name="phorai">Hora incio de la cita</param>
        /// <returns> True - Si esta disponible
        /// False - No esta disponible</returns>
        public bool Se_puede(DateTime pfecha, TimeSpan phorai)
        {
            DateTime aux = DateTime.Now.AddHours(Get_DirefenciaHora());
            if (aux.ToShortDateString().CompareTo(pfecha.ToShortDateString()) == 0)
            {
                return true;
            }
            if (aux.CompareTo(pfecha) < 0)
                return true;
            return false;
        }

        /// <summary>
        /// Verifica si la cita esta a tiempo para ser atendida
        /// </summary>
        /// <param name="pfecha">Fecha de la cita</param>
        /// <param name="phorai">Hora incio de la cita</param>
        /// <returns>True - Si esta disponible
        /// False - No esta disponible</returns>
        public bool Esta_a_Tiempo(DateTime pfecha, TimeSpan phorai)
        {
            DateTime aux = DateTime.Now.AddHours(Get_DirefenciaHora());
            if (aux.ToShortDateString().CompareTo(pfecha.ToShortDateString()) == 0)
            {
                TimeSpan vHoraf = new TimeSpan(phorai.Hours + 1, phorai.Minutes, 00);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Permite cambiar el estado de una cita a ATENDIDO
        /// </summary>
        /// <param name="pIDCita">Id de la Cita</param>
        /// <param name="pIDUsuario">ID del usuario que realiza accion</param>
        /// <returns> 0- No existe cita
        /// 1- Se actualizo Correctamente
        /// 2 - No se pudo actualizar</returns>
        public int Actualizar_Atendido(string pIDCita, string pIDUsuario)
        {
            var cita = from c in dataContext.Cita
                       where c.idcita == pIDCita
                       select c;
            if (cita.Count() > 0)
            {
                cita.First().atendido = true;
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se actualizo corretamente el estado atendido", pIDUsuario);
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
        public bool No_es_Atendido(string pIDCita)
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
        #endregion
    }
}