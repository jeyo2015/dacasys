using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataTableConverter;
using DLogin;
using System.Data;
using NEventos;
using Herramientas;
namespace NLogin
{
    public class ABMNotificacionesConsultorio
    {
        #region VariablesGlobales
        DLoginLinqDataContext gDc = new DLoginLinqDataContext();
        ControlBitacora gCb = new ControlBitacora();
        ControlLogErrores gCe = new ControlLogErrores();
        #endregion

        #region MetodosPublicos

        public NotificacionesConsultorioNewDto GetNotificacionesPendientes(int pIDConsultorio, int pIDTipoNotificacion)
        {
            var query = (from no in gDc.NotificacionConsultorio
                         from cp in gDc.Cliente_Paciente
                         from p in gDc.Paciente
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
            var query = (from no in gDc.NotificacionConsultorio
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
                gDc.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                gCe.Insertar("NLogin", "ABMNotificacionesConsultorio", "DeshabilitarNuevasNotificaciones", ex);
                return false;
            }
        }
        public bool AceptarSolicitudPaciente(NotificacionesConsultorioDto pNotificacionPaciente)
        {
            var query = (from cp in gDc.Empresa_Cliente
                         where cp.id_empresa == pNotificacionPaciente.IDConsultorio
                         && cp.id_usuariocliente == pNotificacionPaciente.LoginUsuario
                         select cp).FirstOrDefault();
            if (query == null)
            {
                Empresa_Cliente ep = new Empresa_Cliente();
                ep.id_usuariocliente = pNotificacionPaciente.LoginUsuario;
                ep.id_empresa = pNotificacionPaciente.IDConsultorio;
                gDc.Empresa_Cliente.InsertOnSubmit(ep);
                var notificacion = (from no in gDc.NotificacionConsultorio
                                    where no.ID == pNotificacionPaciente.IDNotificacion
                                    select no).FirstOrDefault();
                if (notificacion != null)
                    notificacion.Estado = 0;
            }

            try
            {
                gDc.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                gCe.Insertar("NLogin", "ABMNotificacionesConsultorio", "AceptarSolicitudPaciente", ex);
                return false;
            }
        }
        public bool CancelarSolicitudPaciente(NotificacionesConsultorioDto pNotificacionPaciente)
        {

            var notificacion = (from no in gDc.NotificacionConsultorio
                                where no.ID == pNotificacionPaciente.IDNotificacion
                                select no).FirstOrDefault();
            if (notificacion != null)
                notificacion.Estado = 0;

            try
            {
                gDc.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                gCe.Insertar("NLogin", "ABMNotificacionesConsultorio", "AceptarSolicitudPaciente", ex);
                return false;
            }
        }
        #endregion
    }
}
