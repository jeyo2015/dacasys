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
        public int IDHistorico { get; set; }
        public int IDPacienteSeleccionado { get; set; }
        public string IDCitaSeleccionada { get; set; }
        public int Licencia { get; set; }
    }

    public class CuentasPorCobrarDto
    {
        public int ID { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
        public int? IDTrabajo { get; set; }
        public string TrabajoDescripcion { get; set; }
        public string NombreConsultorio { get; set; }
        public decimal Saldo { get; set; }
        public int Estado { get; set; }
        public string Login { get; set; }
        public string NombreCliente { get; set; }
        public int IDConsultorio { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string EstadoShort { get; set; }
        public string EstadoFull { get; set; }
        public int? IDHistoricoFichaCab { get; set; }
        // public PacienteDto Cliente { get; set; }
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
    public class ValoresOdontogramaDto
    {
        public string T { get; set; }
        public string P { get; set; }
        public string Criterio { get; set; }
    }

    public class ValoresPeriodontalDto {
        public string ID { get; set; }
        public string Descripcion { get; set; }
    }
    public class ConsultorioDto
    {
        public int TipoLicencia { get; set; }
        public string Direccion { get; set; }
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
        public List<HorarioMapaDto> HorarioParaMapa { get; set; }
        public string NombreDoctor { get; set; }
    }

    public class TelefonoDto
    {
        public int ID { get; set; }
        public int IDConsultorio { get; set; }
        public int IDClinica { get; set; }
        public string Telefono { get; set; }
        public string Nombre { get; set; }
        public bool IsChecked { get; set; }
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
        public bool EstaEliminada { get; set; }
    }

    public class TrabajosConsultorioDto
    {
        public int ID { get; set; }
        public int IDConsultorio { get; set; }

        public int State { get; set; }
    }
    public class TrabajosDto
    {
        public int ID { get; set; }
        public string Descripcion { get; set; }

    }
    public class TrabajosClinicaDto
    {
        public int IDClinica { get; set; }
        public int ID { get; set; }
        public List<int> IDConsultorio { get; set; }
        public string Descripcion { get; set; }
        public int State { get; set; }
        public bool IsChecked { get; set; }
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
        public DateTime FechaInicioLicencia { get; set; }
        public string FechaInicioLicenciaString { get; set; }
        public DateTime FechaFinLicencia { get; set; }
        public int CantidadMeses { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int CantidadConsultorios { get; set; }
        public List<ConsultorioDto> Consultorios { get; set; }
        public bool Estado { get; set; }
        public List<TrabajosClinicaDto> Trabajos { get; set; }
        public List<TelefonoDto> Telefonos { get; set; }
        public int Status { get; set; }
        public int TipoLicencia { get; set; }
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

    public class NotificacionCitas
    {
        public int IDConsultorio { get; set; }
        public string Email { get; set; }
        public string IDCita { get; set; }
        public DateTime FechaCita { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public string FechaString { get; set; }
        public string HoraInicioStrig { get; set; }
        public string NombrePaciente { get; set; }
        public string loginCliente { get; set; }
        public string NombreConsultorio { get; set; }
        public string EmailPaciente { get; set; }
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
        public string EstadoString { get; set; }
        public List<HistoricoDetallePacienteDto> DetalleHistorico { get; set; }

    }

    public class HistoricoFichaTrabajoDto
    {
        public int ID;

        public int IDHistoricoFichaCab;

        public int Pieza;

        public string Descripcion;

        public decimal Costo;
        public bool Eliminar { get; set; }
    }

    public class HistoricoFichaDetalleDto
    {
        public int ID { get; set; }

        public int IDHistoricoFichaCab { get; set; }

        public System.DateTime Fecha { get; set; }

        public int Pieza { get; set; }

        public string TrabajoRealizado { get; set; }
        public bool Eliminar { get; set; }
    }
    public class HistoricoFichaCabDto
    {

        public List<HistoricoFichaTrabajoDto> FichaTrabajo { get; set; }
        public List<HistoricoFichaDetalleDto> FichaDetalle { get; set; }
        public int ID { get; set; }

        public int IDPaciente { get; set; }
        public int IDConsultorio { get; set; }
        public string Titulo { get; set; }

        public DateTime? Fecha { get; set; }

        public string Odontograma { get; set; }

        public bool? Estado { get; set; }
        public string EstadoString { get; set; }
        public HistoricoPacienteOdontogramaDto HistoricoPaciente { get; set; }
    }

    public class HistoricoPacienteOdontogramaDto
    {
        public int ID { get; set; }

        public int IdHistoricoFichaCab { get; set; }

        public System.Nullable<System.DateTime> FechaCreacion { get; set; }

        public string ApellidoPaterno { get; set; }

        public string ApellidoMaterno { get; set; }

        public string Nombre { get; set; }

        public int Edad { get; set; }

        public string Sexo { get; set; }

        public string LugarNacimiento { get; set; }

        public System.DateTime FechaNacimiento { get; set; }

        public string Ocupacion { get; set; }

        public string Direccion { get; set; }

        public string Telefono { get; set; }

        public string GradoInstruccion { get; set; }

        public string EstadoCivil { get; set; }

        public string NacionesOriginarias { get; set; }

        public string Idioma { get; set; }

        public string PersonaInformacion { get; set; }

        public string ApellidoPaternoPI { get; set; }

        public string ApellidoMaternoPI { get; set; }

        public string NombresPI { get; set; }

        public string DireccionPI { get; set; }

        public string TelefonoPI { get; set; }

        public string AntPatFam { get; set; }

        public System.Nullable<bool> Anemia { get; set; }

        public System.Nullable<bool> Cardiopatias { get; set; }

        public System.Nullable<bool> EnfGastricas { get; set; }

        public System.Nullable<bool> Hepatitis { get; set; }

        public System.Nullable<bool> Tuberculosis { get; set; }

        public System.Nullable<bool> Asma { get; set; }

        public System.Nullable<bool> DiabetesMel { get; set; }

        public System.Nullable<bool> Epilepsia { get; set; }

        public System.Nullable<bool> Hipertension { get; set; }

        public System.Nullable<bool> VIH { get; set; }

        public string OtrosAntP { get; set; }

        public System.Nullable<bool> Alergias { get; set; }

        public System.Nullable<int> Embarazo { get; set; }

        public string TratamientoMedico { get; set; }

        public string Medicamento { get; set; }

        public System.Nullable<int> HemorragiaExtDent { get; set; }

        public string ATM { get; set; }

        public string GangliosLinf { get; set; }

        public System.Nullable<int> Respirador { get; set; }

        public string OtrosExtOral { get; set; }

        public string Labios { get; set; }

        public string Lengua { get; set; }

        public string Paladar { get; set; }

        public string PisoBoca { get; set; }

        public string MucosaYugal { get; set; }

        public string Encias { get; set; }

        public System.Nullable<bool> isProtesisDental { get; set; }

        public System.Nullable<System.DateTime> FecUltVisita { get; set; }

        public System.Nullable<bool> isFuma { get; set; }

        public System.Nullable<bool> isBebe { get; set; }

        public string OtroHabitos { get; set; }

        public System.Nullable<bool> isCepilloDental { get; set; }

        public System.Nullable<bool> isHiloDental { get; set; }

        public System.Nullable<bool> isEnjuagueBucal { get; set; }

        public string FrecCepDent { get; set; }

        public System.Nullable<bool> isSangreEncias { get; set; }

        public System.Nullable<int> HigieneBucal { get; set; }

        public string Observaciones { get; set; }

        public string Sext17_16 { get; set; }

        public string Sext11 { get; set; }

        public string Sext26_27 { get; set; }

        public string Sext46_47 { get; set; }

        public string Sext31 { get; set; }

        public string Sext37_36 { get; set; }

        public System.Nullable<int> Cminuscula { get; set; }

        public System.Nullable<int> E { get; set; }

        public System.Nullable<int> Ominuscula { get; set; }

        public System.Nullable<int> TotalCEO { get; set; }

        public System.Nullable<int> Cmayuscula { get; set; }

        public System.Nullable<int> P { get; set; }

        public System.Nullable<int> Ei { get; set; }

        public System.Nullable<int> Omayuscula { get; set; }

        public System.Nullable<int> TotalCPO { get; set; }

        public System.Nullable<int> TotalPiezasSanas { get; set; }

        public System.Nullable<int> TotalPiezasDent { get; set; }
        public string FechaNacimientoString { get; set; }
        public string UltimaVisitaString { get; set; }
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

    public class HorarioMapa
    {
        public int IdDia { get; set; }
        public string NombreCorto { get; set; }
        public bool hasFriend { get; set; }
        public List<HorarioDto> Horarios { get; set; }
    }
    public class HorarioMapaDto
    {
        public List<HorarioDto> Horarios { get; set; }
        //public List<DiaDto> Dias { get; set; }
        public string Dias { get; set; }
    }
    public class HorarioDto
    {
        public TimeSpan HoraInicioSpan { get; set; }
        public TimeSpan HoraFinSpan { get; set; }
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
        public bool EstadoCita { get; set; }
        public bool Atendido { get; set; }
        public TimeSpan HoraInicioCita { get; set; }
        public bool NoDisponible { get; set; }
        public string EstadoMostrar { get; set; }
        public DateTime FechaCita { get; set; }
    }
}