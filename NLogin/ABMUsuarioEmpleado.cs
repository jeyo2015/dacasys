namespace NLogin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Datos;
    using DataTableConverter;
    using System.Data;
    using NEventos;
    using Herramientas;
    using System.Data.Linq;
    public class ABMUsuarioEmpleado
    {
        #region VariablesGlogales

        readonly static ContextoDataContext dataContext = new ContextoDataContext();

        ABMEmpresa abmEmpresa;

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Inserta un nuevo Usuario para la Empresa
        /// </summary>
        /// <param name="nombre">Nombre del Usuario Empleado</param>
        /// <param name="login">Login de Inicio de Sesión del UsuarioEmpleado</param>
        /// <param name="iDEmpresa">IDEmpresa a la que pertenece el UsuarioEmpleado</param>
        /// <param name="iDRol">IDRol del rol de privilegios que tiene el UsuarioEmpleado</param>
        /// <param name="password">Password de ingreso del UsuarioEmpleado</param>
        /// <param name="passwordConfirma">Password de ingreso del UsuarioEmpleado</param>
        /// <param name="nuevoPassword">True si se desea que cambie pass, False si no esta obligado</param>
        /// <param name="idUsuario">Id del Usuario Que realiza la accion</param>
        /// <returns> 0 - No inserto Usuario
        ///           1 - Inserto Usuario
        ///           2 - Login no valido, debe ser mayor de 4
        ///           3 - ID empresa invalido
        ///           4 - Id rol invalido
        ///           5 - Pass invalido, mayor a 8
        ///           6 - No coincidepass
        ///           7 - Nombre Vacio
        /// </returns>
        public static int Insertar(string nombre, string login, int iDEmpresa, int iDRol, 
            string password, string passwordConfirma, bool? nuevoPassword,string idUsuario)
        {
            //int v = ValidarCampos(nombre, login,idEmpresa,iDRol, password,passwordConfirma,nuevoPassword);
            //if (v != 0)
            //    return v;
            var vPassEncriptada = Encriptador.Encriptar(password);
            var vUsuarioEmpleado = new UsuarioEmpleado();
            vUsuarioEmpleado.Login = login;
            vUsuarioEmpleado.IDEmpresa = iDEmpresa;
            vUsuarioEmpleado.IDRol = iDRol;
            vUsuarioEmpleado.Nombre = nombre;
            vUsuarioEmpleado.Password = vPassEncriptada;
            vUsuarioEmpleado.FechaCreacion = DateTime.Today;
            vUsuarioEmpleado.Estado = true;
            vUsuarioEmpleado.changepass = nuevoPassword;
            try
            {
                dataContext.UsuarioEmpleado.InsertOnSubmit(vUsuarioEmpleado);
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se inserto un nuevo Usuario", idUsuario);
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMUsuarioEmpleado", "InsertarModulo", ex);
                return 0;
            }
        }

        /// <summary>
        /// Modificar el usuario de una empresa
        /// </summary>
        /// <param name="nombre">Nombre del Usuario Empleado</param>
        /// <param name="login">Login de Inicio de Sesión del UsuarioEmpleado</param>
        /// <param name="idEmpresa">IDEmpresa a la que pertenece el UsuarioEmpleado</param>
        /// <param name="idRol">IDRol del rol de privilegios que tiene el UsuarioEmpleado</param>
        /// <param name="password">Password de ingreso del UsuarioEmpleado</param>
        /// <param name="passwordConfirma">Password de ingreso del UsuarioEmpleado</param>
        /// <param name="nuevoPassword">True si se desea que cambie pass, False si no esta obligado</param>
        /// <param name="idUsuario">Id del Usuario Que realiza la accion</param>
        /// <returns> 0 - No Existe Usuario
        ///           1 - Se Modifico Usuario
        ///           2 - Login no valido, debe ser mayor de 4
        ///           3 - ID empresa invalido
        ///           4 - Id rol invalido
        ///           5 - Pass invalido, mayor a 8
        ///           6 - No coincidepass
        ///           7 - Nombre Vacio
        ///           8 - No se pudo Modificar
        ///          
        /// </returns>
        public static int Modificar(string nombre, string login, int idEmpresa, int idRol, 
            string password, string passwordConfirma, bool? nuevoPassword, string idUsuario)
        {
            //int v = ValidarCampos(nombre, login, idEmpresa, iDRol, password, passwordConfirma, nuevoPassword);
            //if (v != 0)
            //    return v;
            var sql = from e in dataContext.UsuarioEmpleado
                      where e.Login == login && e.IDEmpresa == idEmpresa
                      select e;

            if (sql.Any())
            {
                String vPassEncriptada = Encriptador.Encriptar(password);
                sql.First().IDRol = idRol;
                sql.First().Password = vPassEncriptada;
                sql.First().changepass = nuevoPassword;
                sql.First().Nombre = nombre;
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se modifico un Usuario", idUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NLogin", "ABMUsuarioEmpleado", "Modificar", ex);
                    return 8;
                }
            }
            return 0;

        }

        /// <summary>
        /// Pone a un UsuarioEmpleado como Estado false, o sea deshabilitado
        /// </summary>
        /// <param name="login">Es el ID del UsuarioEmpleado a eliminar</param>
        /// <param name="idEmpresa">Es el ID empresa</param>
        /// <param name="idUsuario">Es el ID del usuario</param>
        /// <returns> Return 0 - No existe Usuario
        ///                    1 - Se elimino correctamente
        ///                    2 - No se pudo eliminar</returns>
        public static int Eliminar(string login, int idEmpresa, string idUsuario)
        {
            var sql = from e in dataContext.UsuarioEmpleado
                      where e.Login == login && e.IDEmpresa == idEmpresa
                      select e;

            if (sql.Any())
            {
                sql.First().Estado = false;
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se elimino un Usuario, login: " + login, idUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NLogin", "ABMUsuarioEmpleado", "Eliminar", ex);
                    return 2;

                }

            }
            return 0;
        }

        public static SessionDto VerificarEmpleado(string idUsuario, string password, string idEmpresa)
        {
            var sessionReturn = new SessionDto();
            var empresa = (from emp in dataContext.Empresa
                           where emp.Login.ToUpper() == idEmpresa.ToUpper() && emp.Estado == true
                           select emp).FirstOrDefault();
            if (empresa != null)
            {
                String vPassEncriptada = Encriptador.Encriptar(password);
                int vIdEmpresa = empresa.ID;
                int lic = VerificarLicencia(empresa.IDClinica); //0Desa 1 habi

                var empleado = (from empl in dataContext.UsuarioEmpleado
                                where empl.Login == idUsuario && empl.IDEmpresa == vIdEmpresa
                                select empl).FirstOrDefault();
                if (empleado != null)
                {
                    if (empleado.Password == vPassEncriptada)
                        if (lic == 1)
                        {

                            sessionReturn.Verificar = 3;
                            sessionReturn.ChangePass = empleado.changepass ?? false;
                            sessionReturn.loginUsuario = idUsuario;
                            sessionReturn.IDConsultorio = vIdEmpresa;
                            sessionReturn.Nombre = empleado.Nombre;
                            sessionReturn.IDClinica = empresa.IDClinica;
                            sessionReturn.IDRol = empleado.IDRol;
                            sessionReturn.IsDacasys = idEmpresa.ToUpper().Equals("DACASYS");
                            return sessionReturn;
                        }
                        else
                        {
                            sessionReturn.Verificar = 1;
                            return sessionReturn;
                        }

                    sessionReturn.Verificar = 4;
                    return sessionReturn;
                }
                sessionReturn.Verificar = 2;
                return sessionReturn;


            }
            sessionReturn.Verificar = 0; ///esta desahabilitada 
            return sessionReturn;
        }

        public static List<UsuarioDto> ObtenerListaUsuario(int idEmpresa)
        {
            return (from e in dataContext.UsuarioEmpleado
                    where e.Estado == true
                    && e.IDEmpresa == idEmpresa
                    select e).Select(x => new UsuarioDto()
                    {
                        changepass = x.changepass,
                        ConfirmPass = Encriptador.Desencriptar(x.Password),
                        Estado = x.Estado,
                        IDEmpresa = x.IDEmpresa,
                        IDRol = x.IDRol,
                        Login = x.Login,
                        Nombre = x.Nombre,
                        Password = Encriptador.Desencriptar(x.Password)
                    }).ToList();
        }

        public static UsuarioDto ObtenerUsuario(string login, int idEmpresa)
        {
            return (from e in dataContext.UsuarioEmpleado
                    where e.Estado == true && e.Login == login
                    && e.IDEmpresa == idEmpresa
                    select new UsuarioDto
                    {
                        changepass = e.changepass,
                        ConfirmPass = Encriptador.Desencriptar(e.Password),
                        Estado = e.Estado,
                        IDEmpresa = e.IDEmpresa,
                        IDRol = e.IDRol,
                        Login = e.Login,
                        Nombre = e.Nombre,
                        Password = Encriptador.Desencriptar(e.Password)
                    }).FirstOrDefault();
        }

        /// <summary>
        /// Cambia el Pass
        /// </summary>
        /// <param name="login">Login de Usuario</param>
        /// <param name="password">Nueva Pass</param>
        /// <param name="loginEmpresa">Id de la Empresa</param>
        /// <returns> 0 - Modifico Correctamente
        /// -3 - Error en el Pass
        /// -5 - No existe Usuario
        /// -6 - No Se pudo Modificar</returns>
        public static int CambiarContrasena(string login, string password, string loginEmpresa)
        {
            var user = from u in dataContext.UsuarioEmpleado
                       from c in dataContext.Empresa
                       where u.Login == login &&
                       c.Login.ToUpper() == loginEmpresa.ToUpper()
                       && c.ID == u.IDEmpresa
                       select u;
            if (user.Any())
            {
                String vPassEncriptada = Encriptador.Encriptar(password);
                user.First().changepass = false;
                user.First().Password = vPassEncriptada;
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se modifico un Usuario", login);
                    return 1;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NLogin", "ABMUsuarioEmpleado", "Cambiar_Pass", ex);
                    return 0;
                }

            }
            return 0;
        }

        public static int ResetearContrasena(string login, string idEmpresa)
        {
            var empresa = (from e in dataContext.Empresa
                           where e.Login == idEmpresa
                           select e).FirstOrDefault();
            dataContext.Refresh(RefreshMode.OverwriteCurrentValues, empresa);
            if (empresa != null)
            {
                var usuario = (from u in dataContext.UsuarioEmpleado
                               where u.Login == login &&
                               u.IDEmpresa == empresa.ID && u.Estado == true
                               select u).FirstOrDefault();
                if (usuario != null)
                {
                    String vNewPass = Encriptador.GenerarAleatoriamente();
                    String vNewPassEncriptada = Encriptador.Encriptar(vNewPass);
                    usuario.Password = vNewPassEncriptada;
                    Clinica vclinica = ABMEmpresa.ObtenerClinica(empresa.IDClinica);
                    usuario.changepass = true;
                    try
                    {
                        dataContext.SubmitChanges();
                        ControlBitacora.Insertar("Se reseteo contraseña de usuario " + login + " Consultorio " + vclinica.Nombre, "0000");
                        SMTP vSMTP = new SMTP();
                        String vMensaje = "Consultorio " + vclinica.Nombre + " se solicito el reseteo de contrasena del \n" +
                                         " usuario : " + login + ", con nombre " + usuario.Nombre +
                                             "\nSu nueva contrasena es: " + vNewPass +
                                             ".\nSaludos,\nOdontoweb";
                        vSMTP.Datos_Mensaje(empresa.Email, vMensaje, "Reseteo de Constrasena - Odontoweb");
                        if (vSMTP.Enviar_Mail() == 1)
                            return 3;
                        else
                            return 2;

                    }
                    catch (Exception ex)
                    {
                        ControlLogErrores.Insertar("Nlogin", "ABMUsuarioEmpleado", "Reseto_Constrasena", ex);
                        return 2;
                    }
                }
                return 1;
            }
            return 0;
        }

        #endregion

        #region Metodos Privados

        private static int VerificarLicencia(int idClinica)
        {

            var lic = from l in dataContext.Licencia
                      where l.IDClinica == idClinica
                      orderby l.FechaInicio descending
                      select l;

            if (lic.Any())
            {
                TimeSpan diff = lic.First().FechaFin.Subtract(DateTime.Now.AddHours(3));
                int dias = diff.Days + 1;
                if (dias > -3 && dias <= 0)
                    return 0;
                else
                    return 1;
            }

            return -1;

        }
        
        #endregion
    }
}