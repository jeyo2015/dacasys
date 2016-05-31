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

    public class ABMUsuarioEmpleado
    {
        #region VariablesGlogales

        readonly DataContext dataContext = new DataContext();
        readonly ControlBitacora controlBitacora = new ControlBitacora();
        readonly ControlLogErrores controlErrores = new ControlLogErrores();
        readonly Encriptador abmEncriptador = new Encriptador();
        ABMEmpresa abmEmpresa;

        #endregion

        #region ABM_UsuarioEmpleado

        /// <summary>
        /// Inserta un nuevo Usuario para la Empresa
        /// </summary>
        /// <param name="pnombre">Nombre del Usuario Empleado</param>
        /// <param name="pLogin">Login de Inicio de Sesión del UsuarioEmpleado</param>
        /// <param name="pIDEmpresa">IDEmpresa a la que pertenece el UsuarioEmpleado</param>
        /// <param name="pIDRol">IDRol del rol de privilegios que tiene el UsuarioEmpleado</param>
        /// <param name="pPassword">Password de ingreso del UsuarioEmpleado</param>
        /// <param name="pChangepass">True si se desea que cambie pass, False si no esta obligado</param>
        /// <param name="pIDUsuario">Id del Usuario Que realiza la accion</param>
        /// <returns> 0 - No inserto Usuario
        ///           1 - Inserto Usuario
        ///           2 - Login no valido, debe ser mayor de 4
        ///           3 - ID empresa invalido
        ///           4 - Id rol invalido
        ///           5 - Pass invalido, mayor a 8
        ///           6 - No coincidepass
        ///           7 - Nombre Vacio
        /// </returns>
        public int Insertar(string pnombre, string pLogin, int pIDEmpresa, int pIDRol, string pPassword, string pPass2, bool? pChangepass,
                            string pIDUsuario)
        {
            //int v = ValidarCampos(pnombre, pLogin,pIDEmpresa,pIDRol, pPassword,pPass2,pChangepass);
            //if (v != 0)
            //    return v;
            String vPassEncriptada = abmEncriptador.Encriptar(pPassword);
            UsuarioEmpleado vUsuarioEmpleado = new UsuarioEmpleado();
            vUsuarioEmpleado.Login = pLogin;
            vUsuarioEmpleado.IDEmpresa = pIDEmpresa;
            vUsuarioEmpleado.IDRol = pIDRol;
            vUsuarioEmpleado.Nombre = pnombre;
            vUsuarioEmpleado.Password = vPassEncriptada;
            vUsuarioEmpleado.FechaCreacion = DateTime.Today;
            vUsuarioEmpleado.Estado = true;
            vUsuarioEmpleado.changepass = pChangepass;
            try
            {
                dataContext.UsuarioEmpleado.InsertOnSubmit(vUsuarioEmpleado);
                dataContext.SubmitChanges();
                controlBitacora.Insertar("Se inserto un nuevo Usuario", pIDUsuario);
                return 1;
            }
            catch (Exception ex)
            {
                controlErrores.Insertar("NLogin", "ABMUsuarioEmpleado", "Insertar", ex);
                return 0;
            }

        }

        private int ValidarCampos(string pNombre, string pLogin, int pIDEmpresa, int pIDRol, string pPassword, string pPass2, bool pChangepass)
        {
            if (pLogin.Length < 4)
                return 2;
            else
                // if (pLogin.Equals("adminadmin"))
                // return 8;
                if (pIDEmpresa == 0)
                    return 3;
            if (pIDRol == 0)
                return 4;
            if (pPassword.Length < 2)
                return 5;
            else
                if (!pPassword.Equals(pPass2))
                    return 6;
            if (pNombre.Length < 6)
                return 7;
            //  else
            // if (pNombre.Equals("Administrador"))
            //return 9;
            return 0;   // todo ok;
        }

        /// <summary>
        /// Modificar el usuario de una empresa
        /// </summary>
        /// <param name="pNombre">Nombre del Usuario Empleado</param>
        /// <param name="pLogin">Login de Inicio de Sesión del UsuarioEmpleado</param>
        /// <param name="pIDEmpresa">IDEmpresa a la que pertenece el UsuarioEmpleado</param>
        /// <param name="pIDRol">IDRol del rol de privilegios que tiene el UsuarioEmpleado</param>
        /// <param name="pPassword">Password de ingreso del UsuarioEmpleado</param>
        /// <param name="pChangepass">True si se desea que cambie pass, False si no esta obligado</param>
        /// <param name="pIDUsuario">Id del Usuario Que realiza la accion</param>
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
        public int Modificar(string pNombre, string pLogin, int pIDEmpresa, int pIDRol, string pPassword, string pPass2, bool? pChangepass,
                            string pIDUsuario)
        {
            //int v = ValidarCampos(pNombre, pLogin, pIDEmpresa, pIDRol, pPassword, pPass2, pChangepass);
            //if (v != 0)
            //    return v;
            var sql = from e in dataContext.UsuarioEmpleado
                      where e.Login == pLogin && e.IDEmpresa == pIDEmpresa
                      select e;

            if (sql.Count() > 0)
            {
                String vPassEncriptada = abmEncriptador.Encriptar(pPassword);
                sql.First().IDRol = pIDRol;
                sql.First().Password = vPassEncriptada;
                sql.First().changepass = pChangepass;
                sql.First().Nombre = pNombre;
                try
                {
                    dataContext.SubmitChanges();
                    controlBitacora.Insertar("Se modifico un Usuario", pIDUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    controlErrores.Insertar("NLogin", "ABMUsuarioEmpleado", "Modificar", ex);
                    return 8;
                }
            }
            return 0;

        }

        /// <summary>
        /// Pone a un UsuarioEmpleado como Estado false, o sea deshabilitado
        /// </summary>
        /// <param name="pLogin">Es el ID del UsuarioEmpleado a eliminar</param>
        /// <returns> Return 0 - No existe Usuario
        ///                    1 - Se elimino correctamente
        ///                    2 - No se pudo eliminar</returns>
        public int Eliminar(string pLogin, int pIdEmpresa, string pUsuario)
        {
            var sql = from e in dataContext.UsuarioEmpleado
                      where e.Login == pLogin && e.IDEmpresa == pIdEmpresa
                      select e;

            if (sql.Count() > 0)
            {
                sql.First().Estado = false;
                try
                {
                    dataContext.SubmitChanges();
                    controlBitacora.Insertar("Se elimino un Usuario, login: " + pLogin, pUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    controlErrores.Insertar("NLogin", "ABMUsuarioEmpleado", "Eliminar", ex);
                    return 2;

                }

            }
            return 0;
        }
        #endregion

        public SessionDto verificar_empleado(String pUsuario, String pPass, String pEmpresa)
        {
            var sessionReturn = new SessionDto();
            var empresa = (from emp in dataContext.Empresa
                           where emp.Login == pEmpresa && emp.Estado == true
                           select emp).FirstOrDefault();
            if (empresa != null)
            {
                String vPassEncriptada = abmEncriptador.Encriptar(pPass);
                int vIdEmpresa = empresa.ID;
                int lic = Verificar_licencia(empresa.IDClinica); //0Desa 1 habi

                var empleado = (from empl in dataContext.UsuarioEmpleado
                                where empl.Login == pUsuario && empl.IDEmpresa == vIdEmpresa
                                select empl).FirstOrDefault();
                if (empleado != null)
                {
                    if (empleado.Password == vPassEncriptada)
                        if (lic == 1)
                        {

                            sessionReturn.Verificar = 3;
                            sessionReturn.ChangePass = empleado.changepass ?? false;
                            sessionReturn.loginUsuario = pUsuario;
                            sessionReturn.IDConsultorio = vIdEmpresa;
                            sessionReturn.Nombre = empleado.Nombre;
                            sessionReturn.IDClinica = empresa.IDClinica;
                            sessionReturn.IDRol = empleado.IDRol;
                            sessionReturn.IsDacasys = pEmpresa.ToUpper().Equals("DACASYS");
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

        public int Get_DirefenciaHora()
        {
            var sql = from p in dataContext.ParametroSistemas
                      where p.Elemento == "DiferenciaHora"
                      select p;
            if (sql.Count() > 0)
            {
                return (int)sql.First().ValorI;
            }
            return 0;
        }

        public void verificar_todas_licencias()
        {
            var Laste = from e in dataContext.Envio
                        select e;
            if (Laste.Count() > 0)
            {
                if (Laste.First().Fecha.ToShortDateString().Equals(DateTime.Now.AddHours(Get_DirefenciaHora()).ToShortDateString()))//ya envio
                    return;
                else
                {
                    Laste.First().Fecha = DateTime.Now.AddHours(Get_DirefenciaHora());

                    dataContext.SubmitChanges();
                }
            }
            else
            {
                Envio vEnvio = new Envio();
                vEnvio.id = 1;
                vEnvio.Fecha = DateTime.Now.AddHours(Get_DirefenciaHora());
                dataContext.Envio.InsertOnSubmit(vEnvio);
                dataContext.SubmitChanges();
            }
            EnviarCorreo ec = new EnviarCorreo();
            abmEmpresa = new ABMEmpresa();
            ec.Iniciar(abmEmpresa.Get_IDempresasp());

        }

        private int Verificar_licencia(int pIdClinica)
        {

            var lic = from l in dataContext.Licencia
                      where l.IDClinica == pIdClinica
                      orderby l.FechaInicio descending
                      select l;

            if (lic.Count() > 0)
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

        private IEnumerable<UsuarioEmpleado> Get_Empleado(String pLoginUsuario, string pLoginEmpresa)
        {

            return from e in dataContext.UsuarioEmpleado
                   join em in dataContext.Empresa on e.IDEmpresa equals em.ID
                   where e.Estado == true && e.Login == pLoginUsuario &&
                   em.Login == pLoginEmpresa
                   select e;

        }

        /// <summary>
        /// Devuelve el Empleado
        /// </summary>
        /// <param name="pLoginUsuario">ID del Usuario </param>
        /// <param name="pLoginEmpresa">Login Empresa empresa </param>
        /// <returns>un DataTable que contiene el usuario</returns>
        public DataTable Get_Empleadop(String pLoginUsuario, string pLoginEmpresa)
        {
            return Converter<UsuarioEmpleado>.Convert(Get_Empleado(pLoginUsuario, pLoginEmpresa).ToList());
        }

        private IEnumerable<UsuarioEmpleado> Get_Empleado(String pLoginUsuario, int pIDEmpresa)
        {

            return from e in dataContext.UsuarioEmpleado
                   join em in dataContext.Empresa on e.IDEmpresa equals em.ID
                   where e.Estado == true && e.Login == pLoginUsuario &&
                   em.ID == pIDEmpresa
                   select e;

        }

        /// <summary>
        /// Devuelve el Empleado 
        /// </summary>
        /// <param name="pLoginUsuario">ID del Usuario </param>
        /// <param name="pIDEmpresa">ID empresa </param>
        /// <returns>un DataTable que contiene el usuario</returns>
        public DataTable Get_Empleadop(String pLoginUsuario, int pIDEmpresa)
        {
            return Converter<UsuarioEmpleado>.Convert(Get_Empleado(pLoginUsuario, pIDEmpresa).ToList());
        }

        private IEnumerable<UsuarioEmpleado> Buscar_Usuarios(String pLoginUsuario, int pIDEmpresa)
        {

            return from e in dataContext.UsuarioEmpleado
                   where e.Estado == true && (e.Login.Contains(pLoginUsuario) || e.Nombre.Contains(pLoginUsuario))
                   && e.IDEmpresa == pIDEmpresa
                   select e;

        }

        public DataTable Buscar_Usuariosp(string pLogin, int pIDEmpresa)
        {
            return Converter<UsuarioEmpleado>.Convert(Buscar_Usuarios(pLogin, pIDEmpresa).ToList());
        }

        public List<UsuarioDto> Get_Usuarios(int pIDEmpresa)
        {
            return (from e in dataContext.UsuarioEmpleado
                    where e.Estado == true
                    && e.IDEmpresa == pIDEmpresa
                    select e).Select(x => new UsuarioDto()
                    {
                        changepass = x.changepass,
                        ConfirmPass = abmEncriptador.Desencriptar(x.Password),
                        Estado = x.Estado,
                        IDEmpresa = x.IDEmpresa,
                        IDRol = x.IDRol,
                        Login = x.Login,
                        Nombre = x.Nombre,
                        Password = abmEncriptador.Desencriptar(x.Password)
                    }).ToList();
        }

        //public DataTable Get_Usuariop(string pLogin, int pIDEmpresa)
        //{
        //    return Converter<UsuarioEmpleado>.Convert(Get_Usuarios(pLogin, pIDEmpresa).ToList());
        //}

        public UsuarioDto Get_Usuario(string pLogin, int pIDEmpresa)
        {
            return (from e in dataContext.UsuarioEmpleado
                    where e.Estado == true && e.Login == pLogin
                    && e.IDEmpresa == pIDEmpresa
                    select new UsuarioDto
                    {
                        changepass = e.changepass,
                        ConfirmPass = abmEncriptador.Desencriptar(e.Password),
                        Estado = e.Estado,
                        IDEmpresa = e.IDEmpresa,
                        IDRol = e.IDRol,
                        Login = e.Login,
                        Nombre = e.Nombre,
                        Password = abmEncriptador.Desencriptar(e.Password)
                    }).FirstOrDefault();
        }

        /// <summary>
        /// Modifica el usuario por defaut de un Empresa
        /// </summary>
        /// <param name="pIDempresa">Id de la Empresa</param>
        /// <param name="pNombre">Nombre del Usuario a Modificar</param>
        /// <param name="pLogin">Nuevo Login del usuario</param>
        /// <param name="pPass">Nuevao Pass</param>
        /// <returns> 0 - Se modifico Correctamente
        ///      -1 - Error en el nombre
        ///      -2 - Error en Login
        ///      -3 - Errpr en Pass
        ///      -4 - El login ya existe
        ///      -5 - No existe user Default 
        ///      -6 - No Se pudo Modificar</returns>
        public int Modificar_UserDefault(int pIDempresa, string pNombre, string pLogin, string pPass)
        {
            int v = Validar_CamposD(pNombre, pLogin, pPass);
            if (v != 0)
                return v;

            if (Buscar_Usuariosp(pLogin, pIDempresa).Rows.Count == 0)
            {
                var vUserD = from u in dataContext.UsuarioEmpleado
                             where u.Login == "admin" && u.IDEmpresa == pIDempresa
                             select u;
                if (vUserD.Count() > 0)
                {
                    UsuarioEmpleado vNewUser = new UsuarioEmpleado();
                    vNewUser.Login = pLogin;
                    vNewUser.Nombre = pNombre;
                    string vnewp = abmEncriptador.Encriptar(pPass);
                    vNewUser.Password = vnewp;
                    vNewUser.IDRol = vUserD.First().IDRol;
                    vNewUser.IDEmpresa = pIDempresa;
                    vNewUser.Estado = true;
                    vNewUser.FechaCreacion = vUserD.First().FechaCreacion;
                    vNewUser.changepass = false;
                    dataContext.UsuarioEmpleado.InsertOnSubmit(vNewUser);
                    dataContext.UsuarioEmpleado.DeleteOnSubmit(vUserD.First());

                    try
                    {
                        dataContext.SubmitChanges();
                        controlBitacora.Insertar("Se modifico un Usuario Default", pLogin);
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        controlErrores.Insertar("NLogin", "ABMUsuarioEmpleado", "Modificar_UserDefault", ex);
                        return -6;
                    }
                }
                return -5;
            }
            return -4;

        }

        /// <summary>
        /// Valida que los parametros no estes vacios
        /// </summary>
        /// <param name="pNombre">Nombre de Usuario</param>
        /// <param name="pLogin">Login de Usuario</param>
        /// <param name="pPass">Pass de Usuario</param>
        /// <returns>0  - Todo ok
        ///          -1 - Error Nombre
        ///          -2 - Error Login    
        ///          -3 - Error Pass</returns>
        private int Validar_CamposD(string pNombre, string pLogin, string pPass)
        {
            if (pNombre.Length < 5)
            {
                return -1;
            }
            if (pLogin.Length < 3)
                return -2;
            if (pPass.Length < 3)
            {
                return -3;
            }
            return 0;
        }

        /// <summary>
        /// Cambia el Pass
        /// </summary>
        /// <param name="pLogin">Login de Usuario</param>
        /// <param name="pPass">Nueva Pass</param>
        /// <param name="pIDempresa">Id de la Empresa</param>
        /// <returns> 0 - Modifico Correctamente
        /// -3 - Error en el Pass
        /// -5 - No existe Usuario
        /// -6 - No Se pudo Modificar</returns>
        public int Cambiar_pass(string pLogin, string pPass, string loginEmpresa)
        {

            var user = from u in dataContext.UsuarioEmpleado
                       from c in dataContext.Empresa
                       where u.Login == pLogin &&
                       c.Login.ToUpper() == loginEmpresa.ToUpper()
                       && c.ID == u.IDEmpresa
                       select u;
            if (user.Count() > 0)
            {
                String vPassEncriptada = abmEncriptador.Encriptar(pPass);
                user.First().changepass = false;
                user.First().Password = vPassEncriptada;
                try
                {
                    dataContext.SubmitChanges();
                    controlBitacora.Insertar("Se modifico un Usuario", pLogin);
                    return 1;
                }
                catch (Exception ex)
                {
                    controlErrores.Insertar("NLogin", "ABMUsuarioEmpleado", "Cambiar_Pass", ex);
                    return 0;
                }

            }
            return 0;
        }

        public int Resetear_Contrasena(string pLogin, String pLEmpresa)
        {
            var empresa = (from e in dataContext.Empresa
                           where e.Login == pLEmpresa
                           select e).FirstOrDefault();
            if (empresa != null)
            {
                var usuario = (from u in dataContext.UsuarioEmpleado
                               where u.Login == pLogin &&
                               u.IDEmpresa == empresa.ID && u.Estado == true
                               select u).FirstOrDefault();
                if (usuario != null)
                {
                    String vNewPass = abmEncriptador.Generar_Aleatoriamente();
                    String vNewPassEncriptada = abmEncriptador.Encriptar(vNewPass);
                    usuario.Password = vNewPassEncriptada;
                    abmEmpresa = new ABMEmpresa();
                    Clinica vclinica = abmEmpresa.Get_ClinicaByID(empresa.IDClinica);
                    usuario.changepass = true;
                    try
                    {
                        dataContext.SubmitChanges();
                        controlBitacora.Insertar("Se reseteo contraseña de usuario " + pLogin + " Consultorio " + vclinica.Nombre, "0000");
                        SMTP vSMTP = new SMTP();
                        String vMensaje = "Consultorio " + vclinica.Nombre + " se solicito el reseteo de contrasena del \n" +
                                         " usuario : " + pLogin + ", con nombre " + usuario.Nombre +
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
                        controlErrores.Insertar("Nlogin", "ABMUsuarioEmpleado", "Reseto_Constrasena", ex);
                        return 2;
                    }
                }
                return 1;
            }
            return 0;
        }

        private IEnumerable<Planilla_Vendedores> Get_PlanillaUser(DateTime pfechaaux, string pIDUsuario)
        {

            return from pv in dataContext.Planilla_Vendedores
                   where pv.Fecha == pfechaaux && pv.IDUsuario == pIDUsuario
                   select pv;

        }

        private IEnumerable<Planilla_Vendedores> Get_PlanillaL(DateTime pfechaaux)
        {

            return from pv in dataContext.Planilla_Vendedores
                   where pv.Fecha == pfechaaux
                   select pv;

        }

        public DataTable Get_Planilla(DateTime pfechaaux, int pIDRol, string pIDUsuario)
        {
            var sql = from r in dataContext.Rol
                      where r.ID == pIDRol
                      select r;
            if (sql.First().Nombre == "Administrador DACASYS")
                return Converter<Planilla_Vendedores>.Convert(Get_PlanillaL(pfechaaux).ToList());
            else
                return Converter<Planilla_Vendedores>.Convert(Get_PlanillaUser(pfechaaux, pIDUsuario).ToList());
        }

        public void clean_logs()
        {
            dataContext.CleanLog();
        }
    }
}