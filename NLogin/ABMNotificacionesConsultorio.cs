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

        readonly DataContext dataContext = new DataContext();
        readonly ControlLogErrores controlErrores = new ControlLogErrores();

        #endregion

        #region MetodosPublicos

        public NotificacionesConsultorioNewDto GetNotificacionesPendientes(int pIDConsultorio, int pIDTipoNotificacion)
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

        public bool DeshabilitarNuevasNotificaciones(int pIDConsultorio, int pIDTipoNotificacion)
        {
            var query = (from no in dataContext.NotificacionConsultorio
                         where no.IDConsultorio == pIDConsultorio
                         && no.TipoNotificacion == pIDTipoNotificacion
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
                controlErrores.Insertar("NLogin", "ABMNotificacionesConsultorio", "DeshabilitarNuevasNotificaciones", ex);
                return false;
            }
        }
        public bool AceptarSolicitudPaciente(NotificacionesConsultorioDto pNotificacionPaciente)
        {
            var query = (from cp in dataContext.Empresa_Cliente
                         where cp.id_empresa == pNotificacionPaciente.IDConsultorio
                         && cp.id_usuariocliente == pNotificacionPaciente.LoginUsuario
                         select cp).FirstOrDefault();
            if (query == null)
            {
                Empresa_Cliente ep = new Empresa_Cliente();
                ep.id_usuariocliente = pNotificacionPaciente.LoginUsuario;
                ep.id_empresa = pNotificacionPaciente.IDConsultorio;
                dataContext.Empresa_Cliente.InsertOnSubmit(ep);
                var notificacion = (from no in dataContext.NotificacionConsultorio
                                    where no.ID == pNotificacionPaciente.IDNotificacion
                                    select no).FirstOrDefault();
                if (notificacion != null)
                    notificacion.Estado = 0;
            }

            try
            {
                dataContext.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                controlErrores.Insertar("NLogin", "ABMNotificacionesConsultorio", "AceptarSolicitudPaciente", ex);
                return false;
            }
        }
        public bool CancelarSolicitudPaciente(NotificacionesConsultorioDto pNotificacionPaciente)
        {

            var notificacion = (from no in dataContext.NotificacionConsultorio
                                where no.ID == pNotificacionPaciente.IDNotificacion
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
                controlErrores.Insertar("NLogin", "ABMNotificacionesConsultorio", "AceptarSolicitudPaciente", ex);
                return false;
            }
        }
        #endregion
    }
}