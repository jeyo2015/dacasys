namespace NLogin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Datos;
    using Herramientas;

    public class Seguridad
    {
        #region VariablesGlobales

        readonly static ContextoDataContext dataContext = new ContextoDataContext();

        #endregion

        public static SessionPermisosDto ObtenerPermisos(int idRol)
        {
            try
            {
                var listModulo = from modulo in dataContext.Modulo
                                 join rolModulo in dataContext.Rol_Modulo on modulo.ID equals rolModulo.IDModulo into rolModulos
                                 from permiso in rolModulos.Where(x => x.IDRol == idRol).DefaultIfEmpty()
                                 select new ModuloDto()
                                 {
                                     IdModulo = modulo.ID,
                                     NombreModulo = modulo.Nombre,
                                     TienePermiso = permiso != null
                                 };

                var listComponente = from componente in dataContext.Componente
                                     join rolComponente in dataContext.Rol_Componente on componente.ID equals rolComponente.ID_Componente into rolComponentes
                                     from permiso in rolComponentes.Where(x => x.ID_Rol == idRol).DefaultIfEmpty()
                                     select new ComponenteDto()
                                     {
                                         IdComponente = componente.ID,
                                         NombreComponente = componente.Nombre,
                                         TienePermiso = permiso != null
                                     };

                var listFormulario = from fomulario in dataContext.Fomulario
                                     join rolFormulario in dataContext.Rol_Formulario on fomulario.ID equals rolFormulario.IDFormulario into rolFormularios
                                     from permiso in rolFormularios.Where(x => x.IDRol == idRol).DefaultIfEmpty()
                                     select new FormularioDto()
                                     {
                                         IdFormulario = fomulario.ID,
                                         NombreFormulario = fomulario.Nombre,
                                         TienePermiso = permiso != null
                                     };

                return new SessionPermisosDto()
                {
                    Modulos = listModulo.ToList(),
                    Componentes = listComponente.ToList(),
                    Formularios = listFormulario.ToList()
                };
            }
            catch (Exception)
            {
                return new SessionPermisosDto()
                {
                    Modulos = new List<ModuloDto>(),
                    Componentes = new List<ComponenteDto>(),
                    Formularios = new List<FormularioDto>()
                };
            }
        }
    }
}