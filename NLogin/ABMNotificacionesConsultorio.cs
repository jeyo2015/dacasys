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
                EnviarCorreoAceptacion(notificacionesConsultorioDto.LoginUsuario, notificacionesConsultorioDto.IDConsultorio);
                return true;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMNotificacionesConsultorio", "AceptarSolicitudPaciente", ex);
                return false;
            }
        }

        private static void EnviarCorreoAceptacion(string pLogin, int pIdConsultorio)
        {
            var paciente = (from p in dataContext.Paciente
                from pc in dataContext.Cliente_Paciente
                where pc.id_usuariocliente == pLogin
                      && pc.IsPrincipal == true
                      && p.id_paciente == pc.id_paciente
                select p).FirstOrDefault();
            var consultorio = (from c in dataContext.Empresa
                from cc in dataContext.Clinica
                where c.ID == pIdConsultorio
                      && cc.ID == c.IDClinica
                select new ConsultorioDto()
                {
                    NombreClinica = cc.Nombre,
                    Login = c.Login
                }).FirstOrDefault();
            if (paciente != null && consultorio != null)
            {
                var mensajeConfirmacion = "Su solicitud al consultorio " + consultorio.Login + " de la clinica " +
                                          consultorio.NombreClinica + " ha sido aceptada. "+
                                           "\nSaludos,\nOdontoweb";
                var vSMTP = new SMTP();
                vSMTP.Datos_Mensaje(paciente.email, mensajeConfirmacion, "Solicitud aceptada -  ODONTOWEB");
                vSMTP.Enviar_Mail();
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
                return true;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMNotificacionesConsultorio", "AceptarSolicitudPaciente", ex);
                return false;
            }
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
                ControlBitacora.Insertar("Se envio una notificacion", notificacionesConsultorioDto.LoginUsuario);
                return true;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMNotificaciones", "EnviarSolicitudConsultorio", ex);
                return false;
            }
        }
        #endregion
    }
}