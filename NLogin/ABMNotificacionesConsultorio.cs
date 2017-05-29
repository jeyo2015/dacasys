using System.Diagnostics;
using System.Security.Cryptography;

namespace NLogin
{
    using System;
    using System.Linq;
    using Datos;
    using NEventos;
    using Herramientas;

    public class ABMNotificacionesConsultorio
    {
        #region VariablesGlobales

        readonly static ContextoDataContext dataContext = new ContextoDataContext();

        #endregion

        #region Metodos Publicos

        public static NotificacionesConsultorioNewDto ObtenerNotificacionesPendientes(int idConsultorio, int idTipoNotificacion)
        {
            var query = (from no in dataContext.NotificacionConsultorio
                         from cp in dataContext.Cliente_Paciente
                         from p in dataContext.Paciente
                         where no.Estado != 0 &&
                         no.TipoNotificacion == 1
                         && cp.id_usuariocliente == no.LoginUsuario
                         && no.IDConsultorio == idConsultorio
                         && p.id_paciente == cp.id_paciente
                         && cp.IsPrincipal == true
                         select new NotificacionesConsultorioDto()
                         {
                             LoginUsuario = no.LoginUsuario,
                             FechaNotificacion = no.Fecha,
                             EstadoNotificacion = no.Estado,
                             IDConsultorio = no.IDConsultorio,
                             IDNotificacion = no.ID,
                             NombreUsuario = p.nombre + ' ' + p.apellido
                         }).OrderBy(x => x.FechaNotificacion).ToList();
            return new NotificacionesConsultorioNewDto()
            {
                Notificaciones = query,
                CantidadNuevasNotificaciones = query.Where(x => x.EstadoNotificacion == 1).Count()
            };
        }

        public static bool DeshabilitarNuevasNotificaciones(int idConsultorio, int idTipoNotificacion)
        {
            var query = (from no in dataContext.NotificacionConsultorio
                         where no.IDConsultorio == idConsultorio
                         && no.TipoNotificacion == idTipoNotificacion
                         && no.Estado == 1
                         select no).ToList();


            foreach (var notificacion in query)
            {
                notificacion.Estado = 2;

            }
            try
            {
                dataContext.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMNotificacionesConsultorio", "DeshabilitarNuevasNotificaciones", ex);
                return false;
            }
        }

        public static bool AceptarSolicitudPaciente(NotificacionesConsultorioDto notificacionesConsultorioDto)
        {
            var query = (from cp in dataContext.Empresa_Cliente
                         where cp.id_empresa == notificacionesConsultorioDto.IDConsultorio
                         && cp.id_usuariocliente == notificacionesConsultorioDto.LoginUsuario
                         select cp).FirstOrDefault();
            if (query == null)
            {
                Empresa_Cliente ep = new Empresa_Cliente();
                ep.id_usuariocliente = notificacionesConsultorioDto.LoginUsuario;
                ep.id_empresa = notificacionesConsultorioDto.IDConsultorio;
                dataContext.Empresa_Cliente.InsertOnSubmit(ep);
                var notificacion = (from no in dataContext.NotificacionConsultorio
                                    where no.ID == notificacionesConsultorioDto.IDNotificacion
                                    select no).FirstOrDefault();
                if (notificacion != null)
                    notificacion.Estado = 0;
            }

            try
            {
                dataContext.SubmitChanges();
                EnviarNotificacionAceptarSolicitud(notificacionesConsultorioDto.IDConsultorio, notificacionesConsultorioDto.LoginUsuario);
                return true;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMNotificacionesConsultorio", "AceptarSolicitudPaciente", ex);
                return false;
            }
        }

      

        public static bool CancelarSolicitudPaciente(NotificacionesConsultorioDto notificacionesConsultorioDto)
        {

            var notificacion = (from no in dataContext.NotificacionConsultorio
                                where no.ID == notificacionesConsultorioDto.IDNotificacion
                                select no).FirstOrDefault();
            if (notificacion != null)
                notificacion.Estado = 0;

            try
            {
                dataContext.SubmitChanges();
                
                EnviarNotificacionRechazoSolicitud(notificacion.IDConsultorio, notificacion.LoginUsuario);
                return true;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMNotificacionesConsultorio", "AceptarSolicitudPaciente", ex);
                return false;
            }
        }

        private static void EnviarNotificacionRechazoSolicitud(int idConsultorio, string loginUsuario)
        {
            ConsultorioDto consultorio = ABMEmpresa.ObtenerConsultorioPorId(idConsultorio);
            PacienteDto user = ABMUsuarioCliente.ObtenerDatoPaciente(loginUsuario);
            if (consultorio == null || user == null) return;
            SMTP vSMTP = new SMTP();
            String vMensaje ="Su solicitud enviada al consultorio "+  consultorio.NombreClinica + " fue rechazada. \nSaludos,\nOdontoweb";
            vSMTP.Datos_Mensaje(user.Email, vMensaje, "Solicitud rechazada");
            vSMTP.Enviar_Mail();
        }
        private static void EnviarNotificacionAceptarSolicitud(int idConsultorio, string loginUsuario)
        {
            ConsultorioDto consultorio = ABMEmpresa.ObtenerConsultorioPorId(idConsultorio);
            PacienteDto user = ABMUsuarioCliente.ObtenerDatoPaciente(loginUsuario);
            if (consultorio == null || user == null) return;
            SMTP vSMTP = new SMTP();
           
            String vMensaje = "Su solicitud al consultorio " + consultorio.NombreClinica + " ha sido aceptada. " +
                                           "\nSaludos,\nOdontoweb";
            vSMTP.Datos_Mensaje(user.Email, vMensaje, "Solicitud aceptada");
            vSMTP.Enviar_Mail();
        }
        public static bool EnviarSolicitudConsultorio(NotificacionesConsultorioDto notificacionesConsultorioDto)
        {
            var notificacion = new NotificacionConsultorio();
            notificacion.Estado = 1;
            notificacion.Fecha = DateTime.Now;
            notificacion.IDConsultorio = notificacionesConsultorioDto.IDConsultorio;
            notificacion.LoginUsuario = notificacionesConsultorioDto.LoginUsuario;
            notificacion.TipoNotificacion = 1;
            try
            {
                dataContext.NotificacionConsultorio.InsertOnSubmit(notificacion);
                dataContext.SubmitChanges();
                //enviar notificacion
                EnviarNotificacion(notificacionesConsultorioDto.IDConsultorio, notificacionesConsultorioDto.LoginUsuario);
                ControlBitacora.Insertar("Se envio una notificacion", notificacionesConsultorioDto.LoginUsuario);
                return true;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMNotificaciones", "EnviarSolicitudConsultorio", ex);
                return false;
            }
        }

        private static void EnviarNotificacion(int idConsultorio, string loginUsuario)
        {
            ConsultorioDto consultorio = ABMEmpresa.ObtenerConsultorioPorId(idConsultorio);
            UsuarioDto user = ABMUsuarioCliente.ObtenerDatosClientePaciente(loginUsuario);
            if(consultorio == null || user == null) return; 
            SMTP vSMTP = new SMTP();
            String vMensaje = "El paciente " + user.Nombre+ " quiere pertener a su consultorio." + "\nSaludos,\nOdontoweb";
            vSMTP.Datos_Mensaje(consultorio.Email, vMensaje, "Solicitud de nuevo paciente");
            vSMTP.Enviar_Mail();
        }
        #endregion
    }
}