using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAgenda;
using NEventos;
using DConsulta;
using DataTableConverter;
using System.Data;
using NLogin;
namespace NAgenda
{
    public class ABMCita
    {
        #region VariableGlobales
        DAgendaDataContext gDc = new DAgendaDataContext();
        ControlBitacora gCb = new ControlBitacora();
        ControlLogErrores gLe = new ControlLogErrores();
        #endregion

        #region ABM_Cita

        /// <summary>
        /// Inserta una Cita en la Agenda
        /// </summary>
        /// <param name="pIDcita">ID de la cita</param>
        /// <param name="pFecha">Fecha de la cita</param>
        /// <param name="pHoraIni">Hora inicio de la cita</param>
        /// <param name="pHoraFin">Hora fin de la cita</param>
        /// <param name="pIDClienteEmpresa">ID del Cliente</param>
        /// <param name="pIDEmpresa">Id de la empresa</param>
        /// <param name="pIDUsuario">ID del usuario que realiza la accion</param>
        public int Insertar(string pIDcita, DateTime pFecha, TimeSpan pHoraIni, TimeSpan pHoraFin,
                                string pIDClienteEmpresa, int pIDEmpresa, string pIDUsuario)
        {
            var sql = from c in gDc.Cita
                      where c.idcita == pIDcita
                      && c.id_cliente == pIDClienteEmpresa
                      select c;
            if (sql.Count() > 0)
            { //true== Se elimino la cita anteriormente del mismo cliente(por error talves)
                ///entonces se cambia el estado y libre
                sql.First().estado = true;
                sql.First().libre = false;
                try
                {
                    gDc.SubmitChanges();
                    gDc.SubmitChanges();
                    gCb.Insertar("Se inserto una cita, ya eliminada", pIDUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    gLe.Insertar("NAgenda", "ABMCita", "Insertar", ex);
                    return 0;
                }
            }
            else {// Se crea una cita normal.
                Cita vCita = new Cita();
                vCita.estado = true;
                vCita.fecha = pFecha;
                vCita.hora_fin = pHoraFin;
                vCita.hora_inicio = pHoraIni;
                vCita.id_cliente = pIDClienteEmpresa;
                vCita.idempresa = pIDEmpresa;
                vCita.idcita = pIDcita;
                vCita.libre = false;
                vCita.atendido = false;
                try
                {
                    gDc.Cita.InsertOnSubmit(vCita);
                    gDc.SubmitChanges();
                    gCb.Insertar("Se inserto una cita", pIDUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    gLe.Insertar("NAgenda", "ABMCita", "Insertar", ex);
                    return 0;
                }
            
            }
           
        }

        /// <summary>
        /// Modifica una Cita especifica
        /// </summary>
        /// <param name="pIDcita">ID de la cita</param>
        /// <param name="pFecha">Fecha de la cita</param>
        /// <param name="pHoraIni">Hora inicio de la cita</param>
        /// <param name="pHoraFin">Hora fin de la cita</param>
        /// <param name="pIDClienteEmpresa">ID del Cliente</param>
        /// <param name="pIDEmpresa">Id de la empresa</param>
        /// <param name="pIDUsuario">ID del usuario que realiza la accion</param>
        public void Modificar(string pIDcita, DateTime pFecha, TimeSpan pHoraIni, TimeSpan pHoraFin,
                               string pIDClienteEmpresa, int pIDEmpresa, string pIDUsuario)
        {
            var sql = from c in gDc.Cita
                      where c.idcita == pIDcita
                      select c;
            if (sql.Count() > 0)
            {
                sql.First().hora_fin = pHoraFin;
                sql.First().hora_inicio = pHoraIni;
                sql.First().id_cliente = pIDClienteEmpresa;
                sql.First().idempresa = pIDEmpresa;
                try
                {
                    gDc.SubmitChanges();
                    gCb.Insertar("Se modifico la cita", pIDUsuario);
                }
                catch (Exception ex)
                {
                    gLe.Insertar("NAgenda", "ABMCita", "Modificar", ex);
                }

            }
            else
            {
                gLe.Insertar("NAgenda", "ABMCita", "Modificar, no se pudo obtener el horario", null);
            }

        }
        /// <summary>
        /// Elimina una cita
        /// </summary>
        /// <param name="pIDCita">ID de la cita</param>
        /// <param name="pIDUsuario">ID del Usuario</param>
        /// <param name="plibre">True, queda libre la hora False, queda ocupado</param>
        /// <returns> 1 - Se Elimino cita
        ///           0 - No se elimino cita
        ///           2 - No exite cita </returns>
        public int Eliminar(string pIDCita, string pIDUsuario, bool plibre, String pMotivo)
        {
            var sql = from c in gDc.Cita
                      where c.idcita == pIDCita
                      select c;
            if (sql.Count() > 0)
            {

                if (!pMotivo.Equals(""))
                {
                    Enviar_Correo_Cancelacion(sql.First().id_cliente, pIDUsuario, pMotivo);
                    sql.First().estado = false;
                    sql.First().libre = plibre;
                   

                }
                else {
                    gDc.Cita.DeleteOnSubmit(sql.First());
                }
                try
                    { 
                       
                        gDc.SubmitChanges();
                        gCb.Insertar("Se elimino la cita", pIDUsuario);

                        return 1;
                    }
                    catch (Exception ex)
                    {
                        gLe.Insertar("NAgenda", "ABMCita", "Eliminar", ex);
                        return 0;
                    }

            }
            else
                return 2;
        }

        #endregion
        #region Getter_Citas
        /// <summary>
        /// Retorna un List con las citas de un cliente, no atendidas a partir de la fecha actul
        /// </summary>
        /// <param name="pLoginUsuario">Login del Cliente</param>
        /// <returns></returns>
        public List<get_miscitasclientResult> Get_CitasClientpList(String pLoginUsuario)
        {
            return Get_CitasClient(pLoginUsuario).ToList();
        }

        /// <summary>
        /// Metodo privado que obtiene todas las citas de un cliente, no atendidas y a partir de la fecha actual
        /// </summary>
        /// <param name="pLoginClient">ID Login del Cliente</param>
        /// <returns></returns>
        private IEnumerable<get_miscitasclientResult> Get_CitasClient(string pLoginClient)
        {
            return gDc.get_miscitasclient(pLoginClient);

                        
                   
        }
        /// <summary>
        /// Devuelve todas las citas de un cliente, no atendidas y a partir de la fecha actual
        /// </summary>
        /// <param name="pLoginClient">ID Login del cliente</param>
        /// <returns></returns>
        public DataTable Get_CitasClientp(string pLoginClient)
        {
            return Converter<get_miscitasclientResult>.Convert(Get_CitasClient(pLoginClient) .ToList());
        }

        /// <summary>
        /// Metodo primado que obiente citas de una empresa segun la fecha
        /// </summary>
        /// <param name="pFecha">Fecha de las citas</param>
        /// <param name="pidEmpresa">ID de la citas</param>
        /// <returns>IEnumerble de CITA</returns>
        private IEnumerable<Cita> Get_Citas(DateTime pFecha, int pidEmpresa)
        {
            return from h in gDc.Cita
                   where h.idempresa == pidEmpresa && h.fecha == pFecha
                   select h;
        }

        /// <summary>
        /// Devuelve las citas de la Fecha especificada
        /// </summary>
        /// <param name="pFecha">Fecha de las Citas</param>
        /// <param name="ipdEmpresa">ID de la empresa</param>
        /// <returns> Un DataTable que contiene las citas</returns>
        public DataTable Get_Citasp(DateTime pFecha, int ipdEmpresa)
        {
            return Converter<Cita>.Convert(Get_Citas(pFecha, ipdEmpresa).ToList());
        }

        /// <summary>
        /// Metodo privado que retorna una cita de una empresa segun la fecha y hora de inicio
        /// </summary>
        /// <param name="pFecha">Fecha </param>
        /// <param name="phoraini">Hora incio de la cita</param>
        /// <param name="pidEmpresa">ID de la empresa</param>
        /// <returns>IEnumerable<Cita></returns>
        private IEnumerable<Cita> Get_Cita(DateTime pFecha, TimeSpan phoraini, int pidEmpresa)
        {
            return from h in gDc.Cita
                   where h.idempresa == pidEmpresa && h.fecha == pFecha
                         && h.hora_inicio == phoraini && h.libre == false
                   select h;
        }

        /// <summary>
        /// Devuelve una cita especifica, segun fecha y Horainicio
        /// </summary>
        /// <param name="pFecha"> Fecha de las Citas</param>
        /// <param name="pHoraini"> Hora de inicio de la Cita</param>
        /// <param name="ipdEmpresa">ID de la empresa</param>
        /// <returns> Un DataTable que contiene la cita</returns>
        public DataTable Get_Citap(DateTime pFecha, TimeSpan pHoraini, int ipdEmpresa)
        {
            return Converter<Cita>.Convert(Get_Cita(pFecha, pHoraini, ipdEmpresa).ToList());
        }

        /// <summary>
        /// Metodo privado que retorna una cita segun su codigo
        /// </summary>
        /// <param name="pIDcita">Codigo de la cita</param>
        /// <returns>IEnumerable<Cita> </returns>
        private IEnumerable<Cita> Get_Cita_Codigo(String pIDcita)
        {
            return from h in gDc.Cita
                   where h.idcita == pIDcita && h.estado == true
                   select h;
        }

        /// <summary>
        /// Devuelve Cita segun su codigo
        /// </summary>
        /// <param name="pIDCita">ID de la cita a buscar</param>
        /// <returns>un DataTable con la cita</returns>
        public DataTable Get_Cita_Codigop(String pIDCita)
        {
            return Converter<Cita>.Convert(Get_Cita_Codigo(pIDCita).ToList());
        }



        /// <summary>
        /// Obtiene el Id del cliente de una cita programada
        /// </summary>
        /// <param name="pcodigocita">ID de la cita</param>
        /// <returns>El codigo de la cita</returns>
        public String Obtener_id_cliente(string pcodigocita)
        {
            var sql = from c in gDc.Cita
                      where c.idcita == pcodigocita
                      select c;
            String id_cliente = "";
            if (sql.Count() > 0)
            {
                id_cliente = sql.First().id_cliente;
            }

            return id_cliente;
        }

        /// <summary>
        /// Genera el codigo de la cita a reservar
        /// </summary>
        /// <param name="pNro_cita">Nro de la cita del dia</param>
        /// <param name="pFechaCita">Fecha en la que se realizara la reserva</param>
        /// <param name="pIDEmpresa">ID de la empresa </param>
        /// <returns>Codigo Generado segun los parametros recibidos</returns>
        public string Obtener_Codigo(int pNro_cita, DateTime pFechaCita, int pIDEmpresa)
        {
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
            return codigo;
        }
        #endregion
        #region Metodos_Auxiliares
        /// <summary>
        /// Envia un correo al cliente explicando el motivo de cita cancelada
        /// </summary>
        /// <param name="pIDcliente">ID del cliente</param>
        /// <param name="pIDUsuario">ID del usuario que cancela cita</param>
        /// <param name="pMotivo">Motivo de la anulacion de cita</param>
        private void Enviar_Correo_Cancelacion(string pIDcliente, string pIDUsuario, String pMotivo)
        {
            var paciente = from pac in gDc.Paciente
                           join cliente in gDc.UsuarioCliente on
                           pac.ci equals cliente.Login
                           where cliente.Login == pIDcliente && cliente.Estado == true
                           select pac;
            if (paciente.Count() > 0)
            {

                SMTP vSMTP = new SMTP();
                vSMTP.Datos_Mensaje(paciente.First().email, pMotivo, "Cita Cancelada - MEDIWEB");
                vSMTP.Enviar_Mail();

            }


        }
        /// <summary>
        /// Devuelve el parametro de sistema, la diferencia de hora con el servidor
        /// </summary>
        /// <returns></returns>
        public int Get_DirefenciaHora()
        {
            var sql = from p in gDc.ParametroSistemas
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
            var cita = from c in gDc.Cita
                       where c.idcita == pIDCita
                       select c;
            if (cita.Count() > 0)
            {
                cita.First().atendido = true;
                try
                {
                    gDc.SubmitChanges();
                    gCb.Insertar("Se actualizo corretamente el estado atendido", pIDUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    gLe.Insertar("NAgenda", "ABMCita", "Actualizar_Atendido", ex);
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

            var sql = from c in gDc.Cita
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
