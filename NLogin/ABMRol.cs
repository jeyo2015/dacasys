namespace NLogin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataTableConverter;
    using Datos;
    using System.Data;
    using NEventos;
    using Herramientas;

    public class ABMRol
    {
        #region VariablesGlobales

        readonly static ContextoDataContext dataContext = new ContextoDataContext();

        #endregion

        #region Metodos Publicos

        public static bool isModuloChecked(int idModulo, int idRol)
        {
            return (from mr in dataContext.Rol_Modulo
                    where mr.IDModulo == idModulo
                          && mr.IDRol == idRol
                    select mr).Any();
        }

        public static bool isFormularioChecked(int idFormulario, int idRol)
        {
            return (from fr in dataContext.Rol_Formulario
                    where fr.IDFormulario == idFormulario
                          && fr.IDRol == idRol
                    select fr).Any();
        }

        public static bool isComponenteChecked(int idComponente, int idRol)
        {
            return (from cr in dataContext.Rol_Componente
                    where cr.ID_Componente == idComponente
                          && cr.ID_Rol == idRol
                    select cr).Any();
        }

        public static List<ModulosTree> GetModulos(int idRol)
        {
            //var moduloOfRol = (from mr in dataContext.Rol_Modulo
            //                   where mr.IDRol == idRol
            //                   select mr).ToList();
            return (from m in dataContext.Modulo
                    select m).Select(x =>
                        new ModulosTree()
                        {
                            ID = x.ID,
                            IsChecked = isModuloChecked(x.ID, idRol),
                            Nombre = x.Texto,
                            Hijos = getFormularios(idRol, x.ID),
                            IsCollapsed = false
                        }).ToList();
        }

        /// <summary>
        /// InsertarModulo un nuevo Rol
        /// </summary>
        /// <param name="nombre">Nombre del Rol</param>
        /// <param name="idEmpresa">ID de la Empresa</param>
        /// <param name="idUsuario">ID del usuario que ejecuta accion</param>
        /// <param name="idDacasys">ID del empresa</param>
        /// <returns> 1 - Insertado Correctamente
        ///           2 - No se Pudo insertar
        ///           3 - Nombre existe
        ///           </returns>
        public static int Insertar(string nombre, int idEmpresa, string idUsuario, int idDacasys)
        {
            var rol = from r in dataContext.Rol
                      where r.Nombre.ToUpper() == nombre.ToUpper() &&
                      r.IDEmpresa == idEmpresa
                      select r;
            if (rol.Any())
                return 3;
            var vRol = new Rol { IDEmpresa = idEmpresa, Nombre = nombre };
            try
            {
                dataContext.Rol.InsertOnSubmit(vRol);
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se inserto un Rol", idUsuario);
                return Insertar_Rol(vRol, idUsuario, idDacasys);
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMRol", "InsertarModulo", ex);
                return 2;
            }
        }

        /// <summary>
        /// Inserta o elimina un Modulo a un rol
        /// </summary>
        /// <param name="idRol">ID del rol</param>
        /// <param name="idModulo">Id del modulo</param>
        /// <param name="pinsert">True inserta, False elimina</param>
        /// <param name="idUsuario">Id del usuario que realiza accion</param>
        /// <param name="idDacasys">ID del empresa</param>
        public static void Insertar_EliminarModulo(int idRol, int idModulo, bool pinsert, string idUsuario, int idDacasys)
        {
            var sql = from m in dataContext.Rol_Modulo
                      where m.IDRol == idRol && m.IDModulo == idModulo
                      select m;
            if (pinsert)
            {
                if (!sql.Any())
                {
                    var vrol_modulo = new Rol_Modulo { IDModulo = idModulo, IDRol = idRol };
                    try
                    {
                        dataContext.Rol_Modulo.InsertOnSubmit(vrol_modulo);
                        dataContext.SubmitChanges();
                        ControlBitacora.Insertar("Se inserto un nuevo Rol_Modulo", idUsuario);
                    }
                    catch (Exception) { }
                }
            }
            else
            {//elimina
                if (sql.Any())
                {
                    dataContext.Rol_Modulo.DeleteOnSubmit(sql.First());
                    dataContext.SubmitChanges();
                }

            }
            // DataTable vDTFormularios = Get_Formulariosp(idRol, idModulo,idDacasys);
            //   foreach (DataRow formulario in vDTFormularios.Rows)
            //  {
            //      Insertar_EliminarFormulario(idRol, (int)formulario[0], idUsuario, pinsert,idDacasys);
            // }

        }

        /// <summary>
        /// Inserta o Eliminar un formulario de un rol, con sus respectivos componentes
        /// </summary>
        /// <param name="idRol">Id del Rol</param>
        /// <param name="idFormulario">Id del Formulario</param>
        /// <param name="idUsuario">ID del Usuario que realiza accion</param>
        /// <param name="pinsert">True insertar, False Elimina</param>
        /// <param name="idDacasys">ID del empresa</param>
        public static void Insertar_EliminarFormulario(int idRol, int idFormulario, string idUsuario, bool pinsert, int idDacasys)
        {
            var sql = from f in dataContext.Rol_Formulario
                      where f.IDRol == idRol && f.IDFormulario == idFormulario
                      select f;
            if (pinsert)
            {
                if (!sql.Any())
                {
                    Rol_Formulario vrol_formulario = new Rol_Formulario();
                    vrol_formulario.IDFormulario = idFormulario;
                    vrol_formulario.IDRol = idRol;
                    try
                    {
                        dataContext.Rol_Formulario.InsertOnSubmit(vrol_formulario);
                        dataContext.SubmitChanges();
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                if (sql.Any())
                {
                    dataContext.Rol_Formulario.DeleteOnSubmit(sql.First());
                    dataContext.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Inserta o elimina un Componente de un rol
        /// </summary>
        /// <param name="idRol">ID del Rol </param>
        /// <param name="idComponente">Id del componente a elimnar </param>
        /// <param name="idUsuario">Id del usuario que realiza accion</param>
        /// <param name="pinsert">True insertar, False eliminar</param>
        public static void Insertar_EliminarComponente(int idRol, int idComponente, string idUsuario, bool pinsert)
        {
            var sql = from c in dataContext.Rol_Componente
                      where c.ID_Rol == idRol && c.ID_Componente == idComponente
                      select c;
            if (pinsert)
            {
                if (!sql.Any())
                {
                    Rol_Componente vrol_componente = new Rol_Componente();
                    vrol_componente.ID_Componente = idComponente;
                    vrol_componente.ID_Rol = idRol;
                    try
                    {
                        dataContext.Rol_Componente.InsertOnSubmit(vrol_componente);
                        dataContext.SubmitChanges();
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                if (sql.Any())
                {
                    dataContext.Rol_Componente.DeleteOnSubmit(sql.First());
                    dataContext.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Eliminar Rol
        /// </summary>
        /// <param name="idRol">ID Rol a Eliminar</param>
        /// <param name="idUsuario">ID del Usuario que realiza accion</param>
        /// <returns>1 - Eliminado Correctamente
        ///           2 - No se Pudo Eliminar
        ///           3 - No existe el Rol
        ///           4 - Existen usuarios en este rol</returns>
        public static int Eliminar(int idRol, string idUsuario)
        {
            var sql = from r in dataContext.Rol
                      where r.ID == idRol
                      select r;

            if (sql.Any())
            {
                var vUs = from us in dataContext.UsuarioEmpleado
                          where us.IDRol == idRol
                          select us;
                if (vUs.Any())
                {
                    return 4;
                }

                try
                {
                    dataContext.Rol.DeleteOnSubmit(sql.First());
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se elimino Rol", idUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NLogin", "ABMRol", "Eliminar", ex);
                    return 2;
                }
            }
            return 3;
        }

        /// <summary>
        /// Metodo privado que devuelve los roles de un consultorio
        /// </summary>
        /// <param name="idEmpresa">Id del consultorio</param>
        /// <returns></returns>
        public static List<RolDto> Get_Roles(int idEmpresa)
        {
            return (from r in dataContext.Rol
                    where r.IDEmpresa == idEmpresa
                    orderby r.Nombre
                    select new RolDto()
                    {
                        ID = r.ID,
                        IDEmpresa = r.IDEmpresa,
                        Nombre = r.Nombre
                    }).ToList();
        }

        public static List<ModulosTree> getComponenentes(int idRol, int idFormulario)
        {
            var componenteOfRol = (from cr in dataContext.Rol_Componente
                                   where cr.ID_Rol == idRol
                                   select cr).ToList();
            return (from c in dataContext.Componente
                    where c.IDFormulario == idFormulario
                    select c).Select(x =>
                    new ModulosTree()
                    {
                        ID = x.ID,
                        IsChecked = isComponenteChecked(x.ID, idRol),
                        Nombre = x.Texto,
                        Hijos = new List<ModulosTree>(),
                        IsCollapsed = false
                    }).ToList();

        }

        public static List<ModulosTree> getFormularios(int idRol, int idModulo)
        {
            var formulariosOfRol = (from fr in dataContext.Rol_Formulario
                                    where fr.IDRol == idRol
                                    select fr).ToList();

            return (from f in dataContext.Fomulario
                    where f.IDModulo == idModulo
                    select f).Select(x =>
                            new ModulosTree()
                            {
                                ID = x.ID,
                                Nombre = x.Texto,
                                IsChecked = isFormularioChecked(x.ID, idRol),
                                Hijos = getComponenentes(idRol, x.ID),
                                IsCollapsed = false
                            }).ToList();
        }

        public static bool ModificarPermisos(List<ModulosTree> modulos, int idRol)
        {
            //  var isDone = true;
            foreach (var modulo in modulos)
            {
                if (modulo.IsChecked)
                    InsertarModulo(modulo, idRol);
                else
                    DeleteModulo(modulo, idRol);
                if (modulo.Hijos != null)
                    foreach (var formulario in modulo.Hijos)
                    {
                        if (formulario.IsChecked)
                            Insertarformulario(formulario, idRol);
                        else
                            Deleteformulario(formulario, idRol);
                        if (formulario.Hijos != null)
                            foreach (var componente in formulario.Hijos)
                            {
                                if (componente.IsChecked)
                                    InsertarComponente(componente, idRol);
                                else
                                    DeleteComponente(componente, idRol);
                            }
                    }
            }
            try
            {
                dataContext.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMRol", "ModificarPermisos", ex);
                return false;
            }
        }

        #endregion

        #region Metodos Privados

        /// <summary>
        /// Este metodo los permisos por defecto de un nuevo rol
        /// </summary>
        /// <param name="rolDto">Rol nuevo</param>
        /// <param name="idUsuario">ID del usuario que crea</param>
        /// <param name="idDacasys">Si es dacasys 1, 0 lo contrario</param>
        /// <returns>-1 algo anda mal</returns>
        private static int Insertar_Rol(Rol rolDto, string idUsuario, int idDacasys)
        {
            var vRol = from rol in dataContext.Rol
                       where rol.IDEmpresa == rolDto.IDEmpresa && rol.Nombre == rolDto.Nombre
                       select rol;

            if (vRol.Any())
            {
                int vIDRol = vRol.First().ID;
                Insertar_Modulos(vIDRol, idUsuario, idDacasys);
                Insertar_Formularios(vIDRol, idUsuario, idDacasys);
                Insertar_Componentes(vIDRol, idUsuario, idDacasys);
                return vRol.First().ID;
            }
            return -1;
        }

        /// <summary>
        /// Inserta los componentes a un nuevo rol
        /// </summary>
        /// <param name="idRol">ID rol nuevo</param>
        /// <param name="idUsuario">ID del usuario</param>
        /// <param name="idDacasys">Si es dacasys 1, 0 lo contrario</param>
        private static void Insertar_Componentes(int idRol, string idUsuario, int idDacasys)
        {
            bool vDacasys = Convert.ToBoolean(idDacasys);
            var vComponentes = from componentes in dataContext.Componente
                               where componentes.dacasys == vDacasys
                               select componentes;
            if (vDacasys)

                vComponentes = from componentes in dataContext.Componente
                               select componentes;

            foreach (var componente in vComponentes)
                Insertar_EliminarComponente(idRol, componente.ID, idUsuario, true);
        }

        /// <summary>
        /// Inserta los formularios a un nuevo Rol
        /// </summary>
        /// <param name="idRol">ID rol</param>
        /// <param name="idUsuario">ID usuario</param>
        /// <param name="idDacasys">Si es dacasys 1, 0 lo contrario</param>
        private static void Insertar_Formularios(int idRol, string idUsuario, int idDacasys)
        {
            bool vDacasys = Convert.ToBoolean(idDacasys);
            var vFormularios = from formularios in dataContext.Fomulario
                               where formularios.dacasys == vDacasys
                               select formularios;
            if (vDacasys)

                vFormularios = from formularios in dataContext.Fomulario
                               select formularios;

            foreach (var formulario in vFormularios)
                Insertar_EliminarFormulario(idRol, formulario.ID, idUsuario, true, idDacasys);
        }

        /// <summary>
        /// Inserta los modulos a un nuevo Rol
        /// </summary>
        /// <param name="idRol">ID rol nuevo</param>
        /// <param name="idUsuario">ID del usuario</param>
        /// <param name="idDacasys">Si es dacasys 1, 0 lo contrario</param>
        private static void Insertar_Modulos(int idRol, string idUsuario, int idDacasys)
        {
            bool vDacasys = Convert.ToBoolean(idDacasys);
            var vModulos = from modulos in dataContext.Modulo
                           where modulos.dacasys == vDacasys
                           select modulos;
            if (vDacasys)

                vModulos = from modulos in dataContext.Modulo
                           select modulos;

            foreach (var modulo in vModulos)
                Insertar_EliminarModulo(idRol, modulo.ID, true, idUsuario, idDacasys);

        }

        private static void DeleteComponente(ModulosTree componente, int idRol)
        {
            var rc = (from cr in dataContext.Rol_Componente
                      where cr.ID_Componente == componente.ID
                          && cr.ID_Rol == idRol
                      select cr).FirstOrDefault();
            if (rc != null)
            {
                dataContext.Rol_Componente.DeleteOnSubmit(rc);
            }
        }

        private static void InsertarComponente(ModulosTree componente, int idRol)
        {
            var rc = (from cr in dataContext.Rol_Componente
                      where cr.ID_Componente == componente.ID
                          && cr.ID_Rol == idRol
                      select cr).FirstOrDefault();
            if (rc == null)
            {
                Rol_Componente rfor = new Rol_Componente();
                rfor.ID_Rol = idRol;
                rfor.ID_Componente = componente.ID;
                dataContext.Rol_Componente.InsertOnSubmit(rfor);
            }
        }

        private static void Deleteformulario(ModulosTree formulario, int idRol)
        {
            var rf = (from fr in dataContext.Rol_Formulario
                      where fr.IDFormulario == formulario.ID
                      && fr.IDRol == idRol
                      select fr).FirstOrDefault();
            if (rf != null)
            {
                dataContext.Rol_Formulario.DeleteOnSubmit(rf);
            }
        }

        private static void Insertarformulario(ModulosTree formulario, int idRol)
        {
            var rf = (from fr in dataContext.Rol_Formulario
                      where fr.IDFormulario == formulario.ID
                      && fr.IDRol == idRol
                      select fr).FirstOrDefault();
            if (rf == null)
            {
                Rol_Formulario rfor = new Rol_Formulario();
                rfor.IDRol = idRol;
                rfor.IDFormulario = formulario.ID;
                dataContext.Rol_Formulario.InsertOnSubmit(rfor);
            }
        }

        private static void DeleteModulo(ModulosTree modulo, int idRol)
        {
            var rm = (from mr in dataContext.Rol_Modulo
                      where mr.IDRol == idRol &&
                      mr.IDModulo == modulo.ID
                      select mr).FirstOrDefault();
            if (rm != null)
            {

                dataContext.Rol_Modulo.DeleteOnSubmit(rm);
            }
        }

        private static void InsertarModulo(ModulosTree modulo, int idRol)
        {
            var query = (from mr in dataContext.Rol_Modulo
                         where mr.IDRol == idRol &&
                         mr.IDModulo == modulo.ID
                         select mr).FirstOrDefault();
            if (query == null)
            {
                Rol_Modulo mr = new Rol_Modulo();
                mr.IDModulo = modulo.ID;
                mr.IDRol = idRol;
                dataContext.Rol_Modulo.InsertOnSubmit(mr);
            }

        }

        #endregion
    }
}