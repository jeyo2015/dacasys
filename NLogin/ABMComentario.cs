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

        readonly static DataContext dataContext = new DataContext();

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
                ControlLogErrores.Insertar("NLogin", "ABMComentario", "Insertar", ex);
                return 0;
            }
        }

        public static List<ComentarioDto> ObtenerComentarios(int idConsultorio)
        {
            return (from comentario in dataContext.ComentariosClinica
                    where comentario.IsVisible == true && comentario.IDClinica == idConsultorio
                    orderby comentario.FechaCreacion descending
                    select new ComentarioDto()
                    {
                        IDEmpresa = comentario.IDClinica,
                        LoginCliente = comentario.IDUsuarioPaciente,
                        Comentario = comentario.Comentario,
                        IsVisible = comentario.IsVisible,
                        FechaCreacion = comentario.FechaCreacion
                    }).ToList();
        }

        #endregion
    }
}