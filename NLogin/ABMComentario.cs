namespace NLogin
{
    using System;
    using System.Linq;
    using NEventos;
    using Herramientas;
    using Datos;
    using System.Collections.Generic;

    public class ABMComentario
    {
        #region VariablesGlogales

        readonly static ContextoDataContext dataContext = new ContextoDataContext();

        #endregion

        #region Metodos Publicos

        public static int Eliminar(ComentarioDto comentarioDto, string idUsuario) {

            var comen = (from c in dataContext.ComentariosClinica
                        where c.ID == comentarioDto.IDComentario
                        select c).FirstOrDefault();
            if (comen == null) return 0;
            try {
                dataContext.ComentariosClinica.DeleteOnSubmit(comen);
                dataContext.SubmitChanges();
                return 1;
            }catch(Exception ex){

                ControlLogErrores.Insertar("NLogin", "ABMComentario", "Eliminar Comentario", ex);
                return 0;
            }

            return 0;
        }

        public static int Insertar(ComentarioDto comentarioDto, string idUsuario)
        {
            var comentario = new ComentariosClinica
            {
                Comentario = comentarioDto.Comentario,
                FechaCreacion = System.DateTime.Now,
                IDClinica = comentarioDto.IDEmpresa,
                IDUsuarioPaciente = comentarioDto.LoginCliente,
                IsVisible = comentarioDto.IsVisible
            };
            try
            {
                dataContext.ComentariosClinica.InsertOnSubmit(comentario);
                dataContext.SubmitChanges();
                EnviarNotificacionComentarioNuevo(comentarioDto, idUsuario);
                ControlBitacora.Insertar("Se Inserto un nuevo comentario", idUsuario);
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMComentario", "Insertar Comentario", ex);
                return 0;
            }
        }

        private static void EnviarNotificacionComentarioNuevo(ComentarioDto comentarioDto, string idUsuario)
        {
            ConsultorioDto consultorio = ABMEmpresa.ObtenerConsultorioPorId(comentarioDto.IDEmpresa);
            PacienteDto user = ABMUsuarioCliente.ObtenerDatoPaciente(idUsuario);
            if (consultorio == null || user == null) return;
            SMTP vSMTP = new SMTP();
            String vMensaje = "Estimado Dr." + " Tiene un nuevo comentario del paciente "+ user.NombrePaciente+":\n"+ "\""+ comentarioDto.Comentario + "\""+ "\nSaludos,\nOdontoweb";
            vSMTP.Datos_Mensaje(consultorio.Email, vMensaje, "Nuevo comentario");
            vSMTP.Enviar_Mail();
        }

        public static List<ComentarioDto> ObtenerComentarios(int idConsultorio)
        {
            var comentariosCliente = (from comentario in dataContext.ComentariosClinica
                                      from cp in dataContext.Cliente_Paciente
                                      from p in dataContext.Paciente
                                      where comentario.IsVisible == true && comentario.IDClinica == idConsultorio
                                      && cp.id_usuariocliente == comentario.IDUsuarioPaciente
                                      && p.id_paciente == cp.id_paciente && cp.IsPrincipal == true
                                      //orderby comentario.FechaCreacion descending
                                      select new ComentarioDto()
                                      {
                                          IDEmpresa = comentario.IDClinica,
                                          LoginCliente = comentario.IDUsuarioPaciente,
                                          Comentario = comentario.Comentario,
                                          IsVisible = comentario.IsVisible,
                                          FechaCreacion = comentario.FechaCreacion,
                                          NombrePaciente = p.nombre + " " + p.apellido,
                                          IDComentario = comentario.ID
                                      }).ToList();
            var comentariosDoctor = (from comentario in dataContext.ComentariosClinica
                                     from cp in dataContext.UsuarioEmpleado
                                     where comentario.IsVisible == true && comentario.IDClinica == idConsultorio
                                     && comentario.IDUsuarioPaciente == cp.Login
                                     && cp.IDEmpresa == idConsultorio
                                    // orderby comentario.FechaCreacion descending
                                     select new ComentarioDto()
                                     {
                                         IDEmpresa = comentario.IDClinica,
                                         LoginCliente = comentario.IDUsuarioPaciente,
                                         Comentario = comentario.Comentario,
                                         IsVisible = comentario.IsVisible,
                                         FechaCreacion = comentario.FechaCreacion,
                                         NombrePaciente = cp.Nombre,
                                         IDComentario=comentario.ID
                                     }).ToList();
            comentariosDoctor.AddRange(comentariosCliente);
            return comentariosDoctor.OrderByDescending(o=> o.FechaCreacion).ToList();
        }

        #endregion
    }
}