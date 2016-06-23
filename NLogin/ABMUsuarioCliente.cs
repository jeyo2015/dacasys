namespace NLogin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Datos;
    using NEventos;
    using System.Data;
    using DataTableConverter;
    using Herramientas;

    public class ABMUsuarioCliente
    {
        #region VariablesGlogales

        readonly static DataContext dataContext = new DataContext();

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
        /// <param name="pLogin">Login del UsuarioCliente a modificar</param>
        /// <param name="pPassword">Password del UsuarioCliente a modificar</param>
        public static void Modificar(string pLogin, string pPassword)
        {
            var sql = from e in dataContext.UsuarioCliente
                      where e.Login == pLogin
                      select e;

            if (sql.Count() > 0)
            {
                String vPassEncriptada = Encriptador.Encriptar(pPassword);
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

        public static SessionDto verificar_cliente(String pUsuario, String pass)
        {
            var sessionReturn = new SessionDto();
            var cliente = (from cli in dataContext.UsuarioCliente
                          where cli.Login == pUsuario
                          select cli).FirstOrDefault();
            if (cliente!= null)
            {
                var paciente = (from p in dataContext.Paciente
                                from pc in dataContext.Cliente_Paciente
                                where p.id_paciente == pc.id_paciente
                                && pc.id_usuariocliente == cliente.Login
                                && pc.IsPrincipal
                                select p).FirstOrDefault();
                String vPassEncriptada = Encriptador.Encriptar(pass);
                if (cliente.Password == vPassEncriptada)
                {
                    sessionReturn.loginUsuario = pUsuario;
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

        public static IEnumerable<UsuarioCliente> Get_Clientep(string loginCliente)
        {
            return from c in dataContext.UsuarioCliente
                   where c.Login == loginCliente
                   select c;
        }
        
        /// <summary>
        /// Permite Cambiar el Pass de un Cliente
        /// </summary>
        /// <param name="pLogin">Login del Cliente</param>
        /// <param name="pPass">Nueva Password</param>
        /// <returns>0 - Todo ok
        /// -3 - Error pass
        /// -5 - No existe Cliente
        /// -6 - No se Pudo Modificar Pass</returns>
        public static int Cambiar_pass(string pLogin, string pPass)
        {

            var user = from u in dataContext.UsuarioCliente
                       where u.Login == pLogin
                       select u;
            if (user.Count() > 0)
            {
                String vPassEncriptada = Encriptador.Encriptar(pPass);
                user.First().changepass = false;
                user.First().Password = vPassEncriptada;
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se modifico un Usuario", pLogin);
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

        public static int ResetearPass(string pLogin)
        {
            var user = (from u in dataContext.UsuarioCliente
                        where u.Login == pLogin
                        select u).FirstOrDefault();
            if (user != null)
            {
                String vPassNew = Encriptador.Generar_Aleatoriamente();
                String vPassNewEncriptada = Encriptador.Encriptar(vPassNew);
                user.changepass = true;
                user.Password = vPassNewEncriptada;
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se reseteo password de Usuario " + pLogin, "0000");
                    Enviar_ReseteoDePass(pLogin, vPassNew);
                    return 3;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NLogin", "ABMUsuarioCliente", "ResetearPass", ex);
                    return 2;
                }
            }
            else
            {
                return 1;
            }
        }

        public static void EnviarCorreoDeBienvenida(int pIDEmpresa, string pemail, string vPass, string pcodigo_cliente)
        {
            var em = (from e in dataContext.Empresa
                      from c in dataContext.Clinica
                      where e.ID == pIDEmpresa && e.Estado == true
                      && c.ID == e.IDClinica
                      select c).FirstOrDefault();
            if (em != null)
            {
                SMTP vSMTP = new SMTP();
                String vMensaje = "";
                if (vPass.Equals(""))
                {//Solo se asigna nueva empresa
                    vMensaje = "Estimado Cliente ha sido suscrito a " + em.Nombre.ToUpper() + ". \nIngrese a la pagina " +
                        "para poder informarse acerca de este consultorio. Sus datos de acceso son los mismos." +
                        "\nSaludos,\nOdontoweb";
                    vSMTP.Datos_Mensaje(pemail, vMensaje, "Nuevo Consultorio");
                }
                else
                {
                    vMensaje = "Bienvenido a Odontoweb.\n Ha sido suscrito a " + em.Nombre.ToUpper() +
                       " sus datos para ingresar al sistema son: \n" + "Usuario: " + pcodigo_cliente +
                       "\nContraseña: " + vPass + "\nSaludos,\nOdontoweb.";
                    vSMTP.Datos_Mensaje(pemail, vMensaje, "Bienvenido a Odontoweb");
                }
                vSMTP.Enviar_Mail();
            }
        }

        #endregion

        #region Metodos Privados

        private static void Enviar_ReseteoDePass(string pLogin, string pNewPass)
        {
            var pac = from p in dataContext.Paciente
                      join cp in dataContext.Cliente_Paciente
                      on p.id_paciente equals cp.id_paciente
                      join
                          c in dataContext.UsuarioCliente on
                          cp.id_usuariocliente equals c.Login
                      where c.Login == pLogin
                      select p;
            if (pac.Count() > 0)
            {
                SMTP vSMTP = new SMTP();
                String vMensaje = "Estimado Cliente su contrasena ha sido reseteada, por lo tanto su nueva " +
                    "\nconstrasena es: " + pNewPass + ". \nSaludos,\nOdontoweb";
                vSMTP.Datos_Mensaje(pac.First().email, vMensaje, "Reseteo de Constrasena");
                vSMTP.Enviar_Mail();
            }
        }

        #endregion
    }
}