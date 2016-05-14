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

        #endregion
    }
}
