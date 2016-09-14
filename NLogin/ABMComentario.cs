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
                ControlBitacora.Insertar("Se Inserto un nuevo comentario", idUsuario);
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMComentario", "InsertarModulo", ex);
                return 0;
            }
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
                                          NombrePaciente = p.nombre + " " + p.apellido
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
                                         NombrePaciente = cp.Nombre
                                     }).ToList();
            comentariosDoctor.AddRange(comentariosCliente);
            return comentariosDoctor.OrderByDescending(o=> o.FechaCreacion).ToList();
        }

        #endregion
    }
}