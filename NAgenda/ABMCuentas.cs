namespace NAgenda
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using NEventos;
    using NLogin;
    using Herramientas;
    using Datos;
    using System.Data.Linq;

    public class ABMCuenta
    {
        #region VariableGlobales

        readonly static ContextoDataContext dataContext = new ContextoDataContext();

        #endregion

        #region Metodos Publicos
        public static PacienteDto ObtenerDatosCliente(String pLogin)
        {
            return (from clientePaciente in dataContext.Cliente_Paciente
                    from paciente in dataContext.Paciente
                    where clientePaciente.id_usuariocliente == pLogin
                    && clientePaciente.id_paciente == paciente.id_paciente
                    && clientePaciente.IsPrincipal == true
                    && paciente.estado
                    select new PacienteDto()
                    {
                        Antecedentes = paciente.antecedente,
                        Ci = paciente.ci,
                        Direccion = paciente.direccion,
                        Email = paciente.email,
                        Estado = paciente.estado,
                        LoginCliente = pLogin,
                        NombrePaciente = paciente.nombre + " " + paciente.apellido,
                        Nombre = paciente.nombre,
                        Apellido = paciente.apellido,
                        Telefono = paciente.nro_telefono,
                        TipoSangre = paciente.tipo_sangre,
                        IdPaciente = paciente.id_paciente,
                        Sexo = paciente.sexo.ToString(),
                        IsPrincipal = clientePaciente.IsPrincipal ?? false
                    }).FirstOrDefault();
        }
        public static List<CuentasPorCobrarDetalleDto> ObtenerDetalles(int pIDCuenta)
        {
            return (from p in dataContext.Pago
                    where p.IDCuentaPorCobrar == pIDCuenta
                    select new CuentasPorCobrarDetalleDto
                    {
                        Descripcion = p.Descripcion,
                        FechaCreacion = p.FechaPago,
                        ID = p.ID,
                        Monto = p.Monto
                    }).ToList();
        }

        public static List<TrabajosDto> GetTrabajosConsultorio(int pIdConsultorio) {
            return (from tc in dataContext.TrabajosConsultorio
                    from t in dataContext.Trabajos
                    where t.ID == tc.IDTrabajo
                    && tc.IDConsultorio == pIdConsultorio
                    select new TrabajosDto
                    {
                        Descripcion = t.Descripcion,
                        ID = t.ID
                    }).ToList();
        }
        public static List<CuentasPorCobrarDto> ObtenerCuentasPorCobrarPorConsultorio(int pConsultorio)
        {
            return (from c in dataContext.CuentasPorCobrar

                    where c.IDConsultorio == pConsultorio
                    && c.Estado == 1
                    select new CuentasPorCobrarDto
                    {
                        Descripcion = c.Descripcion,
                        Estado = c.Estado,
                        FechaCreacion = c.FechaRegistro,
                        ID = c.ID,
                        IDConsultorio = c.IDConsultorio,
                        IDTrabajo = c.IDTrabajo,
                        Cliente = ObtenerDatosCliente(c.Login),
                        Monto = c.Monto,
                        Saldo = c.Saldo,
                        Detalle = ObtenerDetalles(c.ID)
                    }).ToList();
        }

        public static bool InsertarUnPago(CuentasPorCobrarDetalleDto pPago, string pIdUsuario)
        {
            var vPago = new Pago();
            vPago.Monto = pPago.Monto;
            vPago.IDCuentaPorCobrar = pPago.IDCuentasPorCobrar;
            vPago.FechaPago = DateTime.Now;
            vPago.Descripcion = pPago.Descripcion;
            dataContext.Pago.InsertOnSubmit(vPago);
            try
            {
                var pago = from c in dataContext.CuentasPorCobrar
                          where c.ID == pPago.IDCuentasPorCobrar
                          select c;
                pago.First().Saldo = pago.First().Saldo - pPago.Monto;
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se inserto un pago", pIdUsuario);
                return true;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NAgenda", "ABMCuentas", "Insertar un pago", ex);
                return false;
            }
        }
        public static bool ModificarPago(CuentasPorCobrarDetalleDto pPago, string idUsuario)
        {
            var sql = from p in dataContext.Pago
                      where p.ID == pPago.ID
                      select p;

            if (sql.Any())
            {
                sql.First().Monto = pPago.Monto;
                sql.First().Descripcion = pPago.Descripcion;
                  try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se modifico pago", idUsuario);
                    return true;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NAgenda", "ABMCuenta", "Modificar Pago", ex);
                    return false;
                }
            }
            return false;
        }

        public static bool EliminarPago(int pIdPago, string idUsuario)
        {
            var sql = from p in dataContext.Pago
                      where p.ID == pIdPago
                      select p;

            if (sql.Any())
            {
                dataContext.Pago.DeleteOnSubmit(sql.FirstOrDefault());
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se elimino un pago", idUsuario);
                    return true;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NAgenda", "ABMCuenta", "Eliminar Pago", ex);
                    return false;
                }
            }
            return false;
        }
        public static bool InsertarUnaCuenta(CuentasPorCobrarDto pCuenta, string pIdUsuario)
        {
            var vCuenta = new CuentasPorCobrar();
            vCuenta.Saldo = pCuenta.Monto;
            vCuenta.Monto = pCuenta.Monto;
            vCuenta.IDTrabajo = pCuenta.IDTrabajo;
            vCuenta.Login = pCuenta.Login;
            vCuenta.IDConsultorio = pCuenta.IDConsultorio;
            vCuenta.FechaRegistro = DateTime.Now;
            vCuenta.Descripcion = pCuenta.Descripcion;
            vCuenta.Estado = 1;
            dataContext.CuentasPorCobrar.InsertOnSubmit(vCuenta);
            try
            {
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se inserto una cuenta", pIdUsuario);
                return true;
            }
            catch (Exception ex)
            {
                ControlLogErrores.Insertar("NAgenda", "ABMCuentas", "InsertarUnaCuenta", ex);
                return false;
            }
        }
        public static bool ModificarCuenta(CuentasPorCobrarDto pCuenta, string idUsuario)
        {
            var sql = from c in dataContext.CuentasPorCobrar
                      where c.ID == pCuenta.ID
                      select c;

            if (sql.Any())
            {
                sql.First().Descripcion = pCuenta.Descripcion;
                sql.First().IDTrabajo = pCuenta.IDTrabajo;
                sql.First().Login = pCuenta.Login;
                sql.First().Monto = pCuenta.Monto;
                sql.First().Saldo = pCuenta.Saldo;
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se elimino una cuenta", idUsuario);
                    return true;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NAgenda", "ABMCuenta", "Modificar Pago", ex);
                    return false;
                }
            }
            return false;
        }

        public static bool EliminarCuentaPorCobrar(int pIdCuenta, string idUsuario)
        {
            var sql = from c in dataContext.CuentasPorCobrar
                      where c.ID == pIdCuenta
                      select c;

            if (sql.Any())
            {
                dataContext.CuentasPorCobrar.DeleteOnSubmit(sql.First());
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se elimino una cuenta", idUsuario);
                    return true;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NAgenda", "ABMCuenta", "EliminarCuentaPorCobrar", ex);
                    return false;
                }
            }
            return false;
        }
        #endregion


    }
}