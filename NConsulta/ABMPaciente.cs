namespace NConsulta
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Datos;
    using NEventos;
    using DataTableConverter;
    using System.Data;
    using Herramientas;
    using NLogin;

    public class ABMPaciente
    {
        #region VariableGlobales

        readonly static DataContext dataContext = new DataContext();

        #endregion

        #region Metodos Publicos

        public static List<PacienteDto> ObtenerClientesPorEmpresa(int idConsultorio)
        {
            return (from clientePaciente in dataContext.Cliente_Paciente
                    from paciente in dataContext.Paciente
                    from empresaCliente in dataContext.Empresa_Cliente
                    where empresaCliente.id_empresa == idConsultorio
                    && empresaCliente.id_usuariocliente == clientePaciente.id_usuariocliente
                    && clientePaciente.id_paciente == paciente.id_paciente
                    && clientePaciente.IsPrincipal == true
                    && paciente.estado == true
                    select new PacienteDto()
                    {
                        Antecedentes = paciente.antecedente,
                        Ci = paciente.ci,
                        Direccion = paciente.direccion,
                        Email = paciente.email,
                        Estado = paciente.estado,
                        LoginCliente = empresaCliente.id_usuariocliente,
                        NombrePaciente = paciente.nombre + " " + paciente.apellido,
                        Nombre = paciente.nombre,
                        Apellido = paciente.apellido,
                        Telefono = paciente.nro_telefono,
                        TipoSangre = paciente.tipo_sangre,
                        IdPaciente = paciente.id_paciente,
                        Sexo = paciente.sexo.ToString(),
                        IsPrincipal = clientePaciente.IsPrincipal
                    }).ToList();
        }

        /// <summary>
        /// Devuelve el un paciente 
        /// </summary>
        /// <param name="idPaciente">ID del Paciente a obtener </param>
        /// <returns>Retorna el Paciente en un DataTable, vacia caso de que no exista</returns>
        public static List<PacienteDto> ObtenerPacientesPorCliente(string idPaciente)
        {
            return (from paciente in dataContext.Paciente
                    from clientePaciente in dataContext.Cliente_Paciente
                    where clientePaciente.id_usuariocliente == idPaciente
                    && paciente.id_paciente == clientePaciente.id_paciente
                    && paciente.estado == true && clientePaciente.IsPrincipal == false
                    select new PacienteDto()
                    {
                        Antecedentes = paciente.antecedente,
                        Ci = paciente.ci,
                        Direccion = paciente.direccion,
                        Email = paciente.email,
                        Estado = paciente.estado,
                        NombrePaciente = paciente.nombre + " " + paciente.apellido,
                        Nombre = paciente.nombre,
                        Apellido = paciente.apellido,
                        Telefono = paciente.nro_telefono,
                        TipoSangre = paciente.tipo_sangre,
                        IdPaciente = paciente.id_paciente,
                        Sexo = paciente.sexo.ToString(),
                        IsPrincipal = clientePaciente.IsPrincipal
                    }).ToList();

        }
                
        public static PacienteDto ObtenerPacientePorId(string idPaciente)
        {
            return (from paciente in dataContext.Paciente
                    from clientePaciente in dataContext.Cliente_Paciente
                    where clientePaciente.id_usuariocliente == idPaciente
                    && paciente.id_paciente == clientePaciente.id_paciente
                    && paciente.estado == true && clientePaciente.IsPrincipal == true
                    select new PacienteDto()
                    {
                        LoginCliente = clientePaciente.id_usuariocliente,
                        Antecedentes = paciente.antecedente,
                        Ci = paciente.ci,
                        Direccion = paciente.direccion,
                        Email = paciente.email,
                        Estado = paciente.estado,
                        NombrePaciente = paciente.nombre + " " + paciente.apellido,
                        Nombre = paciente.nombre,
                        Apellido = paciente.apellido,
                        Telefono = paciente.nro_telefono,
                        TipoSangre = paciente.tipo_sangre,
                        IdPaciente = paciente.id_paciente,
                        Sexo = paciente.sexo.ToString(),
                        IsPrincipal = clientePaciente.IsPrincipal
                    }).FirstOrDefault();

        }

        /// <summary>
        /// Permite insertar un nuevo paciente, isPaciente
        /// </summary>
        /// <param name="pacienteDto">Dto paciente</param>
        /// <param name="idUsuario">Id del usuario que realiza accion</param>
        /// <returns> 1 - Se inserto 
        ///  0 - No se inserto
        ///  2 - Nombre vacio
        ///  3 - apellido vacio
        ///  4 - numero vacio, o menor de 7 digitos
        ///  5 - direccion vacia
        ///  6 - tipo de sangre vacio o  menos de 4 caracteres
        ///  7 - ci menos de 7 digitos
        /// </returns>
        public static int Insertar(PacienteDto pacienteDto, string idUsuario)
        {
            var v = Validar(pacienteDto);
            if (v != 0)
                return v;
            var vPaciente = new Paciente
            {
                ci = pacienteDto.Ci != null ? pacienteDto.Ci.Trim() : null,
                direccion = pacienteDto.Direccion,
                email = pacienteDto.Email,
                nombre = pacienteDto.Nombre,
                apellido = pacienteDto.Apellido,
                nro_telefono = pacienteDto.Telefono,
                sexo = char.Parse(pacienteDto.Sexo),
                tipo_sangre = pacienteDto.TipoSangre,
                antecedente = pacienteDto.Antecedentes,
                estado = true
            };
            try
            {
                dataContext.Paciente.InsertOnSubmit(vPaciente);
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se Inserto un nuevo Paciente", idUsuario);
                int nuevoIdPaciente = ObtenerIdPaciente();

                if (pacienteDto.IsPrincipal)
                {
                    var password = Encriptador.Generar_Aleatoriamente();
                    ABMUsuarioCliente.Insertar(pacienteDto.LoginCliente, password, idUsuario);
                    AsignarEmpresaCliente(pacienteDto.IDEmpresa, pacienteDto.LoginCliente, "", idUsuario);
                    ABMUsuarioCliente.EnviarCorreoDeBienvenida(pacienteDto.IDEmpresa, pacienteDto.Email, password, pacienteDto.LoginCliente);
                }
                AsignarClientePaciente(nuevoIdPaciente, pacienteDto.LoginCliente, pacienteDto.IsPrincipal, idUsuario);
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NConsulta", "ABMPAciente", "Insertar", ex);
                return 0;
            }
        }

        /// <summary>
        /// Permite modificar los datos de un paciente
        /// </summary>
        /// <param name="pacienteDto">Dto del paciente a modificar</param>
        /// <param name="pIDUsuario">ID del usuario que modifica</param>
        /// <returns>1 - Se Guardo correctamente
        ///         0 - No se pudo modificar</returns>
        public static int Modificar(PacienteDto pacienteDto, string pIDUsuario)
        {
            var v = Validar(pacienteDto);
            if (v != 0)
                return v;

            var sql = from e in dataContext.Paciente
                      where e.id_paciente == pacienteDto.IdPaciente
                      select e;
            if (!sql.Any()) return 0;
            sql.First().nombre = pacienteDto.Nombre;
            sql.First().apellido = pacienteDto.Apellido;
            sql.First().ci = pacienteDto.Ci;
            sql.First().nro_telefono = pacienteDto.Telefono;
            sql.First().direccion = pacienteDto.Direccion;
            sql.First().email = pacienteDto.Email;
            sql.First().tipo_sangre = pacienteDto.TipoSangre;
            sql.First().sexo = char.Parse(pacienteDto.Sexo);
            sql.First().antecedente = pacienteDto.Antecedentes;
            try
            {
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se Modifico un paciente " + pacienteDto.IdPaciente, pIDUsuario);

                if (pacienteDto.IsPrincipal)
                {
                    if(pacienteDto.Password != null)
                    {
                        ABMUsuarioCliente.Modificar(pacienteDto.LoginCliente, pacienteDto.Password);
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NConsulta", "ABMPaciente", "Modificar Paciente", ex);
                return 0;
            }
        }

        public static bool Eliminar(int idPaciente, bool isPrincipal, string idUsuario)
        {
            var sql = from c in dataContext.Paciente
                      where c.id_paciente == idPaciente
                      select c;

            if (sql.Any())
            {
                var query = from c in dataContext.Cliente_Paciente
                            where c.id_paciente == idPaciente
                            select c;

                if (isPrincipal)
                {
                    if (query.Any())
                    {
                        ABMUsuarioCliente.Eliminar(query.First().id_usuariocliente);
                        EliminarEmpresaCliente(query.First().id_usuariocliente);
                    }
                }

                EliminarClientePaciente(query.First().id_usuariocliente, idUsuario);
                sql.First().estado = false;
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se elimino el paciente", idUsuario);
                    return true;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NConsulta", "ABMPaciente", "Eliminar", ex);
                }
            }
            else
            {
                ControlLogErrores.Insertar("NConsulta", "ABMPaciente", "Eliminar, no se pudo obtener el paciente", null);
            }
            return false;
        }

        #endregion

        #region Metodos Privados

        private static int ObtenerIdPaciente()
        {
            var paciente = (from c in dataContext.Paciente
                            select c);
            return paciente == null ? 1 : paciente.Max(x => x.id_paciente);
        }

        /// <summary>
        /// Asigna un paciente a un isPaciente
        /// </summary>
        /// <param name="idPaciente">Id del paciente</param>
        /// <param name="loginCliente">Codigo del Cliente</param>
        ///  <param name="loginCliente">ID del usuario que realiza accion</param>
        /// <returns>0 - No inserto
        /// 1 - Inserto correctamente</returns>
        private static int AsignarClientePaciente(int idPaciente, string loginCliente, bool isPaciente, string idUsuario)
        {
            Cliente_Paciente clientePaciente = new Cliente_Paciente();
            clientePaciente.id_paciente = idPaciente;
            clientePaciente.id_usuariocliente = loginCliente;
            clientePaciente.IsPrincipal = isPaciente;
            try
            {
                dataContext.Cliente_Paciente.InsertOnSubmit(clientePaciente);
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se Asigno un paciente a un isPaciente", idUsuario);
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NConsulta", "ABMPaciente", "Asignar_Paciente", ex);
                return 0;
            }
        }

        /// <summary>
        /// Permite  asignar un isPaciente a una empresa ya exitente
        /// </summary>
        /// <param name="idEmpresa">ID de la empresa a la que se le asiganara el isPaciente</param>
        /// <param name="loginCliente">Id del Cliente que se asignara</param>
        private static int AsignarEmpresaCliente(int idEmpresa, string loginCliente, string email, string idUsuario)
        {
            Empresa_Cliente empresaCliente = new Empresa_Cliente();
            empresaCliente.id_empresa = idEmpresa;
            empresaCliente.id_usuariocliente = loginCliente;
            try
            {
                dataContext.Empresa_Cliente.InsertOnSubmit(empresaCliente);
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se Inserto un nuevo Empresa_Cliente", idUsuario);
                if (!email.Equals(""))//Si no se ha dado bienvenida
                    ABMUsuarioCliente.EnviarCorreoDeBienvenida(idEmpresa, email, "", loginCliente);
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NConsulta", "ABMPaciente", "Asiganar_Empresa", ex);
                return 0;
            }
        }

        /// <summary>
        /// Permite Convertir a un paciente en isPaciente, desconectandolo de su Cliente Padre
        /// </summary>
        /// <param name="loginCliente">Login del nuevo Cliente</param>
        /// <param name="idEmpresa">Id de la empresa</param>
        /// <param name="idUsuario">Id del Usuario que realiza la accion</param>
        /// <returns>0 - No se pudo crear nuevo Cliente
        /// 1 - Se creo exitosamente
        /// 2 - No se pudo eliminar</returns>
        private int AsignarUsuarioCliente(string loginCliente, int idEmpresa, string email, string idUsuario)
        {
            var password = Encriptador.Generar_Aleatoriamente();
            var idInsetar = ABMUsuarioCliente.Insertar(loginCliente, password, idUsuario);

            if (idInsetar == 1)
            {
                idInsetar = AsignarEmpresaCliente(idEmpresa, loginCliente, "", idUsuario);
                if (idInsetar == 1)
                {
                    idInsetar = EliminarClientePaciente(loginCliente, idUsuario);
                    ABMUsuarioCliente.EnviarCorreoDeBienvenida(idEmpresa, email, password, loginCliente);
                }
                else
                    return 0;
            }
            else
            {
                return 0;
            }
            return idInsetar;
        }

        private static int Validar(PacienteDto pacienteDto)
        {
            if (pacienteDto.Nombre.Length <= 1)
            {
                return 2;
            }
            if (pacienteDto.Apellido.Length <= 1)
            {
                return 3;
            }
            if (pacienteDto.Ci == null && pacienteDto.IsPrincipal) return 7;
            if (pacienteDto.Ci != null && pacienteDto.Ci.Length < 7) return 7;

            if (pacienteDto.IsPrincipal && pacienteDto.IdPaciente == 0)
            {
                var sql = from us in dataContext.UsuarioCliente where us.Login == pacienteDto.LoginCliente select us;
                if (sql.Any() && pacienteDto.IsPrincipal)
                    return 8;
            }

            if (pacienteDto.Email.Length < 3)
            {
                return 4;
            }
            else
            {
                if (!pacienteDto.Email.Contains('@'))
                    return 4;
            }
            return 0;
        }

        /// <summary>
        /// Elimina de manera permanente la asociacion con otros clientes
        /// </summary>
        /// <param name="loginCliente">Ci de identidad</param>
        /// <param name="idUsuario">ID usuario que realiza accion</param>
        /// <returns>0 - no exite paciente
        ///         1 - se elimino exitosamente
        ///         2 - Error al querer eliminar cliente_paciente</returns>
        private static int EliminarClientePaciente(string loginCliente, string idUsuario)
        {
            var sql = from pac in dataContext.Paciente
                      where pac.ci == loginCliente
                      select pac;
            if (sql.Any())
            {
                var cl_pac = from c_p in dataContext.Cliente_Paciente
                             where c_p.id_paciente == sql.First().id_paciente
                             select c_p;
                foreach (var cp in cl_pac)
                {
                    dataContext.Cliente_Paciente.DeleteOnSubmit(cp);
                }

                try
                {
                    dataContext.SubmitChanges();
                    return 1;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NConsulta", "ABMPAciente", "Eliminar_Paciente", ex);
                    return 0;
                }
            }
            else
                return 2;
        }

        /// <summary>
        /// Asigna a un isPaciente a un empresa. Genera su pass y envia correo de bienvenida
        /// </summary>
        /// <param name="loginCliente">Codigo isPaciente</param>
        /// <param name="idEmpresa">ID empresa</param>
        /// <param name="email">Email del isPaciente</param>
        /// <param name="idUsuario">ID Usuario</param>
        /// <param name="idPaciente">ID paciente, para sacar sus datos personales</param>
        /// <returns></returns>
        public int AsignarCliente_Empresa(string loginCliente, int idEmpresa, string email, string idUsuario, int idPaciente)
        {
            String vPass = Encriptador.Generar_Aleatoriamente();
            int vInsert = ABMUsuarioCliente.Insertar(loginCliente, vPass, idUsuario);

            if (vInsert == 1)
            {
                vInsert = AsignarEmpresaCliente(idEmpresa, loginCliente, "", idUsuario);
                if (vInsert == 1)
                {
                    vInsert = Quitar_Cliente_Padre(idPaciente, loginCliente, idUsuario);
                    ABMUsuarioCliente.EnviarCorreoDeBienvenida(idEmpresa, email, vPass, loginCliente);
                }
                else
                    return 0;
            }
            else
            {
                return 0;
            }
            return vInsert;
        }

        /// <summary>
        /// Metodo utilizado cuando un paciente ya dependerá de otro
        /// </summary>
        /// <param name="pIDPaciente">ID del paciente </param>
        /// <param name="pCodCliente">codigo isPaciente a desligar</param>
        /// <param name="pIDUsuario">ID del usuario que realiza accion</param>
        /// <returns></returns>
        private int Quitar_Cliente_Padre(int pIDPaciente, string pCodCliente, string pIDUsuario)
        {
            var cl_pac = from c_p in dataContext.Cliente_Paciente
                         where c_p.id_paciente == pIDPaciente
                         select c_p;
            foreach (var cp in cl_pac)
            {
                try
                {
                    dataContext.Cliente_Paciente.DeleteOnSubmit(cp);
                    ControlBitacora.Insertar("Se elimino un Cliente_Paciente", pIDUsuario);
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NConsulta", "ABMPAciente", "Eliminar_Paciente", ex);
                    return 0;
                }
            }
            try
            {
                dataContext.SubmitChanges();
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NConsulta", "ABMPAciente", "Eliminar_Paciente", ex);
                return 0;
            }

        }

        public static void EliminarEmpresaCliente(string loginCliente)
        {
            var sql = from e in dataContext.Empresa_Cliente
                      where e.id_usuariocliente == loginCliente
                      select e;

            if (!sql.Any()) return;
            dataContext.Empresa_Cliente.DeleteOnSubmit(sql.First());
        }

        #endregion
    }
}