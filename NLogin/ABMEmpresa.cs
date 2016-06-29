namespace NLogin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Datos;
    using System.Data;
    using DataTableConverter;
    using NEventos;
    using System.Data.Linq;
    using Herramientas;
    using DataContext = Datos.DataContext;

    public class ABMEmpresa
    {
        #region VariablesGlogales

        readonly static DataContext dataContext = new DataContext();

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Devuelve las licencias de una empresa
        /// </summary>
        /// <param name="idClinica">ID de la empresa </param>
        /// <returns>Un Datatable con la licencia</returns>
        public static DataTable ObtenerLicencia(int idClinica)
        {
            return Converter<Licencia>.Convert(ObtenerLicenciaPorId(idClinica).ToList());
        }
        
        public static List<EmpresaClinicaDto> ObtenerConsultoriosPorCliente(string loginCliente)
        {
            return (from empresa in dataContext.Empresa
                    join empresaCliente in dataContext.Empresa_Cliente on empresa.ID equals empresaCliente.id_empresa
                    join clinica in dataContext.Clinica on empresa.IDClinica equals clinica.ID
                    from tiempoConsulta in dataContext.Tiempo_Consulta
                    where empresa.Estado == true && empresaCliente.id_usuariocliente == loginCliente && empresa.ID != 1
                    && tiempoConsulta.ID == empresa.IDIntervalo
                    select new EmpresaClinicaDto()
                    {
                        Email = empresa.Email,
                        Estado = empresa.Estado,
                        IDClinica = empresa.IDClinica,
                        Login = clinica.Login,
                        Nit = empresa.NIT,
                        IDEmpresa = empresa.ID,
                        Latitud = clinica.Latitud.ToString(),
                        Longitud = clinica.Longitud.ToString(),
                        LoginCliente = empresaCliente.id_usuariocliente,
                        Direccion = clinica.Direccion,
                        Descripcion = clinica.Descripcion,
                        Nombre = clinica.Nombre,
                        TiempoCita = tiempoConsulta.Value
                    }).ToList();
        }
        
        /// <summary>
        /// Permite InsertarModulo una nueva Empresa
        /// </summary>
        /// <param name="clinicaDto">Datos Empresa</param>
        /// <param name="idUsuario">ID del usuario que realiza la accion</param>
        /// <returns>  0 - No inserto la empresa
        ///            1 - Inserto la Empresa 
        ///          </returns>
        public static int InsertarClinica(ClinicaDto clinicaDto, string idUsuario)
        {
            Clinica vClinicaDefault = new Clinica();
            vClinicaDefault.Nombre = clinicaDto.Nombre;
            vClinicaDefault.IDUsuarioCreacion = idUsuario;
            vClinicaDefault.Login = clinicaDto.Login;
            if (clinicaDto.logoImagen == null)
            {
                vClinicaDefault.ImagenLogo = null;
            }
            else
            {
                Binary vbi = new Binary(clinicaDto.logoImagen);
                vClinicaDefault.ImagenLogo = vbi;
            }
            vClinicaDefault.Latitud = Decimal.Round(Convert.ToDecimal(clinicaDto.Latitud), 8);
            vClinicaDefault.Longitud = Decimal.Round(Convert.ToDecimal(clinicaDto.Longitud), 8);
            vClinicaDefault.CantidadConsultorios = 1;
            vClinicaDefault.Descripcion = clinicaDto.Descripcion;
            vClinicaDefault.Direccion = clinicaDto.Direccion;
            vClinicaDefault.Estado = true;
            vClinicaDefault.FechaCreacion = DateTime.Today;
            vClinicaDefault.FechaDeModificacion = DateTime.Today;
            try
            {
                dataContext.Clinica.InsertOnSubmit(vClinicaDefault);
                dataContext.SubmitChanges();

                int newIDClinica = ObtenerCodigo();

                ControlBitacora.Insertar("Se inserto una Clinica", idUsuario);
                ActivarLicencia(newIDClinica, 12, idUsuario);
                InsertarTelefonosClinica(clinicaDto.Telefonos, idUsuario, newIDClinica);
                InsertarTrabajosClinica(clinicaDto.Trabajos, idUsuario, newIDClinica);
                clinicaDto.Consultorios[0].IDClinica = newIDClinica;
                clinicaDto.Consultorios[0].Login = clinicaDto.Login;
                clinicaDto.Consultorios[0].IDUsuarioCreador = idUsuario;
                clinicaDto.Consultorios[0].NombreClinica = clinicaDto.Nombre;
                return InsertarConsultorio(clinicaDto.Consultorios[0]);
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "InsertarModulo", ex);
                return 0;
            }
        }

        public static int ModificarClinica(ClinicaDto clinicaDto, string idUsuario)
        {
            var clinicaCurrent = (from c in dataContext.Clinica
                                  where c.ID == clinicaDto.IDClinica
                                  select c).FirstOrDefault();
            if (clinicaCurrent != null)
            {


                clinicaCurrent.Nombre = clinicaDto.Nombre;
                clinicaCurrent.IDUsuarioCreacion = idUsuario;
                clinicaCurrent.Login = clinicaDto.Login;
                if (clinicaDto.logoImagen == null)
                {
                    clinicaCurrent.ImagenLogo = null;
                }
                else
                {
                    Binary vbi = new Binary(clinicaDto.logoImagen);
                    clinicaCurrent.ImagenLogo = vbi;
                }
                clinicaCurrent.Latitud = Decimal.Round(Convert.ToDecimal(clinicaDto.Latitud), 8);
                clinicaCurrent.Longitud = Decimal.Round(Convert.ToDecimal(clinicaDto.Longitud), 8);
                clinicaCurrent.Descripcion = clinicaDto.Descripcion;
                clinicaCurrent.Direccion = clinicaDto.Direccion;
                clinicaCurrent.Estado = true;
                clinicaCurrent.FechaDeModificacion = DateTime.Today;
                try
                {

                    ControlBitacora.Insertar("Se modifico una Clinica", idUsuario);
                    InsertarTelefonosClinica(clinicaDto.Telefonos, idUsuario, clinicaDto.IDClinica);
                    InsertarTrabajosClinica(clinicaDto.Trabajos, idUsuario, clinicaDto.IDClinica);
                    dataContext.SubmitChanges();
                    //clinicaDto.Consultorios[0].idClinica = clinicaDto.idClinica;
                    // clinicaDto.Consultorios[0].Login = clinicaDto.Login + "1";
                    // clinicaDto.Consultorios[0].IDUsuarioCreador = idUsuario;
                    // clinicaDto.Consultorios[0].NombreClinica = clinicaDto.Nombre;
                    // return InsertarConsultorio(clinicaDto.Consultorios[0]);
                    return 1;

                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "Modificar", ex);
                    return 0;
                }
            }
            else

                return 0;
        }

        public static int ModificarConsultorio(ConsultorioDto consultorioDto, string idUsuario)
        {
            var sql = from e in dataContext.Empresa
                      where e.ID == consultorioDto.IDConsultorio
                      select e;

            if (sql.Any())
            {
                var empresa = sql.First();
                empresa.Email = consultorioDto.Email;
                empresa.NIT = consultorioDto.NIT;
                empresa.IDIntervalo = consultorioDto.IDIntervalo;
                empresa.FechaModificacion = DateTime.Now;
                try
                {
                    dataContext.SubmitChanges();
                    InsertarTelefonosConsultorio(consultorioDto.Telefonos, idUsuario);
                    InsertarTrabajos(consultorioDto.Trabajos, consultorioDto.IDConsultorio, idUsuario);
                    ControlBitacora.Insertar("Se modifico un Consultorio", idUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "Modificar", ex);
                    return 0;
                }
            }
            return 0;
        }

        public static int EliminarClinica(int idClinica, string idUsuario)
        {
            var sqlClinica = (from c in dataContext.Clinica
                              where c.ID == idClinica
                              select c).FirstOrDefault();
            if (sqlClinica != null)
            {
                sqlClinica.Estado = false;
                var sqlConsultorio = (from c in dataContext.Empresa
                                      where c.IDClinica == idClinica
                                      select c).ToList();
                foreach (var consul in sqlConsultorio)
                {
                    consul.Estado = false;
                }
            }

            try
            {
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se Desactivo una clinica: " + idClinica, idUsuario);
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "Eliminar", ex);
                return 0;
            }


        }

        public static int HabilitarClinica(int idClinica, string idUsuario)
        {
            var sqlClinica = (from c in dataContext.Clinica
                              where c.ID == idClinica
                              select c).FirstOrDefault();
            if (sqlClinica != null)
            {
                sqlClinica.Estado = true;
                var sqlConsultorio = (from c in dataContext.Empresa
                                      where c.IDClinica == idClinica
                                      select c).ToList();
                foreach (var consul in sqlConsultorio)
                {
                    consul.Estado = true;
                }
            }

            try
            {
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se habilito una clinica: " + idClinica, idUsuario);
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "Habilitar", ex);
                return 0;
            }


        }

        /// <summary>
        /// Permite desactivar una empresa
        /// </summary>
        /// <param name="idConsultorio">Id de la Empresa a desactivar</param>
        /// <param name="idUsuario">Id del Usuario Que realiza accion</param>
        /// <returns> 0 - Ocurrio un error No se elimino
        ///         1 - Se elimino Correctamente
        ///         2 - No Existe la Empresa  </returns>
        public static int EliminarConsultorio(int idConsultorio, string idUsuario)
        {
            var sql = from e in dataContext.Empresa
                      where e.ID == idConsultorio
                      select e;

            if (sql.Any())
            {
                sql.First().Estado = !sql.First().Estado;
                var empresas = (from e in dataContext.Empresa
                                where e.IDClinica == sql.First().IDClinica
                                && e.Estado == true
                                select e);
                try
                {
                    if (empresas.Count() == 1)
                    {
                        EliminarClinica(empresas.FirstOrDefault().ID, idUsuario);
                    }
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se Desactivo una empresa: " + idConsultorio, idUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "Eliminar", ex);
                    return 0;
                }
            }
            return 2;
        }

        public static int HabilitarConsultorio(int idConsultorio, string idUsuario)
        {
            var sql = from e in dataContext.Empresa
                      where e.ID == idConsultorio
                      select e;

            if (sql.Any())
            {
                sql.First().Estado = !sql.First().Estado;
                try
                {

                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se Desactivo una empresa: " + idConsultorio, idUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "Eliminar", ex);
                    return 0;
                }
            }
            return 2;
        }

        /// <summary>
        /// Devuelve en un STRING la licencia de un consultorio
        /// </summary>
        /// <param name="idClinica">ID del consultorio</param>
        /// <returns>string </returns>
        public static string ObtenerLicenciaClinica(int idClinica)
        {
            DataTable vDTLicencias = ObtenerLicencia(idClinica);
            String vLicencia = "";
            int vCnt = vDTLicencias.Rows.Count;
            if (vCnt > 0)
            {
                foreach (DataRow lic in vDTLicencias.Rows)
                {
                    if (((DateTime)lic[2]).CompareTo(DateTime.Now.AddHours(DirefenciaHora())) > 0 || vCnt == 1)
                    {
                        vLicencia = ((DateTime)lic[1]).ToShortDateString() + " - ";
                        break;
                    }
                }

                vLicencia = vLicencia + ((DateTime)vDTLicencias.Rows[vCnt - 1][2]).ToShortDateString();
            }
            return vLicencia;
        }

        public static int DirefenciaHora()
        {
            var sql = from p in dataContext.ParametroSistemas
                      where p.Elemento == "DiferenciaHora"
                      select p;
            if (sql.Any())
            {
                var valorI = sql.First().ValorI;
                if (valorI != null) return (int)valorI;
            }
            return 0;
        }

        public static ConsultorioDto ObtenerConsultorioPorId(int idConsultorio)
        {
            return (from c in dataContext.Empresa
                    from tc in dataContext.Tiempo_Consulta
                    where c.ID == idConsultorio && tc.ID == c.IDIntervalo
                    && c.Estado
                    select new ConsultorioDto()
                    {
                        Email = c.Email,
                        Estado = c.Estado,
                        TiempoCita = tc.Value,
                        FechaCreacion = c.FechaCreacion,
                        FechaModificacion = c.FechaModificacion,
                        IDClinica = c.IDClinica,
                        IDConsultorio = c.ID,
                        IDIntervalo = c.IDIntervalo,
                        IDUsuarioCreador = c.IDUsuarioCreador,
                        Login = c.Login,
                        NIT = c.NIT,
                        Telefonos = ObtenerTelefonosConsultorios(c.ID, c.IDClinica)
                    }).FirstOrDefault();
        }

        public static List<ConsultorioDto> ObtenerConsultoriosPorClinica(int idClinica)
        {
            return (from c in dataContext.Empresa
                    from tc in dataContext.Tiempo_Consulta
                    where c.IDClinica == idClinica && tc.ID == c.IDIntervalo

                    select new ConsultorioDto()
                    {
                        Email = c.Email,
                        Estado = c.Estado,
                        TiempoCita = tc.Value,
                        FechaCreacion = c.FechaCreacion,
                        FechaModificacion = c.FechaModificacion,
                        IDClinica = c.IDClinica,
                        IDConsultorio = c.ID,
                        IDIntervalo = c.IDIntervalo,
                        IDUsuarioCreador = c.IDUsuarioCreador,
                        Login = c.Login,
                        NIT = c.NIT,
                        Telefonos = ObtenerTelefonosConsultorios(c.ID, c.IDClinica),
                        Trabajos = ObtenerTrabajosConsultorio(c.ID)
                    }).ToList();
        }

        public static IEnumerable<Tiempo_Consulta> ObtenerIntervalosDeTiempo()
        {
            return from inte in dataContext.Tiempo_Consulta
                   select inte;
        }

        public static List<ConsultorioDto> ObtenerConsultorios(int idClinica)
        {
            return (from e in dataContext.Empresa
                    from tc in dataContext.Tiempo_Consulta
                    where e.IDClinica == idClinica
                    && tc.ID == e.IDIntervalo
                    select new ConsultorioDto()
                    {
                        Email = e.Email,
                        Estado = e.Estado,
                        FechaCreacion = e.FechaCreacion,
                        FechaModificacion = e.FechaModificacion,
                        IDClinica = e.IDClinica,
                        IDConsultorio = e.ID,
                        IDIntervalo = e.IDIntervalo,
                        IDUsuarioCreador = e.IDUsuarioCreador,
                        Login = e.Login,
                        NIT = e.NIT,
                        Telefonos = ObtenerTelefonosConsultorios(e.ID, idClinica),
                        Trabajos = ObtenerTrabajosConsultorio(e.ID),
                        TiempoCita = tc.Value
                    }).ToList();
        }

        private static List<TrabajosConsultorioDto> ObtenerTrabajosConsultorio(int idConsultorio)
        {
            return (from tc in dataContext.TrabajosConsultorio
                    where tc.IDConsultorio == idConsultorio
                    select tc).Select(x => new TrabajosConsultorioDto()
                    {
                        ID = x.IDTrabajo,
                        IDConsultorio = x.IDConsultorio,
                        State = 0
                    }).ToList();
        }

        public static List<TelefonoDto> ObtenerTelefonosConsultorios(int idEmpresa, int idClinica)
        {
            return (from t in dataContext.Telefono
                    join tc in dataContext.TelefonoConsultorio on t.ID equals tc.IDTelefono
                    where t.Estado == true
                    select t).Select(x => new TelefonoDto()
                    {
                        IDClinica = idClinica,
                        IDConsultorio = idEmpresa,
                        Nombre = x.Nombre,
                        Telefono = x.Telefono1,
                        ID = x.ID
                    }).ToList();
        }

        public static List<TelefonoDto> ObtenerTelefonosClinica(int idClinica)
        {
            return (from t in dataContext.Telefono
                    join tc in dataContext.TefonosClinica on t.ID equals tc.IDTelefono
                    where t.Estado == true
                    && tc.IDClinica == idClinica
                    select t).Select(x => new TelefonoDto()
                    {
                        IDClinica = idClinica,
                        IDConsultorio = -1,
                        Nombre = x.Nombre,
                        Telefono = x.Telefono1,
                        ID = x.ID,
                        State = 0
                    }).ToList();
        }

        public static int InsertarConsultorio(ConsultorioDto consultorioDto)
        {
            var idEmpresa = (from e in dataContext.Empresa
                             select e);
            int newID = idEmpresa == null ? 1 : idEmpresa.Max(x => x.ID) + 1;
            var consultorioToSave = new Empresa()
            {
                Email = consultorioDto.Email,
                Estado = true,
                FechaCreacion = DateTime.Today,
                FechaModificacion = DateTime.Today,
                ID = newID,
                IDClinica = consultorioDto.IDClinica,
                IDIntervalo = consultorioDto.IDIntervalo,
                IDUsuarioCreador = consultorioDto.IDUsuarioCreador,
                Login = consultorioDto.Login,
                NIT = consultorioDto.NIT
            };
            try
            {
                dataContext.Empresa.InsertOnSubmit(consultorioToSave);

                dataContext.SubmitChanges();

                ControlBitacora.Insertar("Se inserto un Consultorio", consultorioDto.IDUsuarioCreador);

                //ActivarLicencia(consultorioDto.idClinica, 12, consultorioDto.IDUsuarioCreador);
                // int vRD = Crear_Roles_default(sql.First().ID, idUsuario);
                var IDrol = from r in dataContext.Rol
                            where r.Nombre == "Administrador Dentista"
                            && r.IDEmpresa == newID
                            select r;
                InsertarTelefonosConsultorio(consultorioDto.Telefonos, consultorioDto.IDUsuarioCreador);
                InsertarTrabajos(consultorioDto.Trabajos, newID, consultorioDto.IDUsuarioCreador);
                SMTP vSMTP = new SMTP();
                string vPass = Encriptador.GenerarAleatoriamente();

                ABMUsuarioEmpleado.Insertar("Administrador", "admin", newID, IDrol.First().ID, vPass, vPass, true, consultorioDto.IDUsuarioCreador);
                dataContext.SubmitChanges();

                String vMensaje = "Bienvenido a Odontoweb, usted pertenece a la clinica" + consultorioDto.NombreClinica + " sus datos para ingresar\nal" +
                                    " sistema son:\nConsultorio : " + consultorioDto.Login + " \nUsuario:admin \nPassword: " + vPass + "." +
                                    "\nAl momento de ingresar se le pedira que actualice su contraseña."
                                    + "\nSaludos,\nOdontoweb";
                vSMTP.Datos_Mensaje(consultorioDto.Email, vMensaje, "Bienvenido a Odontoweb");
                vSMTP.Enviar_Mail();
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "InsertarConsultorio", ex);
                return 0;
            }
        }

        public static void InsertarTelefonosConsultorio(List<TelefonoDto> listaTelefonos, string idUsuarioCreador)
        {
            if (listaTelefonos == null) return;
            foreach (var ptelefono in listaTelefonos)
            {
                var vTelefono = new Telefono();

                switch (ptelefono.State)
                {
                    case 1:
                        ABMTelefono.InsertarConsultorio(ptelefono);
                        ControlBitacora.Insertar("Se inserto un Telefono", idUsuarioCreador);
                        break;
                    case 2:
                        ABMTelefono.Modificar(ptelefono.IDConsultorio, ptelefono.Telefono, ptelefono.Nombre, ptelefono.ID);
                        ControlBitacora.Insertar("Se Modifico un Telefono", idUsuarioCreador);
                        break;
                    case 3:
                        vTelefono = (from t in dataContext.Telefono
                                     where t.ID == ptelefono.ID
                                     select t).FirstOrDefault();
                        if (vTelefono != null)
                        {
                            dataContext.Telefono.DeleteOnSubmit(vTelefono);
                            dataContext.SubmitChanges();
                            ControlBitacora.Insertar("Se dio de baja un Telefono", idUsuarioCreador);
                        }
                        break;
                }

            }

        }

        public static void InsertarTelefonosClinica(List<TelefonoDto> listaTelefonos, string idUsuarioCreador, int idClinica)
        {
            if (listaTelefonos == null) return;
            foreach (var ptelefono in listaTelefonos)
            {
                var vTelefono = new Telefono();

                switch (ptelefono.State)
                {
                    case 1:
                        ABMTelefono.InsertarTelefonosDeLaClinica(idClinica, ptelefono.Telefono, ptelefono.Nombre);
                        ControlBitacora.Insertar("Se inserto un Telefono", idUsuarioCreador);
                        break;
                    case 2:
                        ABMTelefono.Modificar(ptelefono.IDConsultorio, ptelefono.Telefono, ptelefono.Nombre, ptelefono.ID);
                        ControlBitacora.Insertar("Se Modifico un Telefono", idUsuarioCreador);
                        break;
                    case 3:
                        vTelefono = (from t in dataContext.Telefono
                                     where t.ID == ptelefono.ID
                                     select t).FirstOrDefault();
                        if (vTelefono != null)
                        {
                            dataContext.Telefono.DeleteOnSubmit(vTelefono);
                            var tesConsultorio = (from tec in dataContext.TelefonoConsultorio
                                                  where tec.IDTelefono == ptelefono.ID
                                                  select tec);
                            foreach (var tec in tesConsultorio)
                            {
                                dataContext.TelefonoConsultorio.DeleteOnSubmit(tec);
                            }
                            dataContext.SubmitChanges();
                            ControlBitacora.Insertar("Se dio de baja un Telefono", idUsuarioCreador);
                        }
                        break;
                }

            }

        }

        public static List<TrabajosClinicaDto> ObtenerTrabajosClinica(int idClinica)
        {
            var trabajosClinica = (from t in dataContext.Trabajos
                                   where t.IDClinica == idClinica
                                   select t);
            if (trabajosClinica.Any())
            {
                var trabajosConsul = (from tc in dataContext.TrabajosConsultorio
                                      where tc.IDConsultorio == idClinica
                                      select tc);
                return trabajosClinica.Select(trabajo => new TrabajosClinicaDto()
                {
                    ID = trabajo.ID,
                    IDClinica = trabajo.IDClinica,
                    Descripcion = trabajo.Descripcion,
                    IDConsultorio = trabajosConsul.Where(tc => tc.IDTrabajo == trabajo.ID).Select(id => id.IDConsultorio).ToList()
                }).ToList();
            }
            return new List<TrabajosClinicaDto>();
        }

        public static List<ClinicaDto> ObtenerClinicas()
        {
            return (from c in dataContext.Clinica
                    where !c.Login.ToUpper().Equals("DACASYS")
                    && !c.Login.ToUpper().Equals("DEFAULT")
                    select c).Select(x => new ClinicaDto()
                    {
                        Descripcion = x.Descripcion,
                        Direccion = x.Direccion,
                        Estado = x.Estado,
                        FechaCreacion = x.FechaCreacion,
                        FechaModificacion = x.FechaDeModificacion,
                        IDClinica = x.ID,
                        Latitud = x.Latitud.ToString(),
                        Login = x.Login,
                        Longitud = x.Longitud.ToString(),
                        Nombre = x.Nombre,
                        Consultorios = ObtenerConsultorios(x.ID),
                        Trabajos = ObtenerTrabajosClinica(x.ID),
                        Status = 0,
                        Telefonos = ObtenerTelefonosClinica(x.ID)
                    }).ToList();
        }

        public static List<ClinicaDto> ObtenerClinicasHabilitadas()
        {
            return (from c in dataContext.Clinica
                    where !c.Login.ToUpper().Equals("DACASYS")
                    && !c.Login.ToUpper().Equals("DEFAULT")
                    && c.Estado == true
                    select c).Select(x => new ClinicaDto()
                    {
                        Descripcion = x.Descripcion,
                        Direccion = x.Direccion,
                        Estado = x.Estado,
                        FechaCreacion = x.FechaCreacion,
                        FechaModificacion = x.FechaDeModificacion,
                        IDClinica = x.ID,
                        Latitud = x.Latitud.ToString(),
                        Login = x.Login,
                        Longitud = x.Longitud.ToString(),
                        Nombre = x.Nombre,
                        Consultorios = ObtenerConsultorios(x.ID),
                        Trabajos = ObtenerTrabajosClinica(x.ID),
                        Status = 0,
                        Telefonos = ObtenerTelefonosClinica(x.ID)
                    }).ToList();
        }
        
        /// <summary>
        /// Devuelve una empresa segun su ID
        /// </summary>
        /// <param name="idEmpresa">ID de la empresa</param>
        /// <returns>un DataTable con los datos de la empresa</returns>
        public static DataTable ObtenerEmpresa(int idEmpresa)
        {
            return Converter<Empresa>.Convert(ObtenerEmpresaPorId(idEmpresa).ToList());
        }
        
        /// <summary>
        /// Devuelve los ID's de todos los consultorios
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable ObtenerIdEmpresa()
        {
            return Converter<getIDEmpresasResult>.Convert(ObtenerListaIdEmpresa().ToList());
        }
        
        public static Clinica ObtenerClinica(int idClinica)
        {
            return (from c in dataContext.Clinica
                    where c.ID == idClinica
                    select c).FirstOrDefault();
        }

        #endregion

        #region Metodos Privados
        
        private static void InsertarTrabajosClinica(List<TrabajosClinicaDto> listaTrabajos, string idUsuario, int idClinica)
        {
            if (listaTrabajos == null) return;
            foreach (var ptrabajo in listaTrabajos)
            {
                var vTrabajo = new Trabajos();

                switch (ptrabajo.State)
                {
                    case 1:
                        vTrabajo.Descripcion = ptrabajo.Descripcion;
                        vTrabajo.IDClinica = idClinica;
                        try
                        {
                            dataContext.Trabajos.InsertOnSubmit(vTrabajo);
                            ControlBitacora.Insertar("Se inserto un Trabajo", idUsuario);
                        }
                        catch (Exception ex)
                        {
                            ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "InsertarTrabajosClinica", ex);
                        }

                        break;
                    case 2:
                        vTrabajo = (from t in dataContext.Trabajos
                                    where t.ID == ptrabajo.ID
                                    select t).FirstOrDefault();
                        if (vTrabajo != null)
                        {
                            try
                            {
                                vTrabajo.Descripcion = ptrabajo.Descripcion;
                                dataContext.SubmitChanges();
                                ControlBitacora.Insertar("Se inserto un Trabajo", idUsuario);
                            }
                            catch (Exception ex)
                            {
                                ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "InsertarTrabajosClinica", ex);
                            }

                        }

                        break;
                    case 3:
                        vTrabajo = (from t in dataContext.Trabajos
                                    where t.ID == ptrabajo.ID
                                    select t).FirstOrDefault();
                        if (vTrabajo != null)
                        {
                            try
                            {
                                dataContext.Trabajos.DeleteOnSubmit(vTrabajo);
                                var trabajosConsultorio = (from tc in dataContext.TrabajosConsultorio
                                                           where tc.IDTrabajo == ptrabajo.ID
                                                           select tc);
                                foreach (var tc in trabajosConsultorio)
                                {
                                    dataContext.TrabajosConsultorio.DeleteOnSubmit(tc);
                                }
                                dataContext.SubmitChanges();
                                ControlBitacora.Insertar("Se elimino un Trabajo", idUsuario);
                            }
                            catch (Exception ex)
                            {
                                ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "InsertarTrabajosClinica", ex);
                            }

                        }
                        break;
                }

            }
        }

        private static int ObtenerCodigo()
        {
            var clinicas = (from c in dataContext.Clinica
                            select c);
            return clinicas == null ? 1 : clinicas.Max(x => x.ID);
        }

        private static void InsertarTrabajos(List<TrabajosConsultorioDto> listaTrabajos, int idConsultorio, string idUsuarioCreador)
        {
            if (listaTrabajos == null) return;
            foreach (var pTrabajo in listaTrabajos)
            {
                var vTrabajoConsultorio = new TrabajosConsultorio();

                switch (pTrabajo.State)
                {
                    case 1:
                        vTrabajoConsultorio.IDConsultorio = idConsultorio;
                        vTrabajoConsultorio.IDTrabajo = pTrabajo.ID;

                        dataContext.TrabajosConsultorio.InsertOnSubmit(vTrabajoConsultorio);
                        dataContext.SubmitChanges();
                        ControlBitacora.Insertar("Se inserto un TrabajoConsultorio", idUsuarioCreador);
                        break;

                    case 3:
                        vTrabajoConsultorio = (from t in dataContext.TrabajosConsultorio
                                               where t.IDTrabajo == pTrabajo.ID && t.IDConsultorio == idConsultorio
                                               select t).FirstOrDefault();
                        if (vTrabajoConsultorio != null)
                        {
                            dataContext.TrabajosConsultorio.DeleteOnSubmit(vTrabajoConsultorio);
                            dataContext.SubmitChanges();
                            ControlBitacora.Insertar("Se dio de baja un Trabajo consultorio", idUsuarioCreador);
                        }
                        break;
                }

            }

        }
        
        /// <summary>
        /// Busca una empresa segun el parametro que reciba, busca por nombre y login
        /// excepto la default
        /// </summary>
        /// <param name="idClinica">Parametro de busqueda</param>
        /// <returns>Un Datatable con los datos de las empresas encontradas</returns>


        /// <summary>
        /// Metodo privado que retorna las licencias de un consultorio
        /// </summary>
        /// <param name="pIDEmpresa">ID del consultorio</param>
        /// <returns>IEnumerable</returns>
        private static IEnumerable<Licencia> ObtenerLicenciaPorId(int idClinica)
        {

            return from l in dataContext.Licencia
                   where l.IDClinica == idClinica
                   select l;

        }
        
        /// <summary>
        /// Metodo privado que devuelve un consultorio 
        /// </summary>
        /// <param name="idEmpresa">ID consultorio</param>
        /// <returns>IEnumerable</returns>
        private static IEnumerable<Empresa> ObtenerEmpresaPorId(int idEmpresa)
        {

            return from e in dataContext.Empresa
                   where e.ID == idEmpresa
                   select e;

        }
        
        /// <summary>
        /// Metodo privado que retorna los ID de todos los consultorios
        /// </summary>
        /// <returns>IEnumerable</returns>
        private IEnumerable<getIDEmpresasResult> ObtenerListaIdEmpresa()
        {
            return dataContext.getIDEmpresas();
        }

        /// <summary>
        /// Crea la licencia a la nueva empresa
        /// </summary>
        /// <param name="idClinica">ID de la nueva empresa</param>
        /// <param name="mes">Cantidad de meses de la licencia</param>
        /// <param name="idUsuario">Id del usuario que realiza accion</param>
        private static void ActivarLicencia(int idClinica, int mes, string idUsuario)
        {
            Licencia vlicencia = new Licencia();
            vlicencia.IDClinica = idClinica;
            vlicencia.FechaInicio = DateTime.Now.AddHours(DirefenciaHora());
            vlicencia.FechaFin = DateTime.Now.AddHours(DirefenciaHora()).AddMonths(mes);
            try
            {
                dataContext.Licencia.InsertOnSubmit(vlicencia);
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se activo una licencia para el consultorio " + idClinica, idUsuario);
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "ActivarLicencia", ex);
            }
        }

        #endregion
    }
}