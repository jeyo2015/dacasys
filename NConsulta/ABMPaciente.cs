﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DConsulta;
using NEventos;
using DataTableConverter;
using System.Data;
using Herramientas;
using NLogin;

namespace NConsulta
{
    public class ABMPaciente
    {
        #region VariableGlobales
        DConsultaDataContext gDc = new DConsultaDataContext();
        ABMUsuarioCliente gADMCliente = new ABMUsuarioCliente();
        ControlBitacora gCb = new ControlBitacora();
        ControlLogErrores gLe = new ControlLogErrores();

        #endregion

        #region ABM_Paciente

        /// <summary>
        /// Permite insertar un nuevo paciente, cliente
        /// </summary>
        /// <param name="pnombre">Nombre del paciente</param>
        /// <param name="papellido">Apellido del paciente</param>
        /// <param name="pci">CI del paciente</param>
        /// <param name="pnumero_telefono">Numero de telefono del paciente</param>
        /// <param name="pdireccion">Dirreccion del paciente</param>
        /// <param name="pemail">Email del paciente</param>
        /// <param name="ptipo_sangre">Tipo de Sangre del paciente</param>
        /// <param name="psexo">Sexo del paciente</param>
        /// <param name="pcodigo_cliente">Codigo del Cliente</param>
        /// <param name="pIDempresa">ID de la Empresa</param>
        /// <param name="cliente">True es cliente, false solo es paciente</param>
        /// <param name="pIDUsuario">Id del usuario que realiza accion</param>
        /// <returns> 1 - Se inserto 
        ///  0 - No se inserto
        ///  2 - Nombre vacio
        ///  3 - apellido vacio
        ///  4 - numero vacio, o menor de 7 digitos
        ///  5 - direccion vacia
        ///  6 - tipo de sangre vacio o  menos de 4 caracteres
        ///  7 - ci menos de 7 digitos
        /// </returns>
        public int Insertar(string pnombre, string papellido, string pci, string pnumero_telefono,
                            string pdireccion, string pemail, string ptipo_sangre, char psexo,
                            string pcodigo_cliente, int pIDempresa, bool cliente,string pantecedente, string pIDUsuario)
        {
            int v = Validar(pnombre, papellido, pci, pnumero_telefono, pdireccion,
                        pemail, ptipo_sangre, psexo, pcodigo_cliente);
            if (v != 0)
                return v;
            Paciente vPaciente = new Paciente();
            vPaciente.ci = pci.Trim();
            vPaciente.direccion = pdireccion;
            vPaciente.email = pemail;
            vPaciente.nombre = pnombre;
            vPaciente.apellido = papellido;
            vPaciente.nro_telefono = pnumero_telefono;
            vPaciente.sexo = psexo;
            vPaciente.tipo_sangre = ptipo_sangre;
            vPaciente.antecedente = pantecedente;
            vPaciente.estado = true;
            try
            {
                gDc.Paciente.InsertOnSubmit(vPaciente);
                gDc.SubmitChanges();
                gCb.Insertar("Se Inserto un nuevo Paciente", pIDUsuario);
                if (cliente)
                {
                    String vPass = new Encriptador().Generar_Aleatoriamente();
                    gADMCliente.Insertar(pcodigo_cliente, vPass, pIDUsuario);
                    Asignar_Empresa(pIDempresa, pci, "", pIDUsuario);
                    
                    gADMCliente.Enviar_Bienvenida(pIDempresa, pemail, vPass, pcodigo_cliente);


                }
                Asignar_Cliente(vPaciente, pcodigo_cliente);
                return 1;
            }
            catch (Exception ex)
            {
                gLe.Insertar("NConsulta", "ABMPAciente", "Insertar", ex);
                return 0;
            }


        }


