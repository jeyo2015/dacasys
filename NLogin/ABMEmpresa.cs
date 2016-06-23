﻿namespace NLogin
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
        /// Retorna la empresa deseado segun su login
        /// </summary>
        /// <param name="pLogin">Login de empresa</param>
        /// <returns>Un datable, vacio si no exite la empresa</returns>
        public DataTable Get_Empresap(string pLogin)
        {
            return Converter<Empresa>.Convert(Get_Empresa(pLogin).ToList());
        }

        /// <summary>
        /// Devuelve las licencias de una empresa
        /// </summary>
        /// <param name="pIDEmpresa">ID de la empresa </param>
        /// <returns>Un Datatable con la licencia</returns>
        public static DataTable Get_Licenciap(int pIDClinica)
        {
            return Converter<Licencia>.Convert(Get_Licencia(pIDClinica).ToList());
        }

        /// <summary>
        /// Devuelve las Empresas segun el usurio
        /// </summary>
        /// <param name="pLoginUsuario">ID del Usuario </param>
        /// <returns>un DataTable que contiene todas las empresas</returns>
        public DataTable Get_Empresasp(String pLoginUsuario)
        {
            return Converter<Empresa>.Convert(Get_Empresas(pLoginUsuario).ToList());
        }

        // TODO Fix this function.
        public static List<EmpresaClinicaDto> ObtenerConsultoriosPorCliente(string loginCliente)
        {
            return (from empresa in dataContext.Empresa
                    join empresaCliente in dataContext.Empresa_Cliente on empresa.ID equals empresaCliente.id_empresa
                    join clinica in dataContext.Clinica on empresa.IDClinica equals clinica.ID
                    from tiempoConsulta in dataContext.Tiempo_Consulta
                    where empresa.Estado == true && empresaCliente.id_usuariocliente == loginCliente //&& e.ID != 1
                    && tiempoConsulta.ID == empresa.IDIntervalo
                    select new EmpresaClinicaDto()
                    {
                        Email = empresa.Email,
                        Estado = empresa.Estado,
                        IDClinica = empresa.IDClinica,
                        Login = clinica.Login,
                        Nit = empresa.NIT,
                        IDEmpresa = empresa.ID,
                        Latitud = clinica.Latitud,
                        Longitud = clinica.Longitud,
                        LoginCliente = empresaCliente.id_usuariocliente,
                        Direccion = clinica.Direccion,
                        Descripcion = clinica.Descripcion,
                        Nombre = clinica.Nombre,
                        TiempoCita = tiempoConsulta.Value
                    }).ToList();
        }

        /// <summary>
        /// Validar los Campos para crear o modificar una empresa
        /// </summary>
        /// <param name="pLogin">Login de la empresa</param>
        /// <param name="pNombre">Nombre de la empresa</param>
        /// <param name="pDireccion">Direccion de la Empresa</param>
        /// <param name="pLatitud">Latitud de la empresa</param>
        /// <param name="pLongitud">Longitud de la empresa</param>
        /// <param name="pemail">email de la empresa</param>
        /// <param name="pmes">mes de la licencia</param>
        /// <param name="pNIT">Nit de la empresa</param>
        /// <returns>0 - Todo OK
        ///         -2 - Login invalido
        ///         -3 - Nombre invalido
        ///         -4 - Direccion invalida
        ///         -5 - Latitud invalida
        ///         -6 - Longitud invalida
        ///         -7 - Email invalido
        ///         -8 - Mes invalido
        ///         -9 - Nit invalido</returns>
        public int validar(string pLogin, string pNombre, string pDireccion, string pLatitud,
                        string pLongitud, string pemail, int pmes, string pNIT, int pIDEmpresa, string[] ptelefonos)
        {
            if (pNombre.Length <= 1)
            {
                return -3;
            }
            if (pLogin.Length <= 1)
            {
                return -2;
            }
            else
            {
                if (pIDEmpresa == -1)
                {
                    var emp = from e in dataContext.Empresa
                              where e.Login == pLogin
                              select e;
                    if (emp.Count() > 0)
                        return -10;
                }
            }

            if (pDireccion.Length <= 5)
            {
                return -4;
            }
            if (pNIT.Length < 7)
            {
                return -9;
            }
            else
            {
                if (pIDEmpresa == -1)
                {
                    var emp = from e in dataContext.Empresa
                              where e.NIT == pNIT
                              select e;
                    if (emp.Count() > 0)
                        return -11;
                }
            }
            if (pemail.Length < 1 || !pemail.Contains("@") || !pemail.Contains(".com"))
            {
                return -7;
            }
            if (pLongitud.Length < 1 || pLatitud.Length < 1)
            {
                return -5;
            }
            if (((Convert.ToInt32(pmes)) < 1 && pIDEmpresa == -1))
            {
                return -8;
            }
            if (ptelefonos == null)
                return -6;
            return 0;

        }

        /// <summary>
        /// Permite Insertar una nueva Empresa
        /// </summary>
        /// <param name="pClinica">Datos Empresa</param>
        /// <param name="pIDUsuario">ID del usuario que realiza la accion</param>
        /// <returns>  0 - No inserto la empresa
        ///            1 - Inserto la Empresa 
        ///          </returns>
        public static int InsertarClinica(ClinicaDto pClinica, string pIDUsuario)
        {
            Clinica vClinicaDefault = new Clinica();
            vClinicaDefault.Nombre = pClinica.Nombre;
            vClinicaDefault.IDUsuarioCreacion = pIDUsuario;
            vClinicaDefault.Login = pClinica.Login;
            if (pClinica.logoImagen == null)
            {
                vClinicaDefault.ImagenLogo = null;
            }
            else
            {
                Binary vbi = new Binary(pClinica.logoImagen);
                vClinicaDefault.ImagenLogo = vbi;
            }
            vClinicaDefault.Latitud = Decimal.Round(Convert.ToDecimal(pClinica.Latitud), 8);
            vClinicaDefault.Longitud = Decimal.Round(Convert.ToDecimal(pClinica.Longitud), 8);
            vClinicaDefault.CantidadConsultorios = 1;
            vClinicaDefault.Descripcion = pClinica.Descripcion;
            vClinicaDefault.Direccion = pClinica.Direccion;
            vClinicaDefault.Estado = true;
            vClinicaDefault.FechaCreacion = DateTime.Today;
            vClinicaDefault.FechaDeModificacion = DateTime.Today;
            try
            {
                dataContext.Clinica.InsertOnSubmit(vClinicaDefault);
                dataContext.SubmitChanges();

                int newIDClinica = GetNextIDClinica();

                ControlBitacora.Insertar("Se inserto una Clinica", pIDUsuario);
                activar_licencia(newIDClinica, 12, pIDUsuario);
                InsertarTelefonosClinica(pClinica.Telefonos, pIDUsuario, newIDClinica);
                InsertarTrabajosClinica(pClinica.Trabajos, pIDUsuario, newIDClinica);
                pClinica.Consultorios[0].IDClinica = newIDClinica;
                pClinica.Consultorios[0].Login = pClinica.Login;
                pClinica.Consultorios[0].IDUsuarioCreador = pIDUsuario;
                pClinica.Consultorios[0].NombreClinica = pClinica.Nombre;
                return InsertarConsultorio(pClinica.Consultorios[0]);
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "Insertar", ex);
                return 0;
            }
        }

        public static int ModificarClinica(ClinicaDto pClinica, string pIDUsuario)
        {
            var clinicaCurrent = (from c in dataContext.Clinica
                                  where c.ID == pClinica.IDClinica
                                  select c).FirstOrDefault();
            if (clinicaCurrent != null)
            {


                clinicaCurrent.Nombre = pClinica.Nombre;
                clinicaCurrent.IDUsuarioCreacion = pIDUsuario;
                clinicaCurrent.Login = pClinica.Login;
                if (pClinica.logoImagen == null)
                {
                    clinicaCurrent.ImagenLogo = null;
                }
                else
                {
                    Binary vbi = new Binary(pClinica.logoImagen);
                    clinicaCurrent.ImagenLogo = vbi;
                }
                clinicaCurrent.Latitud = Decimal.Round(Convert.ToDecimal(pClinica.Latitud), 8);
                clinicaCurrent.Longitud = Decimal.Round(Convert.ToDecimal(pClinica.Longitud), 8);
                clinicaCurrent.Descripcion = pClinica.Descripcion;
                clinicaCurrent.Direccion = pClinica.Direccion;
                clinicaCurrent.Estado = true;
                clinicaCurrent.FechaDeModificacion = DateTime.Today;
                try
                {

                    ControlBitacora.Insertar("Se modifico una Clinica", pIDUsuario);
                    InsertarTelefonosClinica(pClinica.Telefonos, pIDUsuario, pClinica.IDClinica);
                    InsertarTrabajosClinica(pClinica.Trabajos, pIDUsuario, pClinica.IDClinica);
                    dataContext.SubmitChanges();
                    //pClinica.Consultorios[0].IDClinica = pClinica.IDClinica;
                    // pClinica.Consultorios[0].Login = pClinica.Login + "1";
                    // pClinica.Consultorios[0].IDUsuarioCreador = pIDUsuario;
                    // pClinica.Consultorios[0].NombreClinica = pClinica.Nombre;
                    // return InsertarConsultorio(pClinica.Consultorios[0]);
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

            if (sql.Count() > 0)
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

        /// <summary>
        /// Permite Modificar una empresa segun el ID
        /// </summary>
        /// <param name="pLogin">Login de la Empresa</param>
        /// <param name="pNombre">Nombre de la Empresa</param>
        /// <param name="pDireccion">Direccion de la Empresa</param>
        /// <param name="pLatitud">Latitud de la empresa</param>
        /// <param name="pLongitud">Longitud de la Empresa</param>
        /// <param name="pimageLog">Imagen del logo</param>
        /// <param name="pimagemapa">Imagen para el Mapa</param>
        /// <param name="pNIT">Nit de la empresa</param>
        /// <param name="pmes">Numero de meses de la licencia de la empresa</param>
        /// <param name="pIDUsuario">ID del usuario que realiza la accion</param>
        /// <returns>  0 - No inserto la empresa
        ///            1 - Inserto la Empresa 
        ///            -10 - No Existe Licencia para esta empresa
        ///            -11 - No se pudo Modificar la licencia
        ///            -12 - No existe la empresa
        ///         -2 - Login invalido
        ///         -3 - Nombre invalido
        ///         -4 - Direccion invalida
        ///         -5 - Latitud invalida
        ///         -6 - Longitud invalida
        ///         -7 - Email invalido
        ///         -8 - Mes invalido
        ///         -9 - Nit invalido</returns>
        public int Modificar(int pID, int pIDClinica, string pLogin, string pNombre, string pDireccion,
                             string pLatitud, string pLongitud, string pemail, byte[] pimageLog,
                            string pDescripcion, string pTrabajos, string pNIT, int pmes, string pIDUsuario, String[] pTelefonos, int pIDIntervalo)
        {
            var sqlClinica = from c in dataContext.Clinica
                             where c.ID == pIDClinica
                             select c;

            if (sqlClinica.Count() > 0)
            {
                var sqlEmpresa = from e in dataContext.Empresa
                                 where e.ID == pID
                                 select e;

                if (sqlEmpresa.Count() > 0)
                {
                    var empresaMod = sqlEmpresa.First();
                    var clinicaMod = sqlClinica.FirstOrDefault();
                    empresaMod.Login = pLogin;
                    clinicaMod.Nombre = pNombre;
                    clinicaMod.Direccion = pDireccion;
                    decimal vLat = Decimal.Round(Convert.ToDecimal(pLatitud), 8);
                    decimal vLong = Decimal.Round(Convert.ToDecimal(pLongitud), 8);
                    clinicaMod.Latitud = vLat;
                    clinicaMod.Longitud = vLong;
                    empresaMod.Email = pemail;
                    //sql.First().Trabajos = pTrabajos;
                    clinicaMod.Descripcion = pDescripcion;
                    // sql.First().IDTiempoConsulta = pIDIntervalo;
                    if (pimageLog == null)
                    {
                        clinicaMod.ImagenLogo = null;
                    }
                    else
                    {
                        Binary vbi = new Binary(pimageLog);
                        clinicaMod.ImagenLogo = vbi;
                    }


                    empresaMod.NIT = pNIT;
                    try
                    {
                        dataContext.SubmitChanges();
                        ControlBitacora.Insertar("Se modifico un Consultorio", pIDUsuario);
                        //Modificar_Telefonos(pID, pTelefonos, pIDUsuario);
                        if (Convert.ToInt32(pmes) > 0)
                        {
                            return Ampliar_Licencia(pIDClinica, pmes, pIDUsuario);
                        }
                        else
                            return 1;
                    }
                    catch (Exception ex)
                    {
                        ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "Modificar", ex);
                        return 0;
                    }

                }
                else
                    return 4;
            }
            else
                return 4;

        }

        public static int EliminarClinica(int pIDClinica, string pIDUsuario)
        {
            var sqlClinica = (from c in dataContext.Clinica
                              where c.ID == pIDClinica
                              select c).FirstOrDefault();
            if (sqlClinica != null)
            {
                sqlClinica.Estado = false;
                var sqlConsultorio = (from c in dataContext.Empresa
                                      where c.IDClinica == pIDClinica
                                      select c).ToList();
                foreach (var consul in sqlConsultorio)
                {
                    consul.Estado = false;
                }
            }

            try
            {
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se Desactivo una clinica: " + pIDClinica, pIDUsuario);
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "Eliminar", ex);
                return 0;
            }


        }

        /// <summary>
        /// Permite desactivar una empresa
        /// </summary>
        /// <param name="pID">Id de la Empresa a desactivar</param>
        /// <param name="pIDUsuario">Id del Usuario Que realiza accion</param>
        /// <returns> 0 - Ocurrio un error No se elimino
        ///         1 - Se elimino Correctamente
        ///         2 - No Existe la Empresa  </returns>
        public static int Eliminar(int pID, string pIDUsuario)
        {
            var sql = from e in dataContext.Empresa
                      where e.ID == pID
                      select e;

            if (sql.Count() > 0)
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
                        EliminarClinica(empresas.FirstOrDefault().ID, pIDUsuario);
                    }
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se Desactivo una empresa: " + pID, pIDUsuario);
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
        /// Permite borrar fisicamente una empresa
        /// </summary>
        /// <param name="pIDEmpresa">ID de la empresa a eliminar</param>
        /// <param name="PIDUsuario">ID del usuario que realiza accion</param>
        /// <returns> 0 - No se pudo eliminar
        ///     1- Se elimino satisfactoriamente
        ///     2 - No existe esa empresa</returns>
        public int Eliminar_fisicamente(int pIDEmpresa, string PIDUsuario)
        {
            var sql = from e in dataContext.Empresa
                      where e.ID == pIDEmpresa
                      select e;
            if (sql.Count() > 0)
            {
                dataContext.Empresa.DeleteOnSubmit(sql.First());
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se elimino fisicamente un consultorio, sin confirmacion", PIDUsuario);
                    return 1;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "Eliminar_fisicamente", ex);
                    return 0;
                }
            }
            return 2;
        }

        /// <summary>
        /// Proceso que permite modificar las coordenadas de un consultorio
        /// </summary>
        /// <param name="pIDEmpresa">ID del consultorio</param>
        /// <param name="pLat">Latitud </param>
        /// <param name="pLong">Longitud</param>
        public void Modificar_lnglat(int pIDClinica, decimal pLat, decimal pLong)
        {
            var sql = from c in dataContext.Clinica
                      where c.ID == pIDClinica
                      select c;
            if (sql.Count() > 0)
            {
                sql.First().Latitud = pLat;
                sql.First().Longitud = pLong;
                dataContext.SubmitChanges();
            }
        }

        /// <summary>
        /// Proceso que verifica si la licencia de un consultorio esta vencida
        /// </summary>
        /// <param name="pIDEmpresa">ID del consultorio</param>
        /// <returns> TRUE - Vencida
        ///     - FAlse</returns>
        public bool Empresa_vencida(int pIDEmpresa)
        {

            DateTime datelc = Convert.ToDateTime(Get_Licenciap(pIDEmpresa).Rows[0][2].ToString());
            TimeSpan diff = datelc.Subtract(DateTime.Now.AddHours(Get_DirefenciaHora()));
            int dias = diff.Days + 1;
            if (dias <= 0 && dias > -3) /// controla 3 dias despued de vecido
                return true;
            else
                return false;
        }

        /// <summary>
        /// Devuelve en un STRING la licencia de un consultorio
        /// </summary>
        /// <param name="gIDEmpresa">ID del consultorio</param>
        /// <returns>string </returns>
        public static string Get_LicenciaString(int gIDClinica)
        {
            DataTable vDTLicencias = Get_Licenciap(gIDClinica);
            String vLicencia = "";
            int vCnt = vDTLicencias.Rows.Count;
            if (vCnt > 0)
            {
                foreach (DataRow lic in vDTLicencias.Rows)
                {
                    if (((DateTime)lic[2]).CompareTo(DateTime.Now.AddHours(Get_DirefenciaHora())) > 0 || vCnt == 1)
                    {
                        vLicencia = ((DateTime)lic[1]).ToShortDateString() + " - ";
                        break;
                    }
                }

                vLicencia = vLicencia + ((DateTime)vDTLicencias.Rows[vCnt - 1][2]).ToShortDateString();
            }
            return vLicencia;
        }

        public static int Get_DirefenciaHora()
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

        public static ConsultorioDto ObtenerConsultorioPorId(int idConsultorio)
        {
            return (from c in dataContext.Empresa
                    from tc in dataContext.Tiempo_Consulta
                    where c.ID == idConsultorio && tc.ID == c.IDIntervalo
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
                        Telefonos = GetTelefonosConsultorios(c.ID, c.IDClinica)
                    }).FirstOrDefault();
        }

        public int Get_IntervaloConsulta(int pIDEmpresa)
        {
            var sql = from tc in dataContext.Tiempo_Consulta
                      join e in dataContext.Empresa on tc.ID equals e.IDIntervalo
                      where e.ID == pIDEmpresa
                      select tc;
            if (sql.Count() > 0)
            {
                return (int)sql.First().Value;
            }
            return 60;
        }

        public static IEnumerable<Tiempo_Consulta> ObtenerIntervalosDeTiempo()
        {
            return from inte in dataContext.Tiempo_Consulta
                   select inte;
        }

        public static List<ConsultorioDto> GetConsultorios(int IDClinica)
        {
            return (from e in dataContext.Empresa
                    where e.IDClinica == IDClinica
                    select e).Select(x => new ConsultorioDto()
                    {
                        Email = x.Email,
                        Estado = x.Estado,
                        FechaCreacion = x.FechaCreacion,
                        FechaModificacion = x.FechaModificacion,
                        IDClinica = x.IDClinica,
                        IDConsultorio = x.ID,
                        IDIntervalo = x.IDIntervalo,
                        IDUsuarioCreador = x.IDUsuarioCreador,
                        Login = x.Login,
                        NIT = x.NIT,
                        Telefonos = GetTelefonosConsultorios(x.ID, IDClinica),
                        Trabajos = ObtenerTrabajosConsultorio(x.ID)
                    }).ToList();
        }

        private static List<TrabajosConsultorioDto> ObtenerTrabajosConsultorio(int pIdConsultorio)
        {
            return (from tc in dataContext.TrabajosConsultorio
                    where tc.IDConsultorio == pIdConsultorio
                    select tc).Select(x => new TrabajosConsultorioDto()
                    {
                        ID = x.IDTrabajo,
                        IDConsultorio = x.IDConsultorio,
                        State = 0
                    }).ToList();
        }

        public static List<TelefonoDto> GetTelefonosConsultorios(int idEmpresa, int idClinica)
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

        public static List<TelefonoDto> GetTelefonosClinica(int idClinica)
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

        public static int InsertarConsultorio(ConsultorioDto pConsultorio)
        {
            var idEmpresa = (from e in dataContext.Empresa
                             select e);
            int newID = idEmpresa == null ? 1 : idEmpresa.Max(x => x.ID) + 1;
            var consultorioToSave = new Empresa()
            {
                Email = pConsultorio.Email,
                Estado = true,
                FechaCreacion = DateTime.Today,
                FechaModificacion = DateTime.Today,
                ID = newID,
                IDClinica = pConsultorio.IDClinica,
                IDIntervalo = pConsultorio.IDIntervalo,
                IDUsuarioCreador = pConsultorio.IDUsuarioCreador,
                Login = pConsultorio.Login,
                NIT = pConsultorio.NIT
            };
            try
            {
                dataContext.Empresa.InsertOnSubmit(consultorioToSave);

                dataContext.SubmitChanges();

                ControlBitacora.Insertar("Se inserto un Consultorio", pConsultorio.IDUsuarioCreador);

                //activar_licencia(pConsultorio.IDClinica, 12, pConsultorio.IDUsuarioCreador);
                // int vRD = Crear_Roles_default(sql.First().ID, pIDUsuario);
                var IDrol = from r in dataContext.Rol
                            where r.Nombre == "Administrador Dentista"
                            && r.IDEmpresa == newID
                            select r;
                InsertarTelefonosConsultorio(pConsultorio.Telefonos, pConsultorio.IDUsuarioCreador);
                InsertarTrabajos(pConsultorio.Trabajos, newID, pConsultorio.IDUsuarioCreador);
                SMTP vSMTP = new SMTP();
                string vPass = Encriptador.Generar_Aleatoriamente();

                ABMUsuarioEmpleado.Insertar("Administrador", "admin", newID, IDrol.First().ID, vPass, vPass, true, pConsultorio.IDUsuarioCreador);
                dataContext.SubmitChanges();

                String vMensaje = "Bienvenido a Odontoweb, usted pertenece a la clinica" + pConsultorio.NombreClinica + " sus datos para ingresar\nal" +
                                    " sistema son:\nConsultorio : " + pConsultorio.Login + " \nUsuario:admin \nPassword: " + vPass + "." +
                                    "\nAl momento de ingresar se le pedira que actualice su contraseña."
                                    + "\nSaludos,\nOdontoweb";
                vSMTP.Datos_Mensaje(pConsultorio.Email, vMensaje, "Bienvenido a Odontoweb");
                vSMTP.Enviar_Mail();
                return 1;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "InsertarConsultorio", ex);
                return 0;
            }
        }

        public static void InsertarTelefonosConsultorio(List<TelefonoDto> pTelefonos, string IdUsuarioCreador)
        {
            if (pTelefonos == null) return;
            foreach (var ptelefono in pTelefonos)
            {
                var vTelefono = new Telefono();

                switch (ptelefono.State)
                {
                    case 1:
                        ABMTelefono.InsertarConsultorio(ptelefono);
                        ControlBitacora.Insertar("Se inserto un Telefono", IdUsuarioCreador);
                        break;
                    case 2:
                        ABMTelefono.Modificar(ptelefono.IDConsultorio, ptelefono.Telefono, ptelefono.Nombre, ptelefono.ID);
                        ControlBitacora.Insertar("Se Modifico un Telefono", IdUsuarioCreador);
                        break;
                    case 3:
                        vTelefono = (from t in dataContext.Telefono
                                     where t.ID == ptelefono.ID
                                     select t).FirstOrDefault();
                        if (vTelefono != null)
                        {
                            dataContext.Telefono.DeleteOnSubmit(vTelefono);
                            dataContext.SubmitChanges();
                            ControlBitacora.Insertar("Se dio de baja un Telefono", IdUsuarioCreador);
                        }
                        break;
                }

            }

        }

        public static void InsertarTelefonosClinica(List<TelefonoDto> pTelefonos, string IdUsuarioCreador, int pIDClinica)
        {
            if (pTelefonos == null) return;
            foreach (var ptelefono in pTelefonos)
            {
                var vTelefono = new Telefono();

                switch (ptelefono.State)
                {
                    case 1:
                        ABMTelefono.InsertarTelefonosDeLaClinica(pIDClinica, ptelefono.Telefono, ptelefono.Nombre);
                        ControlBitacora.Insertar("Se inserto un Telefono", IdUsuarioCreador);
                        break;
                    case 2:
                        ABMTelefono.Modificar(ptelefono.IDConsultorio, ptelefono.Telefono, ptelefono.Nombre, ptelefono.ID);
                        ControlBitacora.Insertar("Se Modifico un Telefono", IdUsuarioCreador);
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
                            ControlBitacora.Insertar("Se dio de baja un Telefono", IdUsuarioCreador);
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
            if (trabajosClinica.Count() > 0)
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
                        Consultorios = GetConsultorios(x.ID),
                        Trabajos = ObtenerTrabajosClinica(x.ID),
                        Status = 0,
                        Telefonos = GetTelefonosClinica(x.ID)
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
                        Consultorios = GetConsultorios(x.ID),
                        Trabajos = ObtenerTrabajosClinica(x.ID),
                        Status = 0,
                        Telefonos = GetTelefonosClinica(x.ID)
                    }).ToList();
        }

        /// <summary>
        /// Devuelve el logo de un consultorio
        /// </summary>
        /// <param name="pIDEmpresa">ID del consultorio</param>
        /// <returns>byte[] conteniendo la imagen, null si no hay imagen </returns>
        public byte[] Get_imagenes(int pIDClinica)
        {

            Clinica vClinica = Get_ClinicaByID(pIDClinica);
            if (vClinica != null)
            {
                return vClinica.ImagenLogo.ToArray();
            }
            return null;
        }

        /// <summary>
        /// Devuelve una empresa segun su ID
        /// </summary>
        /// <param name="pIDEmpresa">ID de la empresa</param>
        /// <returns>un DataTable con los datos de la empresa</returns>
        public static DataTable Get_Empresap(int pIDEmpresa)
        {
            return Converter<Empresa>.Convert(Get_Empresa(pIDEmpresa).ToList());
        }

        /// <summary>
        ///Devuelve los consultorios que esten disponibles segun fecha y hora
        /// </summary>
        /// <param name="pFecha"></param>
        /// <param name="vHorai"></param>
        /// <param name="pUsuario"></param>
        /// <returns></returns>
        public DataTable Buscar_Empresa_Horariop(DateTime pFecha, TimeSpan vHorai, String pUsuario)
        {
            int vnrodia = Convert.ToInt32(pFecha.DayOfWeek) - 1;
            if (vnrodia == -1)
                vnrodia = 6;
            return Converter<Buscar_Empresa_HorarioResult>.Convert(Buscar_Empresa_Horario(pFecha, vHorai, vnrodia, pUsuario).ToList());
        }

        /// <summary>
        /// Devuelve los ID's de todos los consultorios
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable Get_IDempresasp()
        {
            return Converter<getIDEmpresasResult>.Convert(Get_IDmpresas().ToList());
        }

        /// <summary>
        /// Devuelve todas las licencias 
        /// </summary>
        /// <returns>DataTable con las licencias</returns>
        public DataTable Get_Licenciasp()
        {
            return Converter<Licencia>.Convert(Get_Licencia().ToList());
        }

        /// <summary>
        /// Devuelve el consultorio DEFAULT
        /// </summary>
        /// <returns>DataTable con el consultorio</returns>
        public DataTable Buscar_default()
        {
            return Converter<Empresa>.Convert(Buscar_defaultI().ToList());
        }


        public static Clinica Get_ClinicaByID(int idClinica)
        {
            return (from c in dataContext.Clinica
                    where c.ID == idClinica
                    select c).FirstOrDefault();
        }

        public Empresa Get_Empresa_By_ID(int pIDEmpresa)
        {
            return (from e in dataContext.Empresa
                    where e.ID == pIDEmpresa
                    select e).FirstOrDefault();
        }

        #endregion

        #region Metodos Privados

        /// <summary>
        /// Permite Habilitar una empresa
        /// </summary>
        /// <param name="pID">ID de la empresa a Habilitar</param>
        /// <param name="pIDUsuario">ID del usuario que realiza accion</param>
        /// <returns> 1 - Habilito
        ///           0 - No Habilito
        ///           2 - No existe empresa </returns>
        private int Habilitar_Empresa(int pID, string pIDUsuario)
        {
            var vEmpresa = from e in dataContext.Empresa
                           where e.ID == pID
                           select e;
            if (vEmpresa.Count() > 0)
            {
                vEmpresa.First().Estado = true;
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se Habilito una empresa: " + pID, pIDUsuario);

                    return 1;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "Habilitar_Empresa", ex);
                    return 0;
                }
            }
            else
                return 2;

        }

        private static void InsertarTrabajosClinica(List<TrabajosClinicaDto> pTrabajos, string pIDUsuario, int pIDClinica)
        {
            if (pTrabajos == null) return;
            foreach (var ptrabajo in pTrabajos)
            {
                var vTrabajo = new Trabajos();

                switch (ptrabajo.State)
                {
                    case 1:
                        vTrabajo.Descripcion = ptrabajo.Descripcion;
                        vTrabajo.IDClinica = pIDClinica;
                        try
                        {
                            dataContext.Trabajos.InsertOnSubmit(vTrabajo);
                            ControlBitacora.Insertar("Se inserto un Trabajo", pIDUsuario);
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
                                ControlBitacora.Insertar("Se inserto un Trabajo", pIDUsuario);
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
                                ControlBitacora.Insertar("Se elimino un Trabajo", pIDUsuario);
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

        private static int GetNextIDClinica()
        {
            var clinicas = (from c in dataContext.Clinica
                            select c);
            return clinicas == null ? 1 : clinicas.Max(x => x.ID);
        }

        private static void InsertarTrabajos(List<TrabajosConsultorioDto> pTrabajos, int pConsultorioID, string pIDUsuarioCreador)
        {
            if (pTrabajos == null) return;
            foreach (var pTrabajo in pTrabajos)
            {
                var vTrabajoConsultorio = new TrabajosConsultorio();

                switch (pTrabajo.State)
                {
                    case 1:
                        vTrabajoConsultorio.IDConsultorio = pConsultorioID;
                        vTrabajoConsultorio.IDTrabajo = pTrabajo.ID;

                        dataContext.TrabajosConsultorio.InsertOnSubmit(vTrabajoConsultorio);
                        dataContext.SubmitChanges();
                        ControlBitacora.Insertar("Se inserto un TrabajoConsultorio", pIDUsuarioCreador);
                        break;

                    case 3:
                        vTrabajoConsultorio = (from t in dataContext.TrabajosConsultorio
                                               where t.IDTrabajo == pTrabajo.ID && t.IDConsultorio == pConsultorioID
                                               select t).FirstOrDefault();
                        if (vTrabajoConsultorio != null)
                        {
                            dataContext.TrabajosConsultorio.DeleteOnSubmit(vTrabajoConsultorio);
                            dataContext.SubmitChanges();
                            ControlBitacora.Insertar("Se dio de baja un Trabajo consultorio", pIDUsuarioCreador);
                        }
                        break;
                }

            }

        }

        /// <summary>
        /// Metodo privado que retorna los consultorios en los que esta inscrito un paciente
        /// </summary>
        /// <param name="pLoginUsuario">Login del paciente</param>
        /// <returns>  IEnumerable</returns>
        private IEnumerable<Empresa> Get_Empresas(String pLoginUsuario)
        {
            return from e in dataContext.Empresa
                   join ec in dataContext.Empresa_Cliente on e.ID equals ec.id_empresa
                   where e.Estado == true && e.ID != 1 && ec.id_usuariocliente == pLoginUsuario
                   select e;
        }

        /// <summary>
        /// Metodo privado que busca empresas segun nombre o login
        /// </summary>
        /// <param name="pLogin">Paremetro de busqueda</param>
        /// <returns>IEnumerable</returns>
        //private IEnumerable<Empresa> Buscar_Empresas(String pLogin)
        //{

        //    return from e in dataContext.Empresa
        //           where (e.Login.Contains(pLogin) || e.Nombre.Contains(pLogin)) && e.Estado == true
        //           && e.ID != 1
        //           orderby e.Nombre
        //           select e;

        //}

        /// <summary>
        /// Busca una empresa segun el parametro que reciba, busca por nombre y login
        /// </summary>
        /// <param name="pLogin">Parametro de busqueda</param>
        /// <returns>Un Datatable con los datos de las empresas encontradas</returns>
        //public DataTable Buscar_Empresasp(string pLogin)
        //{
        //    return Converter<Empresa>.Convert(Buscar_Empresas(pLogin).ToList());
        //}

        /// <summary>
        /// Metodo privado que devuelve empresas segun el parametro que reciba por nombre
        /// o login, excepto la default
        /// </summary>
        /// <param name="pLogin">Parametro de busqueda</param>
        /// <returns></returns>
        //private IEnumerable<Empresa> Buscar_EmpresasT(String pLogin)
        //{

        //    return from e in dataContext.Empresa
        //           where (e.Login.Contains(pLogin) || e.Nombre.Contains(pLogin)) &&
        //           !e.Login.Equals("DEFAULT")

        //           select e;

        //}

        /// <summary>
        /// Busca una empresa segun el parametro que reciba, busca por nombre y login
        /// excepto la default
        /// </summary>
        /// <param name="pLogin">Parametro de busqueda</param>
        /// <returns>Un Datatable con los datos de las empresas encontradas</returns>


        /// <summary>
        /// Metodo privado que retorna las licencias de un consultorio
        /// </summary>
        /// <param name="pIDEmpresa">ID del consultorio</param>
        /// <returns>IEnumerable</returns>
        private static IEnumerable<Licencia> Get_Licencia(int pIDClinica)
        {

            return from l in dataContext.Licencia
                   where l.IDClinica == pIDClinica
                   select l;

        }

        /// <summary>
        /// Metodo privado que retorna los telefonos de un consultorio
        /// </summary>
        /// <param name="pIDEmpresa">ID del consultorio</param>
        /// <returns>IEnumerable</returns>
        //private IEnumerable<Telefono> Get_Telefono(int pIDEmpresa)
        //{

        //    return from t in dataContext.Telefono
        //           where t.IDConsultorio == pIDEmpresa && t.Estado == true
        //           select t;

        //}

        /// <summary>
        /// Devuelve los numeros telefonicos de un consultorio
        /// </summary>
        /// <param name="pIDEmpresa">ID del consultorio</param>
        /// <returns>Retorna un Datable con los Telefonos</returns>
        //public DataTable Get_Telefonosp(int pIDEmpresa)
        //{
        //    return Converter<Telefono>.Convert(Get_Telefono(pIDEmpresa).ToList());
        //}

        /// <summary>
        /// Metodo privado que devuelve un consultorio 
        /// </summary>
        /// <param name="pIDEmpresa">ID consultorio</param>
        /// <returns>IEnumerable</returns>
        private static IEnumerable<Empresa> Get_Empresa(int pIDEmpresa)
        {

            return from e in dataContext.Empresa
                   where e.ID == pIDEmpresa
                   select e;

        }

        /// <summary>
        /// Metodo privado que retorna un consultorio segun Login
        /// </summary>
        /// <param name="pLogin">Login del consultorio</param>
        /// <returns>IEnumerable</returns>
        private IEnumerable<Empresa> Get_Empresa(string pLogin)
        {

            return from e in dataContext.Empresa
                   where e.Login == pLogin
                   select e;

        }

        /// <summary>
        /// Metodo privado que llama a un procedimiento almacenado 
        /// el cual retorna los consultorios que esten disponibles segun fecha y hora
        /// </summary>
        /// <param name="pFecha">Fecha disponible</param>
        /// <param name="vHorai">Hora inicio disponible</param>
        /// <param name="pNrodia">Numero del dia</param>
        /// <param name="pUsuario">Usuario que realiza</param>
        /// <returns>IEnumerable</returns>
        private IEnumerable<Buscar_Empresa_HorarioResult> Buscar_Empresa_Horario(DateTime pFecha, TimeSpan vHorai,
                                                           int pNrodia, string pUsuario)
        {
            return dataContext.Buscar_Empresa_Horario(pFecha, vHorai, pNrodia, pUsuario);
        }

        /// <summary>
        /// Metodo privado que retorna todas las licencias
        /// </summary>
        /// <returns>IEnumerable</returns>
        private IEnumerable<Licencia> Get_Licencia()
        {
            return from l in dataContext.Licencia
                   select l;
        }

        /// <summary>
        /// Metodo privado que devuelve el consultorio DEFAULT
        /// </summary>
        /// <returns>IEnumerable</returns>
        private IEnumerable<Empresa> Buscar_defaultI()
        {
            return from e in dataContext.Empresa
                   where e.Login == "DEFAULT"
                   select e;
        }

        /// <summary>
        /// Metodo privado que retorna los ID de todos los consultorios
        /// </summary>
        /// <returns>IEnumerable</returns>
        private IEnumerable<getIDEmpresasResult> Get_IDmpresas()
        {
            return dataContext.getIDEmpresas();
        }

        /// <summary>
        /// Crea la licencia a la nueva empresa
        /// </summary>
        /// <param name="pIDEmpresa">ID de la nueva empresa</param>
        /// <param name="pmes">Cantidad de meses de la licencia</param>
        /// <param name="pIDUsuario">Id del usuario que realiza accion</param>
        private static void activar_licencia(int pIDClinica, int pmes, string pIDUsuario)
        {
            Licencia vlicencia = new Licencia();
            vlicencia.IDClinica = pIDClinica;
            vlicencia.FechaInicio = DateTime.Now.AddHours(Get_DirefenciaHora());
            vlicencia.FechaFin = DateTime.Now.AddHours(Get_DirefenciaHora()).AddMonths(pmes);
            try
            {
                dataContext.Licencia.InsertOnSubmit(vlicencia);
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se activo una licencia para el consultorio " + pIDClinica, pIDUsuario);
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "ActivarLicencia", ex);
            }
        }

        /// <summary>
        /// Amplia Licencia de una empresa
        /// </summary>
        /// <param name="pID"> ID de la empresa para ampliar licencia</param>
        /// <param name="pmes">Cantidad de meses a ampliar</param>
        /// <param name="pIDUsuario">Id del usuario que realiza accion</param>
        /// <returns> 2 - Si la licencia no existe para esa empresa
        ///           3 - No se pudo modificar licencia 
        ///           1 - Se modifico correctamente la licencia</returns>
        private int Ampliar_Licencia(int pIDClinica, int pmes, string pIDUsuario)
        {
            var licencia = from l in dataContext.Licencia
                           where l.IDClinica == pIDClinica
                           orderby l.FechaInicio descending
                           select l;
            if (licencia.Count() > 0)
            {
                DateTime vfechaaux = DateTime.Now.AddHours(Get_DirefenciaHora());
                Licencia pNewLicencia = new Licencia();
                pNewLicencia.IDClinica = pIDClinica;
                if (vfechaaux <= licencia.First().FechaFin)//no se vencio
                {

                    vfechaaux = licencia.First().FechaFin.AddDays(1);

                    pNewLicencia.FechaInicio = vfechaaux;
                    pNewLicencia.FechaFin = vfechaaux.AddMonths(pmes);

                }
                else
                {
                    //  vfechaaux.AddDays(1);

                    pNewLicencia.FechaFin = vfechaaux.AddMonths(pmes);
                    pNewLicencia.FechaInicio = vfechaaux;
                }
                try
                {


                    dataContext.Licencia.InsertOnSubmit(pNewLicencia);
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se Amplio o renovo una licencia", pIDUsuario);
                    Enviar_Notificacion_LicenciaAmpliada(pIDClinica, pmes);
                    return Habilitar_Empresa(pIDClinica, pIDUsuario);

                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NLogin", "ABMEmpresa", "Modificar", ex);
                    return 3;
                }
            }
            else
                return 2;

        }

        /// <summary>
        /// Envia un correo a la empresa cuando su licencia ha sido ampliada
        /// </summary>
        /// <param name="pID">ID de la empresa</param>
        /// <param name="pmes">Numero de meses de ampliacion</param>
        private void Enviar_Notificacion_LicenciaAmpliada(int pIDEmpresa, int pmes)
        {
            Empresa vEmpresa = Get_Empresa_By_ID(pIDEmpresa);
            String vLincenciaS = Get_LicenciaString(vEmpresa.IDClinica);
            SMTP vsmtp = new SMTP();
            Clinica vClinica = Get_ClinicaByID(vEmpresa.IDClinica);

            String vMsj = vClinica.Nombre + " el motivo de este mensaje es notificarle que su lincencia \n " +
                        "ha sido ampliada por " + Convert.ToString(pmes) + " mes(es) mas.\n"
                        + "Por lo tanto su licencia actual es: " + vLincenciaS +
                        "\nSaludos\nMediweb\nPara mayor informacion comuniquese con nosotros: soporte@dacasys.com";
            vsmtp.Datos_Mensaje(vEmpresa.Email, vMsj, "Odontoweb - Renovacion de Licencia");
            vsmtp.Enviar_Mail();

        }

        #endregion
    }
}