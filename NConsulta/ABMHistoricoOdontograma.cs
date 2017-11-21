namespace NConsulta
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NEventos;
    using Datos;
    using Herramientas;
    using System.Globalization;

    public class ABMHistoricoOdontograma
    {
        #region VariableGlobales

        readonly static ContextoDataContext dataContext = new ContextoDataContext();

        #endregion
        #region MetodosPublicos
        public static HistoricoFichaCabDto ObtenerHistoricoFichaCab(int pdIDHistoricoCab)
        {

            var historicoFicha = (from f in dataContext.HistoricoFichaCab
                                  where f.ID == pdIDHistoricoCab
                                  select new HistoricoFichaCabDto()
                                  {
                                      Estado = f.Estado,
                                      Fecha = f.Fecha,
                                      Odontograma = f.Odontograma,
                                      Titulo = f.Titulo,
                                      ID = f.ID
                                  }).FirstOrDefault();
            if (historicoFicha == null) return null;


            historicoFicha.FichaTrabajo = ObtenerHistoricoFichaTrabajo(pdIDHistoricoCab);
            historicoFicha.FichaDetalle = ObtenerHistoricoFichaDetalle(pdIDHistoricoCab);
            historicoFicha.HistoricoPaciente = ObtenerHistoricoPaciente(pdIDHistoricoCab);
            return historicoFicha;
        }

        public static List<HistoricoFichaCabDto> ObtenerHistoricoFichaCabs(int idPaciente, int idConsultorio)
        {
            var fichasCab = (from f in dataContext.HistoricoFichaCab
                             where f.IdConsultorio == idConsultorio
                             && f.IDPaciente == idPaciente
                             select new HistoricoFichaCabDto
                             {
                                 Estado = f.Estado,
                                 Fecha = f.Fecha,
                                 Odontograma = f.Odontograma,
                                 Titulo = f.Titulo,
                                 ID = f.ID,
                                 EstadoString = f.Estado.Value ? "Abierto" : "Cerrado"
                             }).ToList();
            return fichasCab;
        }
        public static List<HistoricoFichaTrabajoDto> ObtenerHistoricoFichaTrabajo(int pdIDHistoricoCab)
        {
            return (from ft in dataContext.HistoricoFichaTrabajo
                    where ft.IDHistoricoFichaCab == pdIDHistoricoCab
                    select new HistoricoFichaTrabajoDto()
                    {
                        Costo = ft.Costo,
                        Descripcion = ft.Descripcion,
                        ID = ft.ID,
                        IDHistoricoFichaCab = ft.IDHistoricoFichaCab,
                        Pieza = ft.Pieza,
                        Eliminar = false
                    }).ToList();
        }

        public static List<HistoricoFichaDetalleDto> ObtenerHistoricoFichaDetalle(int pdIDHistoricoCab)
        {
            return (from fd in dataContext.HistoricoFichaDet
                    where fd.IDHistoricoFichaCab == pdIDHistoricoCab
                    select new HistoricoFichaDetalleDto()
                    {
                        Fecha = fd.Fecha,
                        ID = fd.ID,
                        IDHistoricoFichaCab = pdIDHistoricoCab,
                        Pieza = fd.Pieza,
                        TrabajoRealizado = fd.TrabajoRealizado,
                        Eliminar = false
                    }).ToList();
        }
        //  public static List<HistoricoFichaTrabajoDto> ObtenerTratamientoFichaCab() { }
        public static bool GuardarHistoricoCompleto(HistoricoFichaCabDto fichaHistorico, string idUsuario)
        {
            var fichaBD = (from fh in dataContext.HistoricoFichaCab
                           where fichaHistorico.ID == fh.ID
                           select fh).FirstOrDefault();
            if (fichaBD == null) return false;
            fichaBD.Odontograma = fichaHistorico.Odontograma;

            try
            {
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se insertó una nueva ficha en el historico", idUsuario);
                return true;
            }
            catch (Exception ex)
            {

                ControlLogErrores.Insertar("NConsulta", "ABMHistoricoOdontograma", "GuardarHistoricoFichaPaciente", ex);
                return false;
            }

        }
        public static bool GuardarTrabajosRealizadosHistorico(List<HistoricoFichaDetalleDto> detalles, string idUsuario, bool citaAtendida, string idCita)
        {
            if (detalles != null)
                foreach (var d in detalles)
                {
                    if (d.ID == 0 && !d.Eliminar)
                    {
                        var nuevoDetalle = new HistoricoFichaDet()
                        {
                            TrabajoRealizado = d.TrabajoRealizado,
                            Pieza = d.Pieza,
                            IDHistoricoFichaCab = d.IDHistoricoFichaCab,
                            Fecha = DateTime.Now
                        };
                        dataContext.HistoricoFichaDet.InsertOnSubmit(nuevoDetalle);
                    }
                    else
                    {
                        var detalleBD = (from dt in dataContext.HistoricoFichaDet
                                         where dt.ID == d.ID
                                         select dt).FirstOrDefault();
                        if (detalleBD != null)
                        {
                            if (d.Eliminar)
                            {
                                dataContext.HistoricoFichaDet.DeleteOnSubmit(detalleBD);
                            }
                            else
                            {

                                detalleBD.Pieza = d.Pieza;
                                detalleBD.TrabajoRealizado = d.TrabajoRealizado;

                            }
                        }

                    }
                }
            if (citaAtendida)
            {
                var citaBD = (from c in dataContext.Cita
                              where c.idcita == idCita
                              select c).FirstOrDefault();
                if (citaBD != null)
                    citaBD.atendido = true;

            }
            try
            {
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se insertó una nueva ficha en el historico", idUsuario);
                return true;
            }
            catch (Exception ex)
            {

                ControlLogErrores.Insertar("NConsulta", "ABMHistoricoOdontograma", "GuardarHistoricoFichaPaciente", ex);
                return false;
            }
        }
        public static bool GuardarTratamientoHistorico(List<HistoricoFichaTrabajoDto> trabajos, string idUsuario)
        {

            if (trabajos != null)
                foreach (var t in trabajos)
                {
                    if (t.ID == 0 && !t.Eliminar)
                    {
                        var nuevoTrabajo = new HistoricoFichaTrabajo()
                        {
                            Pieza = t.Pieza,
                            IDHistoricoFichaCab = t.IDHistoricoFichaCab,
                            Descripcion = t.Descripcion,
                            Costo = t.Costo
                        };
                        dataContext.HistoricoFichaTrabajo.InsertOnSubmit(nuevoTrabajo);

                    }
                    else
                    {

                        var trabajoBD = (from ft in dataContext.HistoricoFichaTrabajo
                                         where ft.ID == t.ID
                                         select ft).FirstOrDefault();
                        if (trabajoBD != null)
                        {
                            if (!t.Eliminar)
                            {
                                trabajoBD.Pieza = t.Pieza;
                                trabajoBD.Descripcion = t.Descripcion;
                                trabajoBD.Costo = t.Costo;
                            }
                            else
                                dataContext.HistoricoFichaTrabajo.DeleteOnSubmit(trabajoBD);
                        }
                    }
                }
            try
            {
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se insertó los trabajos de la ficha", idUsuario);
                return true;
            }
            catch (Exception ex)
            {

                ControlLogErrores.Insertar("NConsulta", "ABMHistoricoOdontograma", "GuardarHistoricoFichaPaciente", ex);
                return false;
            }
        }
        public static bool CerrarFichaHistorico(int idFichaHistorico, bool citaAtendida, string idCita, string idUsuario)
        {
            var fichaBD = (from f in dataContext.HistoricoFichaCab
                           where f.ID == idFichaHistorico
                           select f).FirstOrDefault();
            if (fichaBD == null) return true;
            fichaBD.Estado = false;
            if (citaAtendida)
            {
                var citaBD = (from c in dataContext.Cita
                              where c.idcita == idCita
                              select c).FirstOrDefault();
                if (citaBD != null)
                    citaBD.atendido = true;

            }
            try
            {
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se cerro la ficha", idUsuario);
                return true;
            }
            catch (Exception ex)
            {

                ControlLogErrores.Insertar("NConsulta", "ABMHistoricoOdontograma", "CerrarFichaHistorico", ex);
                return false;
            }

        }

        public static int GuardarHistoricoFichaPaciente(HistoricoFichaCabDto fichaHistorico, string idUsuario)
        {
            var pacienteDB = (from p in dataContext.Paciente
                              where p.id_paciente == fichaHistorico.IDPaciente
                              select p).FirstOrDefault();
            if (pacienteDB == null) return 0;
            var historicoDB = (from f in dataContext.HistoricoFichaCab
                             from h in dataContext.HistoricoPaciente
                             where h.IdHistoricoFichaCab == f.ID && 
                             f.IDPaciente == fichaHistorico.IDPaciente && f.IdConsultorio== fichaHistorico.IDConsultorio
                             select h).OrderByDescending(o=> o.ID).FirstOrDefault();
            var fichaHistoricoParaGuardar = new HistoricoFichaCab
            {
                Titulo = fichaHistorico.Titulo,
                Odontograma = "[[{\"Espacio\":50,\"Numero\":18,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":17,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":16,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":15,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":14,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":13,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":12,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":11,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":21,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":22,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":23,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":24,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":25,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":26,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":27,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":28,\"Estado\":1,\"Defectos\":[]}],[{\"Espacio\":176,\"Numero\":55,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":54,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":53,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":52,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":51,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":61,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":62,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":63,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":64,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":65,\"Estado\":1,\"Defectos\":[]}],[{\"Espacio\":176,\"Numero\":85,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":84,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":83,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":82,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":81,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":71,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":72,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":73,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":74,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":176,\"Numero\":75,\"Estado\":1,\"Defectos\":[]}],[{\"Espacio\":50,\"Numero\":48,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":47,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":46,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":45,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":44,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":43,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":42,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":41,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":31,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":32,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":33,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":34,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":35,\"Estado\":1,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":36,\"Estado\":26,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":37,\"Estado\":27,\"Defectos\":[]},{\"Espacio\":50,\"Numero\":38,\"Estado\":28,\"Defectos\":[]}]]",
                IdConsultorio = fichaHistorico.IDConsultorio,
                IDPaciente = fichaHistorico.IDPaciente,

                Fecha = DateTime.Now,
                Estado = true
            };

            try
            {
                ControlBitacora.Insertar("Se insertó una nueva ficha en el historico", idUsuario);
                dataContext.HistoricoFichaCab.InsertOnSubmit(fichaHistoricoParaGuardar);
                dataContext.SubmitChanges();
                var apellidos = pacienteDB.apellido.Split(' ');
                var historico = new HistoricoPaciente();
                if (historicoDB == null)
                {
                    historico = new HistoricoPaciente()
                   {
                       IdHistoricoFichaCab = fichaHistoricoParaGuardar.ID,
                       Nombre = pacienteDB.nombre,
                       ApellidoPaterno = apellidos.Count() > 0 ? apellidos[0] : "",
                       ApellidoMaterno = apellidos.Count() > 1 ? apellidos[1] : "",
                       FechaNacimiento = DateTime.Now,
                       FechaCreacion = DateTime.Now,
                       Sexo = pacienteDB.sexo.ToString()
                   };
                }
                else
                {
                     historico = new HistoricoPaciente()
                    {
                        Alergias = historicoDB.Alergias,
                        Anemia = historicoDB.Anemia,
                        AntPatFam = historicoDB.AntPatFam,
                        ApellidoMaterno = historicoDB.ApellidoMaterno,
                        ApellidoMaternoPI = historicoDB.ApellidoMaternoPI,
                        ApellidoPaterno = historicoDB.ApellidoPaterno,
                        ApellidoPaternoPI = historicoDB.ApellidoPaternoPI,
                        Asma = historicoDB.Asma,
                        ATM = historicoDB.ATM,
                        Cardiopatias = historicoDB.Cardiopatias,
                        Cmayuscula = historicoDB.Cmayuscula,
                        Cminuscula = historicoDB.Cminuscula,
                        DiabetesMel = historicoDB.DiabetesMel,
                        Direccion = historicoDB.Direccion,
                        DireccionPI = historicoDB.DireccionPI,
                        E = historicoDB.E,
                        Edad = historicoDB.Edad,
                        Ei = historicoDB.Ei,
                        Embarazo = historicoDB.Embarazo,
                        Encias = historicoDB.Encias,
                        EnfGastricas = historicoDB.EnfGastricas,
                        Epilepsia = historicoDB.Epilepsia,
                        EstadoCivil = historicoDB.EstadoCivil,
                        FechaCreacion = DateTime.Now,
                        FechaNacimiento = historicoDB.FechaNacimiento,
                        FecUltVisita = historicoDB.FecUltVisita,
                        FrecCepDent = historicoDB.FrecCepDent,
                        GangliosLinf = historicoDB.GangliosLinf,
                        GradoInstruccion = historicoDB.GradoInstruccion,
                        HemorragiaExtDent = historicoDB.HemorragiaExtDent,
                        Hepatitis = historicoDB.Hepatitis,
                        HigieneBucal = historicoDB.HigieneBucal,
                        Hipertension = historicoDB.Hipertension,
                      //  ID = historicoDB.ID,
                        IdHistoricoFichaCab = fichaHistoricoParaGuardar.ID,
                        Idioma = historicoDB.Idioma,
                        isBebe = historicoDB.isBebe,
                        isCepilloDental = historicoDB.isCepilloDental,
                        isEnjuagueBucal = historicoDB.isEnjuagueBucal,
                        isFuma = historicoDB.isFuma,
                        isHiloDental = historicoDB.isHiloDental,
                        isProtesisDental = historicoDB.isProtesisDental,
                        isSangreEncias = historicoDB.isSangreEncias,
                        Labios = historicoDB.Labios,
                        Lengua = historicoDB.Lengua,
                        LugarNacimiento = historicoDB.LugarNacimiento,
                        Medicamento = historicoDB.Medicamento,
                        MucosaYugal = historicoDB.MucosaYugal,
                        NacionesOriginarias = historicoDB.NacionesOriginarias,
                        Nombre = historicoDB.Nombre,
                        NombresPI = historicoDB.NombresPI,
                        Observaciones = historicoDB.Observaciones,
                        Ocupacion = historicoDB.Ocupacion,
                        Omayuscula = historicoDB.Omayuscula,
                        Ominuscula = historicoDB.Ominuscula,
                        OtroHabitos = historicoDB.OtroHabitos,
                        OtrosAntP = historicoDB.OtrosAntP,
                        OtrosExtOral = historicoDB.OtrosExtOral,
                        P = historicoDB.P,
                        Paladar = historicoDB.Paladar,
                        PersonaInformacion = historicoDB.PersonaInformacion,
                        PisoBoca = historicoDB.PisoBoca,
                        Respirador = historicoDB.Respirador,
                        Sexo = historicoDB.Sexo,
                        Sext11 = historicoDB.Sext11,
                        Sext17_16 = historicoDB.Sext17_16,
                        Sext26_27 = historicoDB.Sext26_27,
                        Sext31 = historicoDB.Sext31,
                        Sext37_36 = historicoDB.Sext37_36,
                        Sext46_47 = historicoDB.Sext46_47,
                        Telefono = historicoDB.Telefono,
                        TelefonoPI = historicoDB.TelefonoPI,
                        TotalCEO = historicoDB.TotalCEO,
                        TotalCPO = historicoDB.TotalCPO,
                        TotalPiezasDent = historicoDB.TotalPiezasDent,
                        TotalPiezasSanas = historicoDB.TotalPiezasSanas,
                        TratamientoMedico = historicoDB.TratamientoMedico,
                        Tuberculosis = historicoDB.Tuberculosis,
                        VIH = historicoDB.VIH
                    };
                }

                dataContext.HistoricoPaciente.InsertOnSubmit(historico);
                dataContext.SubmitChanges();
                return fichaHistoricoParaGuardar.ID;
            }
            catch (Exception ex)
            {

                ControlLogErrores.Insertar("NConsulta", "ABMHistoricoOdontograma", "GuardarHistoricoFichaPaciente", ex);
                return -1;
            }
        }
        public static List<ValoresOdontogramaDto> ObtenerCriterios() {

            return (from v in dataContext.ValoresOdontograma
                    select new ValoresOdontogramaDto
                    {
                        T=v.T,
                        P = v.P,
                        Criterio= v.Criterio
                    }).ToList();
        }

        public static List<ValoresPeriodontalDto> ObtenerValoresPeriodontal() {
            return (from v in dataContext.ValoresPeriodontal
                    select new ValoresPeriodontalDto()
                    {
                        ID = v.ID,
                        Descripcion = v.Descripcion
                    }).ToList();
        }
        public static bool GuardarHistoricoPaciente(HistoricoPacienteOdontogramaDto historico, string idUsuario)
        {
            var splitFecha = historico.FechaNacimientoString.Split('/');
            historico.FechaNacimiento = new DateTime(Convert.ToInt16(splitFecha[2]), Convert.ToInt16(splitFecha[1]),
                Convert.ToInt16(splitFecha[0]));
            var splitFecha2 = historico.UltimaVisitaString.Split('/');
            historico.FecUltVisita = new DateTime(Convert.ToInt16(splitFecha2[2]), Convert.ToInt16(splitFecha2[1]),
                Convert.ToInt16(splitFecha2[0]));

            var historicoBd = (from h in dataContext.HistoricoPaciente
                               where h.ID == historico.ID
                               select h).FirstOrDefault();
            if (historicoBd == null)
            { //agregar uno nuevo
                var HistoricoNuevo = new HistoricoPaciente()
                {
                    Alergias = historico.Alergias,
                    Anemia = historico.Anemia,
                    AntPatFam = historico.AntPatFam,
                    ApellidoMaterno = historico.ApellidoMaterno,
                    ApellidoMaternoPI = historico.ApellidoMaternoPI,
                    ApellidoPaterno = historico.ApellidoPaterno,
                    ApellidoPaternoPI = historico.ApellidoPaternoPI,
                    Asma = historico.Asma,
                    ATM = historico.ATM,
                    Cardiopatias = historico.Cardiopatias,
                    Cmayuscula = historico.Cmayuscula,
                    Cminuscula = historico.Cminuscula,
                    DiabetesMel = historico.DiabetesMel,
                    Direccion = historico.Direccion,
                    DireccionPI = historico.DireccionPI,
                    E = historico.E,
                    Edad = historico.Edad,
                    Ei = historico.Ei,
                    Embarazo = historico.Embarazo,
                    Encias = historico.Encias,
                    EnfGastricas = historico.EnfGastricas,
                    Epilepsia = historico.Epilepsia,
                    EstadoCivil = historico.EstadoCivil,
                    FechaCreacion = historico.FechaCreacion,
                    FechaNacimiento = historico.FechaNacimiento,
                    FecUltVisita = historico.FecUltVisita,
                    FrecCepDent = historico.FrecCepDent,
                    GangliosLinf = historico.GangliosLinf,
                    GradoInstruccion = historico.GradoInstruccion,
                    HemorragiaExtDent = historico.HemorragiaExtDent,
                    Hepatitis = historico.Hepatitis,
                    HigieneBucal = historico.HigieneBucal,
                    Hipertension = historico.Hipertension,
                    ID = historico.ID,
                    IdHistoricoFichaCab = historico.IdHistoricoFichaCab,
                    Idioma = historico.Idioma,
                    isBebe = historico.isBebe,
                    isCepilloDental = historico.isCepilloDental,
                    isEnjuagueBucal = historico.isEnjuagueBucal,
                    isFuma = historico.isFuma,
                    isHiloDental = historico.isHiloDental,
                    isProtesisDental = historico.isProtesisDental,
                    isSangreEncias = historico.isSangreEncias,
                    Labios = historico.Labios,
                    Lengua = historico.Lengua,
                    LugarNacimiento = historico.LugarNacimiento,
                    Medicamento = historico.Medicamento,
                    MucosaYugal = historico.MucosaYugal,
                    NacionesOriginarias = historico.NacionesOriginarias,
                    Nombre = historico.Nombre,
                    NombresPI = historico.NombresPI,
                    Observaciones = historico.Observaciones,
                    Ocupacion = historico.Ocupacion,
                    Omayuscula = historico.Omayuscula,
                    Ominuscula = historico.Ominuscula,
                    OtroHabitos = historico.OtroHabitos,
                    OtrosAntP = historico.OtrosAntP,
                    OtrosExtOral = historico.OtrosExtOral,
                    P = historico.P,
                    Paladar = historico.Paladar,
                    PersonaInformacion = historico.PersonaInformacion,
                    PisoBoca = historico.PisoBoca,
                    Respirador = historico.Respirador,
                    Sexo = historico.Sexo,
                    Sext11 = historico.Sext11,
                    Sext17_16 = historico.Sext17_16,
                    Sext26_27 = historico.Sext26_27,
                    Sext31 = historico.Sext31,
                    Sext37_36 = historico.Sext37_36,
                    Sext46_47 = historico.Sext46_47,
                    Telefono = historico.Telefono,
                    TelefonoPI = historico.TelefonoPI,
                    TotalCEO = historico.TotalCEO,
                    TotalCPO = historico.TotalCPO,
                    TotalPiezasDent = historico.TotalPiezasDent,
                    TotalPiezasSanas = historico.TotalPiezasSanas,
                    TratamientoMedico = historico.TratamientoMedico,
                    Tuberculosis = historico.Tuberculosis,
                    VIH = historico.VIH
                };
                dataContext.HistoricoPaciente.InsertOnSubmit(HistoricoNuevo);
            }
            else
            { //modificar
                //  historicoBd.IdConsultorio = historico.IdConsultorio;
                //historicoBd.IdPaciente = historico.IdPaciente;

                historicoBd.Alergias = historico.Alergias;
                historicoBd.Anemia = historico.Anemia;
                historicoBd.AntPatFam = historico.AntPatFam;
                historicoBd.ApellidoMaterno = historico.ApellidoMaterno;
                historicoBd.ApellidoMaternoPI = historico.ApellidoMaternoPI;
                historicoBd.ApellidoPaterno = historico.ApellidoPaterno;
                historicoBd.ApellidoPaternoPI = historico.ApellidoPaternoPI;
                historicoBd.Asma = historico.Asma;
                historicoBd.ATM = historico.ATM;
                historicoBd.Cardiopatias = historico.Cardiopatias;
                historicoBd.Cmayuscula = historico.Cmayuscula;
                historicoBd.Cminuscula = historico.Cminuscula;
                historicoBd.DiabetesMel = historico.DiabetesMel;
                historicoBd.Direccion = historico.Direccion;
                historicoBd.DireccionPI = historico.DireccionPI;
                historicoBd.E = historico.E;
                historicoBd.Edad = historico.Edad;
                historicoBd.Ei = historico.Ei;
                historicoBd.Embarazo = historico.Embarazo;
                historicoBd.Encias = historico.Encias;
                historicoBd.EnfGastricas = historico.EnfGastricas;
                historicoBd.Epilepsia = historico.Epilepsia;
                historicoBd.EstadoCivil = historico.EstadoCivil;
                historicoBd.FechaCreacion = historico.FechaCreacion;
                historicoBd.FechaNacimiento = historico.FechaNacimiento;
                historicoBd.FecUltVisita = historico.FecUltVisita;
                historicoBd.FrecCepDent = historico.FrecCepDent;
                historicoBd.GangliosLinf = historico.GangliosLinf;
                historicoBd.GradoInstruccion = historico.GradoInstruccion;
                historicoBd.HemorragiaExtDent = historico.HemorragiaExtDent;
                historicoBd.Hepatitis = historico.Hepatitis;
                historicoBd.HigieneBucal = historico.HigieneBucal;
                historicoBd.Hipertension = historico.Hipertension;
                historicoBd.ID = historico.ID;
                historicoBd.IdHistoricoFichaCab = historico.IdHistoricoFichaCab;
                historicoBd.Idioma = historico.Idioma;
                historicoBd.isBebe = historico.isBebe;
                historicoBd.isCepilloDental = historico.isCepilloDental;
                historicoBd.isEnjuagueBucal = historico.isEnjuagueBucal;
                historicoBd.isFuma = historico.isFuma;
                historicoBd.isHiloDental = historico.isHiloDental;
                historicoBd.isProtesisDental = historico.isProtesisDental;
                historicoBd.isSangreEncias = historico.isSangreEncias;
                historicoBd.Labios = historico.Labios;
                historicoBd.Lengua = historico.Lengua;
                historicoBd.LugarNacimiento = historico.LugarNacimiento;
                historicoBd.Medicamento = historico.Medicamento;
                historicoBd.MucosaYugal = historico.MucosaYugal;
                historicoBd.NacionesOriginarias = historico.NacionesOriginarias;
                historicoBd.Nombre = historico.Nombre;
                historicoBd.NombresPI = historico.NombresPI;
                historicoBd.Observaciones = historico.Observaciones;
                historicoBd.Ocupacion = historico.Ocupacion;
                historicoBd.Omayuscula = historico.Omayuscula;
                historicoBd.Ominuscula = historico.Ominuscula;
                historicoBd.OtroHabitos = historico.OtroHabitos;
                historicoBd.OtrosAntP = historico.OtrosAntP;
                historicoBd.OtrosExtOral = historico.OtrosExtOral;
                historicoBd.P = historico.P;
                historicoBd.Paladar = historico.Paladar;
                historicoBd.PersonaInformacion = historico.PersonaInformacion;
                historicoBd.PisoBoca = historico.PisoBoca;
                historicoBd.Respirador = historico.Respirador;
                historicoBd.Sexo = historico.Sexo;
                historicoBd.Sext11 = historico.Sext11;
                historicoBd.Sext17_16 = historico.Sext17_16;
                historicoBd.Sext26_27 = historico.Sext26_27;
                historicoBd.Sext31 = historico.Sext31;
                historicoBd.Sext37_36 = historico.Sext37_36;
                historicoBd.Sext46_47 = historico.Sext46_47;
                historicoBd.Telefono = historico.Telefono;
                historicoBd.TelefonoPI = historico.TelefonoPI;
                historicoBd.TotalCEO = historico.TotalCEO;
                historicoBd.TotalCPO = historico.TotalCPO;
                historicoBd.TotalPiezasDent = historico.TotalPiezasDent;
                historicoBd.TotalPiezasSanas = historico.TotalPiezasSanas;
                historicoBd.TratamientoMedico = historico.TratamientoMedico;
                historicoBd.Tuberculosis = historico.Tuberculosis;
                historicoBd.VIH = historico.VIH;

            }

            try
            {
                dataContext.SubmitChanges();
                ControlBitacora.Insertar("Se insertó un nuevo histórico paciente", idUsuario);
                return true;
            }
            catch (Exception ex)
            {

                ControlLogErrores.Insertar("NConsulta", "ABMHistoricoOdontograma", "GuardarHistoricoPaciente", ex);
                return false;
            }

        }

        public static HistoricoPacienteOdontogramaDto ObtenerHistoricoPaciente(int idFichaCab)
        {
            var historicoBd = (from h in dataContext.HistoricoPaciente
                               where h.IdHistoricoFichaCab == idFichaCab
                               select new HistoricoPacienteOdontogramaDto
                               {
                                   Alergias = h.Alergias,
                                   Anemia = h.Anemia,
                                   AntPatFam = h.AntPatFam,
                                   ApellidoMaterno = h.ApellidoMaterno,
                                   ApellidoMaternoPI = h.ApellidoMaternoPI,
                                   ApellidoPaterno = h.ApellidoPaterno,
                                   ApellidoPaternoPI = h.ApellidoPaternoPI,
                                   Asma = h.Asma,
                                   ATM = h.ATM,
                                   Cardiopatias = h.Cardiopatias,
                                   Cmayuscula = h.Cmayuscula,
                                   Cminuscula = h.Cminuscula,
                                   DiabetesMel = h.DiabetesMel,
                                   Direccion = h.Direccion,
                                   DireccionPI = h.DireccionPI,
                                   E = h.E,
                                   Edad = h.Edad,
                                   Ei = h.Ei,
                                   Embarazo = h.Embarazo,
                                   Encias = h.Encias,
                                   EnfGastricas = h.EnfGastricas,
                                   Epilepsia = h.Epilepsia,
                                   EstadoCivil = h.EstadoCivil,
                                   FechaCreacion = h.FechaCreacion,
                                   FechaNacimiento = h.FechaNacimiento,
                                   FecUltVisita = h.FecUltVisita,
                                   FrecCepDent = h.FrecCepDent,
                                   GangliosLinf = h.GangliosLinf,
                                   GradoInstruccion = h.GradoInstruccion,
                                   HemorragiaExtDent = h.HemorragiaExtDent,
                                   Hepatitis = h.Hepatitis,
                                   HigieneBucal = h.HigieneBucal,
                                   Hipertension = h.Hipertension,
                                   ID = h.ID,
                                   IdHistoricoFichaCab = h.IdHistoricoFichaCab,
                                   Idioma = h.Idioma,
                                   isBebe = h.isBebe,
                                   isCepilloDental = h.isCepilloDental,
                                   isEnjuagueBucal = h.isEnjuagueBucal,
                                   isFuma = h.isFuma,
                                   isHiloDental = h.isHiloDental,
                                   isProtesisDental = h.isProtesisDental,
                                   isSangreEncias = h.isSangreEncias,
                                   Labios = h.Labios,
                                   Lengua = h.Lengua,
                                   LugarNacimiento = h.LugarNacimiento,
                                   Medicamento = h.Medicamento,
                                   MucosaYugal = h.MucosaYugal,
                                   NacionesOriginarias = h.NacionesOriginarias,
                                   Nombre = h.Nombre,
                                   NombresPI = h.NombresPI,
                                   Observaciones = h.Observaciones,
                                   Ocupacion = h.Ocupacion,
                                   Omayuscula = h.Omayuscula,
                                   Ominuscula = h.Ominuscula,
                                   OtroHabitos = h.OtroHabitos,
                                   OtrosAntP = h.OtrosAntP,
                                   OtrosExtOral = h.OtrosExtOral,
                                   P = h.P,
                                   Paladar = h.Paladar,
                                   PersonaInformacion = h.PersonaInformacion,
                                   PisoBoca = h.PisoBoca,
                                   Respirador = h.Respirador,
                                   Sexo = h.Sexo,
                                   Sext11 = h.Sext11,
                                   Sext17_16 = h.Sext17_16,
                                   Sext26_27 = h.Sext26_27,
                                   Sext31 = h.Sext31,
                                   Sext37_36 = h.Sext37_36,
                                   Sext46_47 = h.Sext46_47,
                                   Telefono = h.Telefono,
                                   TelefonoPI = h.TelefonoPI,
                                   TotalCEO = h.TotalCEO,
                                   TotalCPO = h.TotalCPO,
                                   TotalPiezasDent = h.TotalPiezasDent,
                                   TotalPiezasSanas = h.TotalPiezasSanas,
                                   TratamientoMedico = h.TratamientoMedico,
                                   Tuberculosis = h.Tuberculosis,
                                   VIH = h.VIH

                               }).FirstOrDefault();

            return historicoBd;
        }
        #endregion

    }
}