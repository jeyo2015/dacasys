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

        readonly DataContext dataContext = new DataContext();
        readonly ControlBitacora controlBitacora = new ControlBitacora();
        readonly ControlLogErrores controlErrores = new ControlLogErrores();
        readonly Encriptador abmEncriptador = new Encriptador();

        #endregion

        #region ABM_UsuarioCliente

        /// <summary>
        /// Inserta un nuevo UsuarioCliente al sistema
        /// </summary>
        /// <param name="pLogin">Login de ingreso al sistema del UsuarioCliente</param>
        /// <param name="pPassword">Pasword de ingreso al sistema</param>
        /// <param name="pFechaCreacion">Fecha de Creación del UsuarioCliente</param>
        public int Insertar(string pLogin, string pPassword, string pIDUsuario)
        {
            var sql = from us in dataContext.UsuarioCliente
                      where us.Login == pLogin
                      select us;
            if (sql.Count() > 0)
            {
                return 2;
            }
            else
            {
                String vPassEncriptada = abmEncriptador.Encriptar(pPassword);
                UsuarioCliente vUsuarioCliente = new UsuarioCliente();
                vUsuarioCliente.Login = pLogin;
                vUsuarioCliente.Password = vPassEncriptada;
                vUsuarioCliente.FechaCreacion = DateTime.Today;
                vUsuarioCliente.Estado = true;
                vUsuarioCliente.changepass = true;
                try
                {
                    dataContext.UsuarioCliente.InsertOnSubmit(vUsuarioCliente);
                    dataContext.SubmitChanges();
                    controlBitacora.Insertar("Se Inserto un nuevo cliente", pIDUsuario);

                    return 1;
                }
                catch (Exception ex)
                {
                    controlErrores.Insertar("NLogin", "ABMCliente", "ABMUsuarioCliente", ex);
                    return 0;
                }
            }

        }

        /// <summary>
        /// Modifica los datos del UsuarioCliente especificado por su ID
        /// </summary>
        /// <param name="pLogin">Login del UsuarioCliente a modificar</param>
        /// <param name="pPassword">Password del UsuarioCliente a modificar</param>
        public void Modificar(string pLogin, string pPassword)
        {
            var sql = from e in dataContext.UsuarioCliente
                      where e.Login == pLogin
                      select e;

            if (sql.Count() > 0)
            {
                String vPassEncriptada = abmEncriptador.Encriptar(pPassword);
                sql.First().Password = vPassEncriptada;
                dataContext.SubmitChanges();
            }
        }

        /// <summary>
        /// Pone a un UsuarioCliente como Estado false, o sea deshabilitado
        /// </summary>
        /// <param name="pID">Es el ID del UsuarioCliente a eliminar</param>
        public void Eliminar(string pLogin)
        {
            var sql = from e in dataContext.UsuarioCliente
                      where e.Login == pLogin
                      select e;

            if (sql.Count() > 0)
            {
                sql.First().Estado = false;
                dataContext.SubmitChanges();
            }
        }

        #endregion

        public SessionDto verificar_cliente(String pUsuario, String pass)
        {
            var sessionReturn = new SessionDto();
            var cliente = from cli in dataContext.UsuarioCliente
                          where cli.Login == pUsuario
                          select cli;
            if (cliente.Count() > 0)
            {

                String vPassEncriptada = abmEncriptador.Encriptar(pass);
                if (cliente.First().Password == vPassEncriptada)
                {

                    sessionReturn.loginUsuario = pUsuario;
                    sessionReturn.IDConsultorio = -1;
                    //sessionReturn.Nombre = cliente.First().
                    sessionReturn.ChangePass = cliente.First().changepass ?? false;
                    sessionReturn.IDClinica = -1;
                    sessionReturn.IsDacasys = false;
                    sessionReturn.Verificar = 3;
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
            return sessionReturn;
        }

        public DataTable Get_Clientep(string pLogin)
        {
            return Converter<UsuarioCliente>.Convert(Get_Gliente(pLogin).ToList());
        }

        private IEnumerable<UsuarioCliente> Get_Gliente(string pLogin)
        {
            return from c in dataContext.UsuarioCliente
                   where c.Login == pLogin
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
        public int Cambiar_pass(string pLogin, string pPass)
        {

            var user = from u in dataContext.UsuarioCliente
                       where u.Login == pLogin
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
                    controlErrores.Insertar("NLogin", "ABMUsuarioCliente", "Cambiar_Pass", ex);
                    return 0;
                }
            }
            return 0;
        }

        public int ResetearPass(string pLogin)
        {
            var user = (from u in dataContext.UsuarioCliente
                        where u.Login == pLogin
                        select u).FirstOrDefault();
            if (user != null)
            {
                String vPassNew = abmEncriptador.Generar_Aleatoriamente();
                String vPassNewEncriptada = abmEncriptador.Encriptar(vPassNew);
                user.changepass = true;
                user.Password = vPassNewEncriptada;
                try
                {
                    dataContext.SubmitChanges();
                    controlBitacora.Insertar("Se reseteo password de Usuario " + pLogin, "0000");
                    Enviar_ReseteoDePass(pLogin, vPassNew);
                    return 3;
                }
                catch (Exception ex)
                {
                    controlErrores.Insertar("NLogin", "ABMUsuarioCliente", "ResetearPass", ex);
                    return 2;
                }
            }
            else
            {
                return 1;
            }
        }

        private void Enviar_ReseteoDePass(string pLogin, string pNewPass)
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
                    "\nconstrasena es: " + pNewPass + ". \nSaludos,\nMediweb";
                vSMTP.Datos_Mensaje(pac.First().email, vMensaje, "Reseteo de Constrasena");
                vSMTP.Enviar_Mail();
            }
        }

        public void Enviar_Bienvenida(int pIDClinica, string pemail, string vPass, string pcodigo_cliente)
        {
            var em = from e in dataContext.Clinica
                     where e.ID == pIDClinica && e.Estado == true
                     select e;
            if (em.Count() > 0)
            {
                SMTP vSMTP = new SMTP();
                String vMensaje = "";
                if (vPass.Equals(""))
                {//Solo se asigna nueva empresa
                    vMensaje = "Estimado Cliente ha sido suscrito a " + em.First().Nombre.ToUpper() + ". \nIngrese a la pagina " +
                        "para poder informarse acerca de este consultorio. Sus datos de acceso son los mismos." +
                        "\nSaludos,\nMediweb";
                    vSMTP.Datos_Mensaje(pemail, vMensaje, "Nuevo Consultorio");
                }
                else
                {
                    vMensaje = "Bienvenido a Odontoweb.\n Ha sido suscrito a " + em.First().Nombre.ToUpper() +
                       " sus datos para ingresar al sistema son: \n" + "Usuario: " + pcodigo_cliente +
                       "\nContraseña: " + vPass + "\nSaludos,\nOdontoweb.";
                    vSMTP.Datos_Mensaje(pemail, vMensaje, "Bienvenido a Odontoweb");
                }
                vSMTP.Enviar_Mail();
            }
        }
    }
}