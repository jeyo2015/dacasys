namespace NLogin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Datos;
    using NEventos;
    using Herramientas;

    public class ABMUsuarioCliente
    {
        #region VariablesGlogales

        readonly static ContextoDataContext dataContext = new ContextoDataContext();

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Inserta un nuevo UsuarioCliente al sistema
        /// </summary>
        /// <param name="login">Login de ingreso al sistema del UsuarioCliente</param>
        /// <param name="password">Pasword de ingreso al sistema</param>
        /// <param name="usuario">Fecha de Creación del UsuarioCliente</param>
        public static int Insertar(string login, string password, string usuario)
        {
            var sql = from us in dataContext.UsuarioCliente
                      where us.Login == login
                      select us;
            if (sql.Any())
            {
                return 2;
            }
            else
            {
                var passwordEncriptado = Encriptador.Encriptar(password);
                UsuarioCliente vUsuarioCliente = new UsuarioCliente();
                vUsuarioCliente.Login = login;
                vUsuarioCliente.Password = passwordEncriptado;
                vUsuarioCliente.FechaCreacion = DateTime.Today;
                vUsuarioCliente.Estado = true;
                vUsuarioCliente.changepass = true;
                try
                {
                    dataContext.UsuarioCliente.InsertOnSubmit(vUsuarioCliente);
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se Inserto un nuevo cliente", usuario);

                    return 1;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NLogin", "ABMCliente", "ABMUsuarioCliente", ex);
                    return 0;
                }
            }

        }

        /// <summary>
        /// Modifica los datos del UsuarioCliente especificado por su ID
        /// </summary>
        /// <param name="login">Login del UsuarioCliente a modificar</param>
        /// <param name="password">Password del UsuarioCliente a modificar</param>
        public static void Modificar(string login, string password)
        {
            var sql = from e in dataContext.UsuarioCliente
                      where e.Login == login
                      select e;

            if (sql.Any())
            {
                var vPassEncriptada = Encriptador.Encriptar(password);
                sql.First().Password = vPassEncriptada;
                dataContext.SubmitChanges();
            }
        }

        /// <summary>
        /// Pone a un UsuarioCliente como Estado false, o sea deshabilitado
        /// </summary>
        /// <param name="login">Es el login del UsuarioCliente a eliminar</param>
        public static void Eliminar(string login)
        {
            var sql = from e in dataContext.UsuarioCliente
                      where e.Login == login
                      select e;

            if (!sql.Any()) return;
            sql.First().Estado = false;
            dataContext.SubmitChanges();
        }

        public static SessionDto VerificarCliente(string idUsuario, string password)
        {
            var sessionReturn = new SessionDto();
            var cliente = (from cli in dataContext.UsuarioCliente
                           where cli.Login == idUsuario
                           select cli).FirstOrDefault();
            if (cliente != null)
            {
                var paciente = (from p in dataContext.Paciente
                                from pc in dataContext.Cliente_Paciente
                                where p.id_paciente == pc.id_paciente
                                && pc.id_usuariocliente == cliente.Login
                                && pc.IsPrincipal == true
                                select p).FirstOrDefault();
                String vPassEncriptada = Encriptador.Encriptar(password);
                if (cliente.Password == vPassEncriptada)
                {
                    sessionReturn.loginUsuario = idUsuario;
                    sessionReturn.IDConsultorio = -1;
                    sessionReturn.ChangePass = cliente.changepass ?? false;
                    sessionReturn.IDClinica = -1;
                    sessionReturn.IsDacasys = false;
                    sessionReturn.Verificar = 3;
                    sessionReturn.Nombre = paciente != null ? paciente.nombre + " " + paciente.apellido : "";
                    sessionReturn.IDRol = -1;
                    return sessionReturn;
                }
                else
                    sessionReturn.Verificar = 4;
                return sessionReturn;
            }
            else
            {
                sessionReturn.Verificar = 2;
                return sessionReturn;
            }
        }

        /// <summary>
        /// Permite Cambiar el Pass de un Cliente
        /// </summary>
        /// <param name="login">Login del Cliente</param>
        /// <param name="password">Nueva Password</param>
        /// <returns>0 - Todo ok
        /// -3 - Error password
        /// -5 - No existe Cliente
        /// -6 - No se Pudo Modificar Pass</returns>
        public static int CambiaContrasena(string login, string password)
        {

            var user = from u in dataContext.UsuarioCliente
                       where u.Login == login
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
                    ControlLogErrores.Insertar("NLogin", "ABMUsuarioCliente", "Cambiar_Pass", ex);
                    return 0;
                }
            }
            return 0;
        }

        public static int ResetearContrasena(string login)
        {
            var user = (from u in dataContext.UsuarioCliente
                        where u.Login == login
                        select u).FirstOrDefault();
            if (user != null)
            {
                String vPassNew = Encriptador.GenerarAleatoriamente();
                String vPassNewEncriptada = Encriptador.Encriptar(vPassNew);
                user.changepass = true;
                user.Password = vPassNewEncriptada;
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se reseteo password de Usuario " + login, "0000");
                    EnviarReseteoContrasena(login, vPassNew);
                    return 3;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NLogin", "ABMUsuarioCliente", "ResetearContrasena", ex);
                    return 2;
                }
            }
            else
            {
                return 1;
            }
        }

        public static UsuarioDto ObtenerDatosClientePaciente(string loginUsuario)
        {
            return (from c_p in dataContext.Cliente_Paciente
                    from pac in dataContext.Paciente
                    where c_p.id_usuariocliente == loginUsuario
                        && pac.id_paciente == c_p.id_paciente
                    select new UsuarioDto()
                    {
                        Login = loginUsuario,
                        Nombre = pac.nombre +" " + pac.apellido

                    }).FirstOrDefault();
        }
        public static PacienteDto ObtenerDatoPaciente(string loginUsuario)
        {
            return (from c_p in dataContext.Cliente_Paciente
                    from pac in dataContext.Paciente
                    where c_p.id_usuariocliente == loginUsuario
                        && pac.id_paciente == c_p.id_paciente
                    select new PacienteDto()
                    {
                       Email = pac.email,
                       NombrePaciente = pac.nombre + " " + pac.apellido

                    }).FirstOrDefault();
        }

        public static PacienteDto ObtenerDatoPaciente(int idPaciente)
        {
            return (from pac in dataContext.Paciente
                    where  pac.id_paciente == idPaciente
                    select new PacienteDto()
                    {
                        Email = pac.email,
                        NombrePaciente = pac.nombre + " " + pac.apellido

                    }).FirstOrDefault();
        }
        public static void EnviarCorreoDeBienvenida(int idEmpresa, string email, string password, string idCliente)
        {
            var em = (from e in dataContext.Empresa
                      from c in dataContext.Clinica
                      where e.ID == idEmpresa && e.Estado == true
                      && c.ID == e.IDClinica
                      select c).FirstOrDefault();
            String vMensaje = "";
            SMTP vSMTP = new SMTP();
            if (em != null)
            {


                if (password.Equals(""))
                {//Solo se asigna nueva empresa
                    vMensaje = "Estimado Cliente ha sido suscrito a " + em.Nombre.ToUpper() + ". \nIngrese a la pagina " +
                        "para poder informarse acerca de este consultorio. Sus datos de acceso son los mismos." +
                        "\nSaludos,\nOdontoweb";
                    vSMTP.Datos_Mensaje(email, vMensaje, "Nuevo Consultorio");
                }
                else
                {
                    vMensaje = "Bienvenido a Odontoweb.\n Ha sido suscrito a " + em.Nombre.ToUpper() +
                       " sus datos para ingresar al sistema son: \n" + "Usuario: " + idCliente +
                       "\nContraseña: " + password + "\nSaludos,\nOdontoweb.";
                    vSMTP.Datos_Mensaje(email, vMensaje, "Bienvenido a Odontoweb");
                }
                vSMTP.Enviar_Mail();
            }
            else
            {
                vMensaje = "Bienvenido a Odontoweb.\n Sus datos para ingresar al sistema son: \n" + "Usuario: " + idCliente +
                          "\nContraseña: " + password + "\nSaludos,\nOdontoweb.";
                vSMTP.Datos_Mensaje(email, vMensaje, "Bienvenido a Odontoweb");
                vSMTP.Enviar_Mail();
            }
        }

        #endregion

        #region Metodos Privados

        private static void EnviarReseteoContrasena(string login, string nuevaContrasena)
        {
            var pac = from p in dataContext.Paciente
                      join cp in dataContext.Cliente_Paciente
                      on p.id_paciente equals cp.id_paciente
                      join
                          c in dataContext.UsuarioCliente on
                          cp.id_usuariocliente equals c.Login
                      where c.Login == login
                      select p;
            if (pac.Any())
            {
                SMTP vSMTP = new SMTP();
                var vMensaje = "Estimado Cliente su contrasena ha sido reseteada, por lo tanto su nueva " +
                    "\nconstrasena es: " + nuevaContrasena + ". \nSaludos,\nOdontoweb";
                vSMTP.Datos_Mensaje(pac.First().email, vMensaje, "Reseteo de Constrasena");
                vSMTP.Enviar_Mail();
            }
        }

        #endregion
    }
}