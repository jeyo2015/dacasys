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
        public static String ObtenerDatosCliente(String pLogin)
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
                    }).FirstOrDefault().NombrePaciente;
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
                        Monto = p.Monto,
                        IDCuentasPorCobrar = pIDCuenta
                    }).ToList();
        }

        public static List<TrabajosDto> GetTrabajosConsultorio(int pIdConsultorio)
        {
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

                    select new CuentasPorCobrarDto
                    {
                        Descripcion = c.Descripcion,
                        Estado = c.Estado,
                        EstadoShort = GetEstadoShort(c.Estado),
                        EstadoFull = GetEstadoFull(c.Estado),
                        FechaCreacion = c.FechaRegistro,
                        ID = c.ID,
                        IDConsultorio = c.IDConsultorio,
                        IDTrabajo = c.IDTrabajo,
                        NombreCliente = ObtenerDatosCliente(c.Login),
                        Monto = c.Monto,
                        Saldo = c.Saldo,
                        Detalle = ObtenerDetalles(c.ID),
                        Login = c.Login
                    }).ToList();
        }

        private static string GetEstadoFull(int pEstado)
        {
            switch (pEstado)
            {
                case 0:
                    return "Pendiente";
                case 1:
                    return "Cancelado";
                case 2:
                    return "Anulado";
            }
            return "";
        }
        private static string GetEstadoShort(int pEstado)
        {
            switch (pEstado)
            {
                case 0:
                    return "PN";
                case 1:
                    return "CL";
                case 2:
                    return "AN";
            }
            return "";
        }
        public static List<CuentasPorCobrarDto> ObtenerCuentasPorPagarCliente(string pLogin)
        {


            return (from c in dataContext.CuentasPorCobrar
                    from t in dataContext.Trabajos
                    from e in dataContext.Empresa
                    from cl in dataContext.Clinica
                    where c.Login == pLogin
                    && t.ID == c.IDTrabajo
                    && e.ID == c.IDConsultorio
                    && cl.ID == e.IDClinica
                    && c.Estado != 2
                    select new CuentasPorCobrarDto
                    {
                        Descripcion = c.Descripcion,
                        Estado = c.Estado,
                        FechaCreacion = c.FechaRegistro,
                        ID = c.ID,
                        IDConsultorio = c.IDConsultorio,
                        IDTrabajo = c.IDTrabajo,
                        NombreCliente = ObtenerDatosCliente(c.Login),
                        Monto = c.Monto,
                        Saldo = c.Saldo,
                        Detalle = ObtenerDetalles(c.ID),
                        Login = c.Login,
                        TrabajoDescripcion = t.Descripcion,
                        NombreConsultorio = cl.Nombre,
                        EstadoShort = GetEstadoShort(c.Estado),
                        EstadoFull = GetEstadoFull(c.Estado)
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
                pago.First().Estado = pago.First().Saldo == 0? 1 : 0;
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

                var pago = from c in dataContext.CuentasPorCobrar
                           where c.ID == pPago.IDCuentasPorCobrar
                           select c;
                pago.First().Saldo = pago.First().Saldo + sql.First().Monto;
                sql.First().Monto = pPago.Monto;
                sql.First().Descripcion = pPago.Descripcion;
                pago.First().Saldo = pago.First().Saldo - pPago.Monto;
                pago.First().Estado = pago.First().Saldo == 0 ? 1 : 0;
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
                    var pago = from c in dataContext.CuentasPorCobrar
                               where c.ID == sql.FirstOrDefault().IDCuentaPorCobrar
                               select c;
                    pago.First().Saldo = pago.First().Saldo + sql.FirstOrDefault().Monto;
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
            vCuenta.Estado = 0;
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
                if (pCuenta.Monto > sql.First().Monto)
                    sql.First().Saldo = sql.First().Saldo + (pCuenta.Monto - sql.First().Monto);
                else
                {
                    if (pCuenta.Monto < sql.First().Monto)
                        sql.First().Saldo = sql.First().Saldo - (sql.First().Monto - pCuenta.Monto);
                }
                sql.First().Monto = pCuenta.Monto;
                //sql.First().Saldo = pCuenta.Saldo;
                try
                {
                    dataContext.SubmitChanges();
                    ControlBitacora.Insertar("Se modifico una cuenta", idUsuario);
                    return true;
                }
                catch (Exception ex)
                {
                    ControlLogErrores.Insertar("NAgenda", "ABMCuenta", "ModificarCuenta", ex);
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
                sql.First().Estado = 2;

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