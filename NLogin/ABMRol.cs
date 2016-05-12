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
    public class ABMRol
    {
        #region VariablesGlobales
        DLoginLinqDataContext gDc = new DLoginLinqDataContext();
        ControlBitacora gCb = new ControlBitacora();
        ControlLogErrores gCe = new ControlLogErrores();
        #endregion
        #region ABM_Rol

        /// <summary>
        /// Insertar un nuevo Rol
        /// </summary>
        /// <param name="pNombre">Nombre del Rol</param>
        /// <param name="pIDEmpresa">ID de la Empresa</param>
        /// <param name="pIDUsuario">ID del usuario que ejecuta accion</param>
        /// <returns> 1 - Insertado Correctamente
        ///           2 - No se Pudo insertar
        ///           3 - Nombre existe
        ///           </returns>
        public int Insertar(String pNombre, int pIDEmpresa, string pIDUsuario, int pDACASYS)
        {

            var rol = from r in gDc.Rol
                      where r.Nombre.ToUpper() == pNombre.ToUpper() &&
                      r.IDEmpresa == pIDEmpresa
                      select r;
            if (rol.Count() > 0)
                return 3;
            Rol vRol = new Rol();
            vRol.IDEmpresa = pIDEmpresa;
            vRol.Nombre = pNombre;
            try
            {
                gDc.Rol.InsertOnSubmit(vRol);
                gDc.SubmitChanges();
                gCb.Insertar("Se inserto un Rol", pIDUsuario);
                return Insertar_Rol(vRol, pIDUsuario, pDACASYS);

            }
            catch (Exception ex)
            {
                gCe.Insertar("NLogin", "ABMRol", "Insertar", ex);
                return 2;
            }
        }

        /// <summary>
        /// Este metodo los permisos por defecto de un nuevo rol
        /// </summary>
        /// <param name="pRol">Rol nuevo</param>
        /// <param name="pIDUsuario">ID del usuario que crea</param>
        /// <param name="pDACASYS">Si es dacasys 1, 0 lo contrario</param>
        /// <returns>-1 algo anda mal</returns>
        private int Insertar_Rol(Rol pRol, string pIDUsuario, int pDACASYS)
        {
            var vRol = from rol in gDc.Rol
                       where rol.IDEmpresa == pRol.IDEmpresa && rol.Nombre == pRol.Nombre
                       select rol;

            if (vRol.Count() > 0)
            {
                int vIDRol = vRol.First().ID;
                Insertar_Modulos(vIDRol, pIDUsuario, pDACASYS);
                Insertar_Formularios(vIDRol, pIDUsuario, pDACASYS);
                Insertar_Componentes(vIDRol, pIDUsuario, pDACASYS);
                return vRol.First().ID;
            }
            return -1;
        }
        /// <summary>
        /// Inserta los componentes a un nuevo rol
        /// </summary>
        /// <param name="pIDRol">ID rol nuevo</param>
        /// <param name="pIDUsuario">ID del usuario</param>
        /// <param name="pDACASYS">Si es dacasys 1, 0 lo contrario</param>
        private void Insertar_Componentes(int pIDRol, string pIDUsuario, int pDACASYS)
        {
            bool vDacasys = Convert.ToBoolean(pDACASYS);
            var vComponentes = from componentes in gDc.Componente
                               where componentes.dacasys == vDacasys
                               select componentes;
            if (vDacasys)

                vComponentes = from componentes in gDc.Componente
                               select componentes;

            foreach (var componente in vComponentes)
                Insertar_EliminarComponente(pIDRol, componente.ID, pIDUsuario, true);
        }
        /// <summary>
        /// Inserta los formularios a un nuevo Rol
        /// </summary>
        /// <param name="pIDRol">ID rol</param>
        /// <param name="pIDUsuario">ID usuario</param>
        /// <param name="pDACASYS">Si es dacasys 1, 0 lo contrario</param>
        private void Insertar_Formularios(int pIDRol, string pIDUsuario, int pDACASYS)
        {
            bool vDacasys = Convert.ToBoolean(pDACASYS);
            var vFormularios = from formularios in gDc.Fomulario
                               where formularios.dacasys == vDacasys
                               select formularios;
            if (vDacasys)

                vFormularios = from formularios in gDc.Fomulario
                               select formularios;

            foreach (var formulario in vFormularios)
                Insertar_EliminarFormulario(pIDRol, formulario.ID, pIDUsuario, true, pDACASYS);
        }
        /// <summary>
        /// Inserta los modulos a un nuevo Rol
        /// </summary>
        /// <param name="pIDRol">ID rol nuevo</param>
        /// <param name="pIDUsuario">ID del usuario</param>
        /// <param name="pDACASYS">Si es dacasys 1, 0 lo contrario</param>
        private void Insertar_Modulos(int pIDRol, string pIDUsuario, int pDACASYS)
        {
            bool vDacasys = Convert.ToBoolean(pDACASYS);
            var vModulos = from modulos in gDc.Modulo
                           where modulos.dacasys == vDacasys
                           select modulos;
            if (vDacasys)

                vModulos = from modulos in gDc.Modulo
                           select modulos;

            foreach (var modulo in vModulos)
                Insertar_EliminarModulo(pIDRol, modulo.ID, true, pIDUsuario, pDACASYS);

        }

        /// <summary>
        /// Inserta o elimina un Modulo a un rol
        /// </summary>
        /// <param name="pIDRol">ID del rol</param>
        /// <param name="pIDModulo">Id del modulo</param>
        /// <param name="pinsert">True inserta, False elimina</param>
        /// <param name="pIDUsuario">Id del usuario que realiza accion</param>
        public void Insertar_EliminarModulo(int pIDRol, int pIDModulo, bool pinsert, string pIDUsuario,
                                              int pDACASYS)
        {
            var sql = from m in gDc.Rol_Modulo
                      where m.IDRol == pIDRol && m.IDModulo == pIDModulo
                      select m;
            if (pinsert)
            {
                if (sql.Count() == 0)
                {
                    Rol_Modulo vrol_modulo = new Rol_Modulo();
                    vrol_modulo.IDModulo = pIDModulo;
                    vrol_modulo.IDRol = pIDRol;
                    try
                    {
                        gDc.Rol_Modulo.InsertOnSubmit(vrol_modulo);
                        gDc.SubmitChanges();
                        gCb.Insertar("Se inserto un nuevo Rol_Modulo", pIDUsuario);
                    }
                    catch (Exception ex)
                    {
                        ///nada
                    }
                }

            }
            else
            {//elimina
                if (sql.Count() > 0)
                {
                    gDc.Rol_Modulo.DeleteOnSubmit(sql.First());
                    gDc.SubmitChanges();
                }

            }
            // DataTable vDTFormularios = Get_Formulariosp(pIDRol, pIDModulo,pDACASYS);
            //   foreach (DataRow formulario in vDTFormularios.Rows)
            //  {
            //      Insertar_EliminarFormulario(pIDRol, (int)formulario[0], pIDUsuario, pinsert,pDACASYS);
            // }

        }

        /// <summary>
        /// Inserta o Eliminar un formulario de un rol, con sus respectivos componentes
        /// </summary>
        /// <param name="pIDRol">Id del Rol</param>
        /// <param name="pIDFormulario">Id del Formulario</param>
        /// <param name="pIDUsuario">ID del Usuario que realiza accion</param>
        /// <param name="pinsert">True insertar, False Elimina</param>
        public void Insertar_EliminarFormulario(int pIDRol, int pIDFormulario, string pIDUsuario, bool pinsert, int pDACASYS)
        {
            var sql = from f in gDc.Rol_Formulario
                      where f.IDRol == pIDRol && f.IDFormulario == pIDFormulario
                      select f;
            if (pinsert)
            {
                if (sql.Count() == 0)
                {
                    Rol_Formulario vrol_formulario = new Rol_Formulario();
                    vrol_formulario.IDFormulario = pIDFormulario;
                    vrol_formulario.IDRol = pIDRol;
                    try
                    {
                        gDc.Rol_Formulario.InsertOnSubmit(vrol_formulario);
                        gDc.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        /// esta insertado
                    }
                }

            }
            else
            {

                if (sql.Count() > 0)
                {
                    gDc.Rol_Formulario.DeleteOnSubmit(sql.First());
                    gDc.SubmitChanges();
                }
            }

        }


        /// <summary>
        /// Inserta o elimina un Componente de un rol
        /// </summary>
        /// <param name="pIDRol">ID del Rol </param>
        /// <param name="pIDComponente">Id del componente a elimnar </param>
        /// <param name="pIDUsuario">Id del usuario que realiza accion</param>
        /// <param name="pinsert">True insertar, False eliminar</param>
        public void Insertar_EliminarComponente(int pIDRol, int pIDComponente, string pIDUsuario, bool pinsert)
        {
            var sql = from c in gDc.Rol_Componente
                      where c.ID_Rol == pIDRol && c.ID_Componente == pIDComponente
                      select c;
            if (pinsert)
            {
                if (sql.Count() == 0)
                {
                    Rol_Componente vrol_componente = new Rol_Componente();
                    vrol_componente.ID_Componente = pIDComponente;
                    vrol_componente.ID_Rol = pIDRol;
                    try
                    {
                        gDc.Rol_Componente.InsertOnSubmit(vrol_componente);
                        gDc.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        ///ya esta insertado
                    }


                }
            }
            else
            {

                if (sql.Count() > 0)
                {
                    gDc.Rol_Componente.DeleteOnSubmit(sql.First());
                    gDc.SubmitChanges();
                }


            }
        }



        /// <summary>
        /// Modificar un nuevo Rol
        /// </summary>
        /// <param name="pIDRol">ID del Rol a Modificar</param>
        /// <param name="pNombre">Nombre del Rol</param>
        /// <param name="pIDEmpresa">ID de la Empresa</param>
        /// <param name="pIDUsuario">ID del usuario que ejecuta accion</param>
        /// <returns> 1 - Insertado Correctamente
        ///           2 - No se Pudo insertar
        ///           3 - No existe el Rol</returns>
        public int Modificar(int pIDRol, String pNombre, int pIDEmpresa, string pIDUsuario)
        {
            var sql = from r in gDc.Rol
                      where r.ID == pIDRol
                      select r;

            if (sql.Count() > 0)
            {
                sql.First().Nombre = pNombre;
                sql.First().IDEmpresa = pIDEmpresa;

                try
                {
                    gDc.SubmitChanges();
                    gCb.Insertar("Se modifico un rol", pIDUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    gCe.Insertar("NLogin", "ABMRol", "Modificar", ex);
                    return 2;
                }

            }
            gCe.Insertar("NLogin", "ABMRol", "Modificar, no existe rol", null);
            return 3;
        }


        /// <summary>
        /// Eliminar Rol
        /// </summary>
        /// <param name="pIDRol">ID Rol a Eliminar</param>
        /// <param name="pIDUsuario">ID del Usuario que realiza accion</param>
        /// <returns>1 - Eliminado Correctamente
        ///           2 - No se Pudo Eliminar
        ///           3 - No existe el Rol
        ///           4 - Existen usuarios en este rol</returns>
        public int Eliminar(int pIDRol, string pIDUsuario)
        {

            var sql = from r in gDc.Rol
                      where r.ID == pIDRol
                      select r;

            if (sql.Count() > 0)
            {
                var vUs = from us in gDc.UsuarioEmpleado
                          where us.IDRol == pIDRol
                          select us;
                if (vUs.Count() > 0)
                {


                    return 4;
                }

                try
                {
                    gDc.Rol.DeleteOnSubmit(sql.First());
                    gDc.SubmitChanges();
                    gCb.Insertar("Se elimino Rol", pIDUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    gCe.Insertar("NLogin", "ABMRol", "Eliminar", ex);
                    return 2;
                }
            }
            return 3;
        }

        #endregion

        #region Getter_Rol
        /// <summary>
        /// Metodo privado que devuelve los roles de un consultorio
        /// </summary>
        /// <param name="pIDempresa">Id del consultorio</param>
        /// <returns></returns>
        public List<Rol> Get_Roles(int pIDempresa)
        {

            return (from r in gDc.Rol
                    where r.IDEmpresa == pIDempresa
                    orderby r.Nombre
                    select r).ToList();

        }

        /// <summary>
        /// Obtiene todos los Roles de una empresa
        /// </summary>
        /// <param name="pIDEmpresa">ID de la Empresa</param>
        /// <returns> DataTable con los roles de la empresa indicada</returns>
        public DataTable Get_rolesp(int pIDEmpresa)
        {
            return Converter<Rol>.Convert(Get_Roles(pIDEmpresa).ToList());
        }

        private IEnumerable<Rol> Get_Rolesn(int pIDempresa, String pNombreRol)
        {

            return from r in gDc.Rol
                   where r.IDEmpresa == pIDempresa
                            && r.Nombre.Contains(pNombreRol)
                   orderby r.Nombre
                   select r;

        }

        /// <summary>
        /// Obtiene todos los Roles de una empresa por nombre
        /// </summary>
        /// <param name="pIDEmpresa">ID de la Empresa</param>
        /// <param name="pNombreRol">Nombre del Rol a buscar</param>
        /// <returns> DataTable con los roles que coincidan con el nombre</returns>
        public DataTable Get_rolesnp(int pIDEmpresa, String pNombreRol)
        {
            return Converter<Rol>.Convert(Get_Rolesn(pIDEmpresa, pNombreRol).ToList());
        }

        /// <summary>
        /// Metodo privad que devuelve un rol
        /// </summary>
        /// <param name="pIDRol">ID del rol</param>
        /// <returns></returns>
        private IEnumerable<Rol> Get_Rol(int pIDRol)
        {

            return from r in gDc.Rol
                   where r.ID == pIDRol
                   select r;
        }

        /// <summary>
        /// Obtiene un Rol
        /// </summary>
        /// <param name="pIDRol">ID del Rol</param>
        /// <returns> DataTable con el rol que desea encontrar</returns>
        public DataTable Get_rolp(int pIDRol)
        {
            return Converter<Rol>.Convert(Get_Rol(pIDRol).ToList());
        }

        /// <summary>
        /// Metodo privado que devuelve los modulos de un rol
        /// </summary>
        /// <param name="pIDRol">ID rol</param>
        /// <param name="pDACASYS">1 es dacasys, 0 no</param>
        /// <returns></returns>
        private IEnumerable<getModulosResult> Get_Modulos(int pIDRol, int pDACASYS)
        {

            return gDc.getModulos(pIDRol, pDACASYS);
        }

        /// <summary>
        /// Obtiene los modulos de un rol
        /// </summary>
        /// <param name="pIDRol">ID del Rol</param>
        /// <returns> DataTable con los modulos del Rol</returns>
        public DataTable Get_Modulosp(int pIDRol, int pDACASYS)
        {

            return Converter<getModulosResult>.Convert(Get_Modulos(pIDRol, pDACASYS).ToList());
        }

        private IEnumerable<getFormulariosResult> Get_Formularios(int pIDRol, int pIDModulo, int pDACASYS)
        {
            return gDc.getFormularios(pIDModulo, pIDRol, pDACASYS);

        }

        /// <summary>
        /// Obtiene los Formularios  de un Modlo
        /// </summary>
        /// <param name="pIDRol">ID del Rol</param>
        /// <param name="pIDModulo">ID del Modulo</param>
        /// <returns> DataTable con los formularios de un modulo indicando si pertenece al Rol</returns>
        public DataTable Get_Formulariosp(int pIDRol, int pIDModulo, int pDACASYS)
        {
            return Converter<getFormulariosResult>.Convert(Get_Formularios(pIDRol, pIDModulo, pDACASYS).ToList());
        }


        private IEnumerable<getComponentesResult> Get_Componentes(int pIDRol, int pIDFormulario, int pDACASYS)
        {
            return gDc.getComponentes(pIDFormulario, pIDRol, pDACASYS);

        }

        /// <summary>
        /// Obtiene los Componentes  de un Formulario
        /// </summary>
        /// <param name="pIDRol">ID del Rol</param>
        /// <param name="pIDFormulario">ID del Formulario</param>
        /// <returns> DataTable con los componetes de un formulario indicando si pertenece al Rol</returns>
        public DataTable Get_Componentesp(int pIDRol, int pIDFormulario, int pDACASYS)
        {
            return Converter<getComponentesResult>.Convert(Get_Componentes(pIDRol, pIDFormulario, pDACASYS).ToList());
        }
        #endregion
        #region Metodos_auxiliares
        /// <summary>
        /// Desabilita las funciones de una empresa, por vencimiento de licencia
        /// </summary>
        /// <param name="pIDEmpresa">ID de la Empresa</param>
        public void Desahabilitar_Por_Licencia(int pIDEmpresa)
        {
            var vRoles = from r in gDc.Rol
                         where r.IDEmpresa == pIDEmpresa
                         select r;
            int vIDVerCi = this.Get_CodigoFormulario("frmCitas");
            foreach (Rol r in vRoles)
            {
                var vformularios = from f in gDc.Rol_Formulario
                                   where f.IDRol == r.ID
                                   select f;

                foreach (Rol_Formulario rf in vformularios)
                {
                    if (rf.IDFormulario != vIDVerCi)
                    {
                        Insertar_EliminarFormulario(rf.IDRol, rf.IDFormulario, "DACASYS", false, 0);
                    }
                }
            }

        }
        /// <summary>
        /// Devuelve ID de un formulario segun su nombre
        /// </summary>
        /// <param name="pNombreForm">Nombre Formulario</param>
        /// <returns></returns>
        public int Get_CodigoFormulario(string pNombreForm)
        {
            var form = from f in gDc.Fomulario
                       where f.Nombre == pNombreForm
                       select f;
            if (form.Count() > 0)
            {
                return form.First().ID;
            }
            return -1;
        }
        /// <summary>
        /// Metodo privado que devuelve todos los formularios de un Rol
        /// </summary>
        /// <param name="pIDRol">ID del rol</param>
        /// <returns></returns>
        private IEnumerable<Fomulario> Get_AllFormularios(int pIDRol)
        {
            return from f in gDc.Fomulario
                   join rf in gDc.Rol_Formulario on f.ID equals rf.IDFormulario
                   where rf.IDRol == pIDRol
                   select f;

        }

        /// <summary>
        /// Obtiene los Componentes  de un Formulario
        /// </summary>
        /// <param name="pIDRol">ID del Rol</param>
        /// <param name="pIDFormulario">ID del Formulario</param>
        /// <returns> DataTable con los componetes de un formulario indicando si pertenece al Rol</returns>
        public DataTable Get_AllFormulariosp(int pIDRol)
        {
            return Converter<Fomulario>.Convert(Get_AllFormularios(pIDRol).ToList());
        }

        /// <summary>
        /// Metodo que verifica si un componente esta habilitado
        /// </summary>
        /// <param name="pDTComponentes">Datable con todos los componetes del rol</param>
        /// <param name="pComponente">Nombre del componente a verificar</param>
        /// <returns>bool</returns>
        public bool Esta_habilitado(DataTable pDTComponentes, string pComponente)
        {
            foreach (DataRow m in pDTComponentes.Rows)
            {
                if (m[2].ToString().Equals(pComponente))
                {
                    return Convert.ToBoolean((int)m[4]);

                }
            }
            return false;
        }

        /// <summary>
        /// Verifica si un formulario esta habilitado
        /// </summary>
        /// <param name="pDTformularios">DataTable con los formularios de un rol</param>
        /// <param name="pFormulario">Nombre del formulario a verificar</param>
        /// <returns></returns>

        public bool Formulario_habilitado(DataTable pDTformularios, string pFormulario)
        {
            foreach (DataRow m in pDTformularios.Rows)
            {
                if (m[2].ToString().Equals(pFormulario))
                {
                    return Convert.ToBoolean((int)m[4]);

                }
            }
            return false;
        }

        public List<ModulosTree> getComponenentes(int pRolID, int pIDFormulario)
        {
            var componenteOfRol = (from cr in gDc.Rol_Componente
                                   where cr.ID_Rol == pRolID
                                   select cr).ToList();
            return (from c in gDc.Componente
                    where c.IDFormulario == pIDFormulario
                    select c).Select(x =>
                    new ModulosTree()
                    {
                        ID = x.ID,
                        IsChecked = isComponenteChecked(x.ID,pRolID),
                        Nombre = x.Nombre,
                        Hijos = new List<ModulosTree>(),
                        IsCollapsed = false
                    }).ToList();

        }
        public List<ModulosTree> getFormularios(int pRolID, int pIDModulo)
        {
            var formulariosOfRol = (from fr in gDc.Rol_Formulario
                                    where fr.IDRol == pRolID
                                    select fr).ToList();

            return (from f in gDc.Fomulario
                    where f.IDModulo == pIDModulo
                    select f).Select(x =>
                            new ModulosTree()
                            {
                                ID = x.ID,
                                Nombre = x.Nombre,
                                IsChecked = isFormularioChecked(x.ID, pRolID),
                                Hijos = getComponenentes(pRolID, x.ID),
                                IsCollapsed = false
                            }).ToList();
        }

        public bool isModuloChecked(int pModuloId, int pRolId) {

            return (from mr in gDc.Rol_Modulo
                    where mr.IDModulo == pModuloId
                    && mr.IDRol == pRolId
                    select mr).Count() > 0;
        }
        public bool isFormularioChecked(int pFormularioId, int pRolId)
        {

            return (from fr in gDc.Rol_Formulario
                    where fr.IDFormulario == pFormularioId
                    && fr.IDRol == pRolId
                    select fr).Count() > 0;
        }

        public bool isComponenteChecked(int pComponenteId, int pRolId)
        {

            return (from cr in gDc.Rol_Componente
                    where cr.ID_Componente == pComponenteId
                    && cr.ID_Rol == pRolId
                    select cr).Count() > 0;
        }
        public List<ModulosTree> GetModulos(int pRolID)
        {


            //var moduloOfRol = (from mr in gDc.Rol_Modulo
            //                   where mr.IDRol == pRolID
            //                   select mr).ToList();
            return (from m in gDc.Modulo
                    select m).Select(x =>
                        new ModulosTree()
                        {
                            ID = x.ID,
                            IsChecked =  isModuloChecked(x.ID, pRolID),
                            Nombre = x.Nombre,
                            Hijos = getFormularios(pRolID, x.ID),
                            IsCollapsed = false
                        }).ToList();


        }

        #endregion
    }
}
