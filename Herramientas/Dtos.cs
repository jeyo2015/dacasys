namespace Herramientas
{
    using System;
    using System.Collections.Generic;

    public class SessionDto
    {
        public string Nombre { get; set; }
        public string loginUsuario { get; set; }
        public int IDConsultorio { get; set; }
        public int IDClinica { get; set; }
        public bool IsDacasys { get; set; }
        public int Verificar { get; set; }
        public bool ChangePass { get; set; }
        public int IDRol { get; set; }
        public SessionPermisosDto Permisos { get; set; }
    }

    public class CuentasPorCobrarDto
    {
        public int ID { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
        public int IDTrabajo { get; set; }
        public decimal Saldo { get; set; }
        public int Estado { get; set; }
        public string Login { get; set; }
        public int IDConsultorio { get; set; }
        public DateTime FechaCreacion { get; set; }
        public PacienteDto Cliente { get; set; }
        public List<CuentasPorCobrarDetalleDto> Detalle { get; set; }
    }
    public class CuentasPorCobrarDetalleDto
    {
        public int ID { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int IDCuentasPorCobrar { get; set; }
    }
    public class SessionPermisosDto
    {
        public List<ModuloDto> Modulos { get; set; }
        public List<FormularioDto> Formularios { get; set; }
        public List<ComponenteDto> Componentes { get; set; }
    }

    public class ModuloDto
    {
        public int IdModulo { get; set; }
        public string NombreModulo { get; set; }
        public bool TienePermiso { get; set; }
    }

    public class FormularioDto
    {
        public int IdFormulario { get; set; }
        public string NombreFormulario { get; set; }
        public bool TienePermiso { get; set; }
    }

    public class ComponenteDto
    {
        public int IdComponente { get; set; }
        public string NombreComponente { get; set; }
        public bool TienePermiso { get; set; }
    }

    public class UsuarioDto
    {
        public string Nombre { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool? changepass { get; set; }
        public int IDEmpresa { get; set; }
        public int IDRol { get; set; }
        public string ConfirmPass { get; set; }
        public bool Estado { get; set; }
    }

    public class ConsultorioDto
    {
        public bool EsConsultorioDefault { get; set; }
        public int IDConsultorio { get; set; }
        public int TiempoCita { get; set; }
        public string Login { get; set; }
        public string NombreClinica { get; set; }
        public string NIT { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string IDUsuarioCreador { get; set; }
        public bool Estado { get; set; }
        public string Email { get; set; }
        public int? IDIntervalo { get; set; }
        public int IDClinica { get; set; }
        public int State { get; set; }
        public List<TelefonoDto> Telefonos { get; set; }
        public List<TrabajosConsultorioDto> Trabajos { get; set; }
    }

    public class TelefonoDto
    {
        public int ID { get; set; }
        public int IDConsultorio { get; set; }
        public int IDClinica { get; set; }
        public string Telefono { get; set; }
        public string Nombre { get; set; }
        public int State { get; set; }
    }

    public class AgendaDto
    {
        public int IDHorario { get; set; }
        public int IDConsultorio { get; set; }
        public string LoginCliente { get; set; }
        public string IdCita { get; set; }
        public TimeSpan HoraFin { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public string HoraInicioString { get; set; }
        public string HoraFinString { get; set; }
        public bool EstaOcupada { get; set; }
        public bool Estalibre { get; set; }
        public bool EstaAtendida { get; set; }
        public PacienteDto Paciente { get; set; }
        public int NumeroCita { get; set; }
        public bool EsTarde { get; set; }
    }

    public class TrabajosConsultorioDto
    {
        public int ID { get; set; }
        public int IDConsultorio { get; set; }

        public int State { get; set; }
    }

    public class TrabajosClinicaDto
    {
        public int IDClinica { get; set; }
        public int ID { get; set; }
        public List<int> IDConsultorio { get; set; }
        public string Descripcion { get; set; }
        public int State { get; set; }
    }

    public class ClinicaDto
    {
        public bool EsConsultorioDefault { get; set; }
        public int IDClinica { get; set; }
        public string Nombre { get; set; }
        public string Login { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public byte[] logoImagen { get; set; }
        public string LogoParaMostrar { get; set; }
        public string NombreArchivo { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public string Nit { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int CantidadConsultorios { get; set; }
        public List<ConsultorioDto> Consultorios { get; set; }
        public bool Estado { get; set; }
        public List<TrabajosClinicaDto> Trabajos { get; set; }
        public List<TelefonoDto> Telefonos { get; set; }
        public int Status { get; set; }
    }

    public class ModulosTree
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public bool IsChecked { get; set; }
        public List<ModulosTree> Hijos { get; set; }
        public bool IsCollapsed { get; set; }
    }

    public class NotificacionesConsultorioDto
    {
        public int IDNotificacion { get; set; }
        public int IDConsultorio { get; set; }
        public string NombreUsuario { get; set; }
        public int TipoNotificacion { get; set; }
        public string LoginUsuario { get; set; }
        public DateTime FechaNotificacion { get; set; }
        public int EstadoNotificacion { get; set; }
    }

    public class NotificacionesConsultorioNewDto
    {
        public List<NotificacionesConsultorioDto> Notificaciones { get; set; }
        public int CantidadNuevasNotificaciones { get; set; }
    }

    public class RolDto
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public int IDEmpresa { get; set; }
    }

    public class TiempoConsultaDto
    {
        public int ID { get; set; }
        public string Descripcion { get; set; }
        public int Value { get; set; }
    }

    public class PacienteDto
    {
        public string LoginCliente { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NombrePaciente { get; set; }
        public string Ci { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public string TipoSangre { get; set; }
        public string Sexo { get; set; }
        public bool Estado { get; set; }
        public string Antecedentes { get; set; }
        public int IdPaciente { get; set; }
        public int IDEmpresa { get; set; }
        public bool IsPrincipal { get; set; }
        public string Password { get; set; }
    }

    public class HistoricoPacienteDto
    {
        public int IdConsultorio { get; set; }
        public int IdPaciente { get; set; }
        public int NumeroHistorico { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int EstimacionCitas { get; set; }
        public int CitasRealizadas { get; set; }
        public bool Estado { get; set; }
        public string TituloHistorico { get; set; }
        public int EstadoABM { get; set; }
        public List<HistoricoDetallePacienteDto> DetalleHistorico { get; set; }
    }

    public class HistoricoDetallePacienteDto
    {
        public int IdConsultorio { get; set; }
        public int IdPaciente { get; set; }
        public int NumeroHistorico { get; set; }
        public int NumeroDetalle { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string TrabajoRealizado { get; set; }
        public string TrabajoARealizar { get; set; }
        public string IdCita { get; set; }
        public bool CerrarHistorico { get; set; }
    }

    public class HorarioDto
    {
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public int NumHorario { get; set; }
        public int IDEmpresa { get; set; }
        public int IDDia { get; set; }
        public int IDHorario { get; set; }
        public bool Estado { get; set; }
        public string NombreDia { get; set; }
    }

    public class DiaDto
    {
        public int IDDia { get; set; }
        public string NombreDia { get; set; }
        public string NombreCorto { get; set; }
        public bool IsChecked { get; set; }
    }

    public class ComentarioDto
    {
        public string Comentario { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string LoginCliente { get; set; }
        public int IDEmpresa { get; set; }
        public int IDComentario { get; set; }
        public bool IsVisible { get; set; }
        public string NombrePaciente { get; set; }
    }

    public class EmpresaClinicaDto
    {
        public int IDClinica { get; set; }
        public string Nombre { get; set; }
        public string Login { get; set; }
        public string LoginCliente { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public byte[] LogoImagen { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public string Nit { get; set; }
        public bool Estado { get; set; }
        public int Status { get; set; }
        public int IDEmpresa { get; set; }
        public int TiempoCita { get; set; }
        public string Email { get; set; }
    }

    public class CitasDelClienteDto
    {
        public int IDConsultorio { get; set; }
        public string LoginCliente { get; set; }
        public string IdCita { get; set; }
        public string FechaString { get; set; }
        public string HoraInicioString { get; set; }
        public string HoraFinString { get; set; }
        public string NombreConsultorio { get; set; }
        public string NombrePaciente { get; set; }
    }
}