        /// <summary>
        /// Permite Convertir a un paciente en cliente, desconectandolo de su Cliente Padre
        /// </summary>
        /// <param name="pCodCliente">Login del nuevo Cliente</param>
        /// <param name="pIDEmpresa">Id de la empresa</param>
        /// <param name="pIDUsuario">Id del Usuario que realiza la accion</param>
        /// <returns>0 - No se pudo crear nuevo Cliente
        /// 1 - Se creo exitosamente
        /// 2 - No se pudo eliminar</returns>
        public int AsignarCliente_Empresa(string pCodCliente, int pIDEmpresa, string pemail, string pIDUsuario)
        {
            String vPass = new Encriptador().Generar_Aleatoriamente();
            int vInsert = gADMCliente.Insertar(pCodCliente, vPass, pIDUsuario);

            if (vInsert == 1)
            {
                vInsert = Asignar_Empresa(pIDEmpresa, pCodCliente, "", pIDUsuario);
                if (vInsert == 1)
                {
                    vInsert = Eliminar_Paciente(pCodCliente, pIDUsuario);
                    gADMCliente.Enviar_Bienvenida(pIDEmpresa, pemail, vPass, pCodCliente);
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
        /// Elimina de manera permanente la asociacion con otros clientes
        /// </summary>
        /// <param name="pCodCliente">Ci de identidad</param>
        /// <param name="pIDUsuario">ID usuario que realiza accion</param>
        /// <returns>0 - no exite paciente
        ///         1 - se elimino exitosamente
        ///         2 - Error al querer eliminar cliente_paciente</returns>
        private int Eliminar_Paciente(string pCodCliente, string pIDUsuario)
        {
            var sql = from pac in gDc.Paciente
                      where pac.ci == pCodCliente
                      select pac;
            if (sql.Count() > 0)
            {

                var cl_pac = from c_p in gDc.Cliente_Paciente
                             where c_p.id_paciente == sql.First().id_paciente
                             select c_p;
                foreach (var cp in cl_pac)
                {
                    try
                    {
                        gDc.Cliente_Paciente.DeleteOnSubmit(cp);
                        gCb.Insertar("Se elimino un Cliente_Paciente", pIDUsuario);
                    }
                    catch (Exception ex)
                    {
                        gLe.Insertar("NConsulta", "ABMPAciente", "Eliminar_Paciente", ex);
                        return 0;
                    }
                }
                Asignar_Paciente(sql.First().id_paciente, pCodCliente, pIDUsuario);
                try
                {
                    gDc.SubmitChanges();
                    return 1;
                }
                catch (Exception ex)
                {
                    gLe.Insertar("NConsulta", "ABMPAciente", "Eliminar_Paciente", ex);
                    return 0;
                }
            }
            else

                return 2;

        }
        private int Validar(string pnombre, string papellido, string pci, string pnumero_telefono, string pdireccion, string pemail, string ptipo_sangre, char psexo, string pcodigo_cliente)
        {
            if (pnombre.Length <= 1)
            {
                return 2;
            }
            if (papellido.Length <= 1)
            {
                return 3;
            }
            if (pci.Length > 0)
            {
                if (pci.Length < 7)
                    return 7;
                else
                {
                    var sql = from us in gDc.UsuarioCliente
                              where us.Login == pci
                              select us;
                    if (sql.Count() > 0)
                        return 8;
                }
                if (pemail.Length < 3)
                {
                    return 4;
                }
                else
                {
                    if (!pemail.Contains('@'))
                        return 4;
                }
            }

            return 0;
        }


        public void Eliminar(int pIDPaciente, bool cliente)
        {
            var sql = from c in gDc.Paciente
                      where c.id_paciente == pIDPaciente
                      select c;

            if (sql.Count() > 0)
            {
                if (cliente)
                {
                    gADMCliente.Eliminar(sql.First().ci);
                }
                sql.First().estado = false;

                gDc.SubmitChanges();

            }


        }

        /// <summary>
        /// Permite modificar los datos de un paciente
        /// </summary>
        /// <param name="pIdPaciente">ID del paciente a modificar</param>
        /// <param name="pnombre">Nombre del paciente</param>
        /// <param name="papellido">Apellido del paciente</param>
        /// <param name="pci">Ci del paciente</param>
        /// <param name="pnumero_telefono">Numero telfonico del paciente</param>
        /// <param name="pdireccion">Direccion del paciente</param>
        /// <param name="pemail">Email del paciente</param>
        /// <param name="ptipo_sangre">Tipo de sangre del paciente</param>
        /// <param name="psexo">Sexo del paciente</param>
        /// <param name="pIDUsuario">ID del usuario que modifica</param>
        /// <returns>1 - Se Guardo correctamente
        ///         0 - No se pudo modificar</returns>
        public int Modificar(int pIdPaciente, string pnombre, string papellido, string pci, string pnumero_telefono,
                            string pdireccion, string pemail, string ptipo_sangre, char psexo, string pantecedente,
                               string pIDUsuario)
        {
            int v = ValidarMod(pnombre, papellido, pci, pnumero_telefono, pdireccion,
                       pemail, ptipo_sangre, psexo);
            if (v != 0)
                return v;

            var sql = from e in gDc.Paciente
                      where e.id_paciente == pIdPaciente
                      select e;
            if (sql.Count() > 0)
            {


                sql.First().nombre = pnombre;
                sql.First().apellido = papellido;
                sql.First().ci = pci;
                sql.First().nro_telefono = pnumero_telefono;
                sql.First().direccion = pdireccion;
                sql.First().email = pemail;
                sql.First().tipo_sangre = ptipo_sangre;
                sql.First().sexo = psexo;
                sql.First().antecedente = pantecedente;
                try
                {
                    gDc.SubmitChanges();
                    gCb.Insertar("Se Modifico un paciente " + pIdPaciente, pIDUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    gLe.Insertar("NConsulta", "ABMPaciente", "Modificar Paciente", ex);
                    return 0;
                }

            }


            return 0;

        }

        private int ValidarMod(string pnombre, string papellido, string pci, string pnumero_telefono, string pdireccion, string pemail, string ptipo_sangre, char psexo)
        {
            if (pnombre.Length <= 1)
            {
                return 2;
            }
            if (papellido.Length <= 1)
            {
                return 3;
            }

            if (pci.Length > 0)
            {
                if (pci.Length < 7)
                    return 7;
                if (pemail.Length < 3)
                {
                    return 4;
                }
                else
                {
                    if (!pemail.Contains('@'))
                        return 4;
                }
            }

            return 0;
        }


        private IEnumerable<BuscarClienteResult> Buscar_Cliente(string pParametro, int pIDEmpresa)
        {
            return gDc.BuscarCliente(pParametro, pIDEmpresa);

        }

        /// <summary>
        /// Busca Clientes segun el parametro, 
        /// 
        /// </summary>
        /// <param name="pNombrePaciente">Parametro codigo, nombre o apellido </param>
        /// <returns>Retorna los clientes que tengan alguna coincidencia con el parametro recibido</returns>
        public DataTable Buscar_Clientep(string pParametro, int pIDEmpresa)
        {

            return Converter<BuscarClienteResult>.Convert(Buscar_Cliente(pParametro, pIDEmpresa).ToList());
        }


        private IEnumerable<getPacientesResult> Get_PacientesClient(string pCodigoCliente, int pIDEmpresa)
        {
            return gDc.getPacientes(pCodigoCliente, pIDEmpresa);

        }

        /// <summary>
        /// Busca Pacientes segun su codigo de cliente
        /// </summary>
        /// <param name="pCodigoCliente">Codigo completo/parcial del Paciente/Cliente </param>
        /// /// <param name="pIDEmpresa">ID de la empresa </param>
        /// <returns>Retorna todos lo clientes que coincidan con el codigo recibido</returns>
        public DataTable Get_PacientespClient(string pCodigoCliente, int pIDEmpresa)
        {

            return Converter<getPacientesResult>.Convert(Get_PacientesClient(pCodigoCliente, pIDEmpresa).ToList());
        }

        /// <summary>
        /// Asigna un paciente a un cliente existente
        /// </summary>
        /// <param name="pPaciente">Paciente que se desea asignar</param>
        /// <param name="pIDCliente">ID del Cliente al que se asignara el paciente</param>
        public void Asignar_Cliente(Paciente pPaciente, string pIDCliente)
        {
            var sql = from c in gDc.Paciente
                      where c.ci == pPaciente.ci && c.nombre == pPaciente.nombre &&
                      c.nro_telefono == pPaciente.nro_telefono && c.sexo == pPaciente.sexo
                      && c.tipo_sangre == pPaciente.tipo_sangre && c.email == pPaciente.email
                      select c;
            int vid_pacient = -1;
            if (sql.Count() > 0)
            {
                vid_pacient = sql.First().id_paciente;
            }
            Cliente_Paciente vCli_Pac = new Cliente_Paciente();
            vCli_Pac.id_paciente = vid_pacient;
            vCli_Pac.id_usuariocliente = pIDCliente;
            gDc.Cliente_Paciente.InsertOnSubmit(vCli_Pac);
            gDc.SubmitChanges();
        }

        /// <summary>
        /// Asigna un paciente a un cliente
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
                gDc.Cliente_Paciente.InsertOnSubmit(cp);
                gDc.SubmitChanges();
                gCb.Insertar("Se Asigno un paciente a un cliente", pIDUsuario);
                return 1;
            }
            catch (Exception ex)
            {
                gLe.Insertar("NConsulta", "ABMPaciente", "Asignar_Paciente", ex);
                return 0;
            }
        }

        /// <summary>
        /// Permite  asignar un cliente a una empresa ya exitente
        /// </summary>
        /// <param name="pIDempresa">ID de la empresa a la que se le asiganara el cliente</param>
        /// <param name="pIDCliente">Id del Cliente que se asignara</param>
        public int Asignar_Empresa(int pIDempresa, string pIDCliente, string pemail, string pIDUsuario)
        {
            Empresa_Cliente vEmp_Clie = new Empresa_Cliente();
            vEmp_Clie.id_empresa = pIDempresa;
            vEmp_Clie.id_usuariocliente = pIDCliente;
            try
            {
                gDc.Empresa_Cliente.InsertOnSubmit(vEmp_Clie);
                gDc.SubmitChanges();
                gCb.Insertar("Se Inserto un nuevo Empresa_Cliente", pIDUsuario);
                if (!pemail.Equals(""))//Si no se ha dado bienvenida
                    gADMCliente.Enviar_Bienvenida(pIDempresa, pemail, "", pIDCliente);
                return 1;
            }
            catch (Exception ex)
            {
                gLe.Insertar("NConsulta", "ABMPaciente", "Asiganar_Empresa", ex);
                return 0;
            }

        }


        private IEnumerable<Paciente> Get_PacientesID(int pIdPaciente)
        {
            return from paciente in gDc.Paciente
                   where paciente.id_paciente == pIdPaciente
                   select paciente;

        }

        /// <summary>
        /// Devuelve el un paciente 
        /// </summary>
        /// <param name="pIdPaciente">ID del Paciente a obtener </param>
        /// <returns>Retorna el Paciente en un DataTable, vacia caso de que no exista</returns>
        public DataTable Get_PacientespID(int pIdPaciente)
        {

            return Converter<Paciente>.Convert(Get_PacientesID(pIdPaciente).ToList());
        }


        private IEnumerable<Paciente> Get_ClientePaciente(String pCodigoCliente)
        {
            return from paciente in gDc.Paciente
                   join cliente in gDc.UsuarioCliente
                   on paciente.ci equals cliente.Login
                   where cliente.Login == pCodigoCliente
                   select paciente;

        }

        /// <summary>
        /// Devuelve los datos de un cliente
        /// </summary>
        /// <param name="pCodigoCliente">Login del Cliente </param>
        /// <returns>Retorna el Paciente en un DataTable, vacia caso de que no exista</returns>
        public DataTable Get_ClientePacientep(String pCodigoCliente)
        {

            return Converter<Paciente>.Convert(Get_ClientePaciente(pCodigoCliente).ToList());
        }

        private IEnumerable<Paciente> Get_ClientePacienteCoin(String pCodigoCliente, int pIDEmpresa)
        {
            return from paciente in gDc.Paciente
                   join empre in gDc.Empresa_Cliente on
                   paciente.ci equals empre.id_usuariocliente
                   where paciente.ci.Contains(pCodigoCliente)
                   && empre.id_empresa == pIDEmpresa
                   select paciente;

        }

        /// <summary>
        /// Devuelve los datos de un cliente, que coincidan con el parametro
        /// </summary>
        /// <param name="pCodigoCliente">Login del Cliente </param>
        /// <returns>Retorna el Paciente en un DataTable, vacia caso de que no exista</returns>
        public DataTable Get_ClientePacienteCoinp(String pCodigoCliente, int pIDEmpresa)
        {

            return Converter<Paciente>.Convert(Get_ClientePacienteCoin(pCodigoCliente, pIDEmpresa).ToList());
        }

        private IEnumerable<Paciente> Get_PacienteCI(String pCi)
        {
            return from paciente in gDc.Paciente
                   where paciente.ci == pCi
                   select paciente;

        }

        /// <summary>
        /// Devuelve los datos de un cliente, que coincidan con el parametro
        /// </summary>
        /// <param name="pCodigoCliente">Login del Cliente </param>
        /// <returns>Retorna el Paciente en un DataTable, vacia caso de que no exista</returns>
        public DataTable Get_ClientePacienteCIp(String pCi)
        {

            return Converter<Paciente>.Convert(Get_PacienteCI(pCi).ToList());
        }


        #endregion
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
                var sql = from user in gDc.UsuarioCliente
                          where user.Login == pCI
                          select user;

                if (sql.Count() > 0)
                    return -2;
            }
            if (pEmail.Length < 5 && !pEmail.Contains('@') && !pEmail.Contains(".com"))
                return -3;
            return 1;
        }
        /// <summary>
        /// Asigna a un cliente a un empresa. Genera su pass y envia correo de bienvenida
        /// </summary>
        /// <param name="pCodCliente">Codigo cliente</param>
        /// <param name="pIDEmpresa">ID empresa</param>
        /// <param name="pemail">Email del cliente</param>
        /// <param name="pIDUsuario">ID Usuario</param>
        /// <param name="pIDPaciente">ID paciente, para sacar sus datos personales</param>
        /// <returns></returns>
        public int AsignarCliente_Empresa(string pCodCliente, int pIDEmpresa, string pemail, string pIDUsuario, int pIDPaciente)
        {
            String vPass = new Encriptador().Generar_Aleatoriamente();
            int vInsert = gADMCliente.Insertar(pCodCliente, vPass, pIDUsuario);

            if (vInsert == 1)
            {
                vInsert = Asignar_Empresa(pIDEmpresa, pCodCliente, "", pIDUsuario);
                if (vInsert == 1)
                {
                    vInsert = Quitar_Cliente_Padre(pIDPaciente,pCodCliente, pIDUsuario);
                    gADMCliente.Enviar_Bienvenida(pIDEmpresa, pemail, vPass, pCodCliente);
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
        /// <param name="pCodCliente">codigo cliente a desligar</param>
        /// <param name="pIDUsuario">ID del usuario que realiza accion</param>
        /// <returns></returns>
        private int Quitar_Cliente_Padre(int pIDPaciente,string pCodCliente, string pIDUsuario)
        {
           

                var cl_pac = from c_p in gDc.Cliente_Paciente
                             where c_p.id_paciente == pIDPaciente
                             select c_p;
                foreach (var cp in cl_pac)
                {
                    try
                    {
                        gDc.Cliente_Paciente.DeleteOnSubmit(cp);
                        gCb.Insertar("Se elimino un Cliente_Paciente", pIDUsuario);
                    }
                    catch (Exception ex)
                    {
                        gLe.Insertar("NConsulta", "ABMPAciente", "Eliminar_Paciente", ex);
                        return 0;
                    }
                }
                Asignar_Paciente(pIDPaciente, pCodCliente, pIDUsuario);
                try
                {
                    gDc.SubmitChanges();
                    return 1;
                }
                catch (Exception ex)
                {
                    gLe.Insertar("NConsulta", "ABMPAciente", "Eliminar_Paciente", ex);
                    return 0;
                }
           
        }
        //desde aqui es lo que uso
        public List<PacienteDto> GetPacientesEmpresa(int pIDConsultorio) {

          return (from uc in gDc.Cliente_Paciente
                         from p in gDc.Paciente
                         from cc in gDc.Empresa_Cliente
                         where cc.id_empresa == pIDConsultorio
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
                             TipoSangre = p.tipo_sangre
                         }).ToList();
        }

       
    }
}
