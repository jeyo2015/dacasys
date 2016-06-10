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
        
        public static List<PacienteDto> GetPacientesEmpresa(int idConsultorio)
        {
            return (from uc in dataContext.Cliente_Paciente
                    from p in dataContext.Paciente
                    from cc in dataContext.Empresa_Cliente
                    where cc.id_empresa == idConsultorio
                    && cc.id_usuariocliente == uc.id_usuariocliente
                    && uc.id_paciente == p.id_paciente
                    && uc.IsPrincipal == true
                    select new PacienteDto()
                    {
                        Antecedentes = p.antecedente,
                        Ci = p.ci,
                        Direccion = p.direccion,
                        Email = p.email,
                        Estado = p.estado,
                        LoginCliente = cc.id_usuariocliente,
                        NombrePaciente = p.nombre + " " + p.apellido,
                        Telefono = p.nro_telefono,
                        TipoSangre = p.tipo_sangre,
                        IdPaciente = p.id_paciente
                    }).ToList();
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
                ci = pacienteDto.Ci.Trim(),
                direccion = pacienteDto.Direccion,
                email = pacienteDto.Email,
                nombre = pacienteDto.Nombre,
                apellido = pacienteDto.Apellido,
                nro_telefono = pacienteDto.Telefono,
                sexo = Char.Parse(pacienteDto.Sexo),
                tipo_sangre = pacienteDto.TipoSangre,
                antecedente = pacienteDto.Antecedentes,
                estado = true
            };
            try
            {
                dataContext.Paciente.InsertOnSubmit(vPaciente);
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se Inserto un nuevo Paciente", idUsuario);
                if (!pacienteDto.IsPaciente)
                {
                    var password = Encriptador.Generar_Aleatoriamente();
                    ABMUsuarioCliente.Insertar(pacienteDto.LoginCliente, password, idUsuario);
                    Asignar_Empresa(pacienteDto.IDEmpresa, pacienteDto.Ci.Trim(), "", idUsuario);
                    ABMUsuarioCliente.Enviar_Bienvenida(pacienteDto.IDEmpresa, pacienteDto.Email, password, pacienteDto.LoginCliente);
                }
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
            sql.First().sexo = Char.Parse(pacienteDto.Sexo);
            sql.First().antecedente = pacienteDto.Antecedentes;
            try
            {
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se Modifico un paciente " + pacienteDto.IdPaciente, pIDUsuario);
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NConsulta", "ABMPaciente", "Modificar Paciente", ex);
                return 0;
            }
        }

        public static bool Eliminar(int idPaciente, bool isPaciente, string idUsuario)
        {
            var sql = from c in dataContext.Paciente
                      where c.id_paciente == idPaciente
                      select c;

            if (sql.Any())
            {
                if (!isPaciente)
                {
                    ABMUsuarioCliente.Eliminar(sql.First().ci);
                }

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

        /// <summary>
        /// Permite Convertir a un paciente en isPaciente, desconectandolo de su Cliente Padre
        /// </summary>
        /// <param name="pCodCliente">Login del nuevo Cliente</param>
        /// <param name="pIDEmpresa">Id de la empresa</param>
        /// <param name="pIDUsuario">Id del Usuario que realiza la accion</param>
        /// <returns>0 - No se pudo crear nuevo Cliente
        /// 1 - Se creo exitosamente
        /// 2 - No se pudo eliminar</returns>
        public int AsignarCliente_Empresa(string pCodCliente, int pIDEmpresa, string pemail, string pIDUsuario)
        {
            var password = Encriptador.Generar_Aleatoriamente();
            var idInsetar = ABMUsuarioCliente.Insertar(pCodCliente, password, pIDUsuario);

            if (idInsetar == 1)
            {
                idInsetar = Asignar_Empresa(pIDEmpresa, pCodCliente, "", pIDUsuario);
                if (idInsetar == 1)
                {
                    idInsetar = Eliminar_Paciente(pCodCliente, pIDUsuario);
                    ABMUsuarioCliente.Enviar_Bienvenida(pIDEmpresa, pemail, password, pCodCliente);
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

        /// <summary>
        /// Asigna un paciente a un isPaciente existente
        /// </summary>
        /// <param name="pPaciente">Paciente que se desea asignar</param>
        /// <param name="pIDCliente">ID del Cliente al que se asignara el paciente</param>
        public void Asignar_Cliente(Paciente pPaciente, string pIDCliente)
        {
            var sql = from c in dataContext.Paciente
                      where c.ci == pPaciente.ci && c.nombre == pPaciente.nombre &&
                      c.nro_telefono == pPaciente.nro_telefono && c.sexo == pPaciente.sexo
                      && c.tipo_sangre == pPaciente.tipo_sangre && c.email == pPaciente.email
                      select c;
            var vid_pacient = -1;
            if (sql.Any())
            {
                vid_pacient = sql.First().id_paciente;
            }
            Cliente_Paciente vCli_Pac = new Cliente_Paciente();
            vCli_Pac.id_paciente = vid_pacient;
            vCli_Pac.id_usuariocliente = pIDCliente;
            dataContext.Cliente_Paciente.InsertOnSubmit(vCli_Pac);
            dataContext.SubmitChanges();
        }

        /// <summary>
        /// Asigna un paciente a un isPaciente
        /// </summary>
        /// <param name="pIDPaciente">Id del paciente</param>
        /// <param name="pCodCliente">Codigo del Cliente</param>
        ///  <param name="pCodCliente">ID del usuario que realiza accion</param>
        /// <returns>0 - No inserto
        /// 1 - Inserto correctamente</returns>
        public int Asignar_Paciente(int pIDPaciente, string pCodCliente, string pIDUsuario)
        {
            Cliente_Paciente cp = new Cliente_Paciente();
            cp.id_paciente = pIDPaciente;
            cp.id_usuariocliente = pCodCliente;
            try
            {
                dataContext.Cliente_Paciente.InsertOnSubmit(cp);
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se Asigno un paciente a un isPaciente", pIDUsuario);
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
        /// <param name="pIDempresa">ID de la empresa a la que se le asiganara el isPaciente</param>
        /// <param name="pIDCliente">Id del Cliente que se asignara</param>
        public static int Asignar_Empresa(int pIDempresa, string pIDCliente, string pemail, string pIDUsuario)
        {
            Empresa_Cliente vEmp_Clie = new Empresa_Cliente();
            vEmp_Clie.id_empresa = pIDempresa;
            vEmp_Clie.id_usuariocliente = pIDCliente;
            try
            {
                dataContext.Empresa_Cliente.InsertOnSubmit(vEmp_Clie);
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se Inserto un nuevo Empresa_Cliente", pIDUsuario);
                if (!pemail.Equals(""))//Si no se ha dado bienvenida
                    ABMUsuarioCliente.Enviar_Bienvenida(pIDempresa, pemail, "", pIDCliente);
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NConsulta", "ABMPaciente", "Asiganar_Empresa", ex);
                return 0;
            }

        }

        /// <summary>
        /// Devuelve el un paciente 
        /// </summary>
        /// <param name="idPaciente">ID del Paciente a obtener </param>
        /// <returns>Retorna el Paciente en un DataTable, vacia caso de que no exista</returns>
        public static List<PacienteDto> GetPacientesByCliente(string idPaciente)
        {
            return (from paciente in dataContext.Paciente
                    from paciente_cliente in dataContext.Cliente_Paciente
                    where paciente_cliente.id_usuariocliente == idPaciente
                    && paciente.id_paciente == paciente_cliente.id_paciente
                    select new PacienteDto()
                    {
                        Email = paciente.email,
                        Estado = paciente.estado,
                        LoginCliente = idPaciente,
                        NombrePaciente = paciente.nombre + " " + paciente.apellido,
                        Telefono = paciente.nro_telefono,
                        TipoSangre = paciente.tipo_sangre,
                        Direccion = paciente.direccion,
                        Ci = paciente.ci,
                        Antecedentes = paciente.antecedente,
                        IdPaciente = paciente.id_paciente
                    }).ToList();

        }
        
        /// <summary>
        /// Valida que el CI sea valido, no se repita y que email este correcto :"@,.com"
        /// </summary>
        /// <param name="pCI">CI Carnet de Identidad</param>
        /// <param name="pEmail">Email del paciente</param>
        /// <returns> -1 - PCI corto
        /// -2 repetido
        /// -3 email incorrecto
        ///         1 - todo ok</returns>
        public int ValidoDatos(string pCI, string pEmail)
        {
            if (pCI.Length < 7)
                return -1;
            else
            {
                var sql = from user in dataContext.UsuarioCliente
                          where user.Login == pCI
                          select user;

                if (sql.Any())
                    return -2;
            }
            if (pEmail.Length < 5 && !pEmail.Contains('@') && !pEmail.Contains(".com"))
                return -3;
            return 1;
        }

        #endregion

        #region Metodos Privados
        
        private IEnumerable<Paciente> Get_PacienteCI(String pCi)
        {
            return from paciente in dataContext.Paciente
                   where paciente.ci == pCi
                   select paciente;
        }

        private static int Validar(PacienteDto pacienteDto)
        {
            if (pacienteDto.NombrePaciente.Length <= 1)
            {
                return 2;
            }
            if (pacienteDto.Apellido.Length <= 1)
            {
                return 3;
            }
            if (pacienteDto.Ci.Length <= 0) return 0;
            if (pacienteDto.Ci.Length < 7)
                return 7;
            else
            {
                var sql = from us in dataContext.UsuarioCliente where us.Login == pacienteDto.Ci select us;
                if (sql.Any())
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
        /// <param name="pCodCliente">Ci de identidad</param>
        /// <param name="pIDUsuario">ID usuario que realiza accion</param>
        /// <returns>0 - no exite paciente
        ///         1 - se elimino exitosamente
        ///         2 - Error al querer eliminar cliente_paciente</returns>
        private int Eliminar_Paciente(string pCodCliente, string pIDUsuario)
        {
            var sql = from pac in dataContext.Paciente
                      where pac.ci == pCodCliente
                      select pac;
            if (sql.Any())
            {
                var cl_pac = from c_p in dataContext.Cliente_Paciente
                             where c_p.id_paciente == sql.First().id_paciente
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
                Asignar_Paciente(sql.First().id_paciente, pCodCliente, pIDUsuario);
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
        /// <param name="pCodCliente">Codigo isPaciente</param>
        /// <param name="pIDEmpresa">ID empresa</param>
        /// <param name="pemail">Email del isPaciente</param>
        /// <param name="pIDUsuario">ID Usuario</param>
        /// <param name="pIDPaciente">ID paciente, para sacar sus datos personales</param>
        /// <returns></returns>
        public int AsignarCliente_Empresa(string pCodCliente, int pIDEmpresa, string pemail, string pIDUsuario, int pIDPaciente)
        {
            String vPass = Encriptador.Generar_Aleatoriamente();
            int vInsert = ABMUsuarioCliente.Insertar(pCodCliente, vPass, pIDUsuario);

            if (vInsert == 1)
            {
                vInsert = Asignar_Empresa(pIDEmpresa, pCodCliente, "", pIDUsuario);
                if (vInsert == 1)
                {
                    vInsert = Quitar_Cliente_Padre(pIDPaciente, pCodCliente, pIDUsuario);
                    ABMUsuarioCliente.Enviar_Bienvenida(pIDEmpresa, pemail, vPass, pCodCliente);
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
            Asignar_Paciente(pIDPaciente, pCodCliente, pIDUsuario);
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

        #endregion
    }
}