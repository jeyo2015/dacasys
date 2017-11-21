app.controller("historicosController", function (loginService, consultasService, comentarioService, $scope, $rootScope, $location) {
    init();

    function init() {
        $rootScope.mostrarMenu = true;
        if (!$rootScope.sessionDto) {
            loginService.getSessionDto().then(function (result) {
                $rootScope.sessionDto = result;
                $rootScope.pacienteSeleccionado = {
                    IdPaciente: result.IDPacienteSeleccionado
                }
                inicializarDatos();
            });
        } else {
            inicializarDatos();
        }


    };
    function inicializarDatos() {

        //$("#fechanac").datepicker({
        //    dateFormat: 'dd/mm/yy',
        //    onSelect: function () {

        //        $scope.historicoNuevo.FechaNacimiento = $.datepicker.formatDate("dd/mm/yy", $(this).datepicker('getDate'));
        //        $scope.historicoNuevo.FechaNacimientoString = $.datepicker.formatDate("dd/mm/yy", $(this).datepicker('getDate'));
        //        $scope.$apply();
        //    }
        //});
        // $('#fechanac').val(moment().format('DD/MM/YYYY'));
        $scope.fichaPacienteSelected = null;
        $scope.tieneHistorico = true;
        consultasService.obtenerHistoricoFichaCabs($rootScope.sessionDto.IDPacienteSeleccionado, $rootScope.sessionDto.IDConsultorio, $rootScope.sessionDto.IDCitaSeleccionada).then(function (result) {
            $scope.FichasPaciente = result;
            angular.forEach($scope.FichasPaciente, function (element) {
                element.Fecha = moment(element.Fecha).format('DD/MM/YYYY');
            })

        })

    }

    $scope.selectFichaPaciente = function (fichaHistorico) {
        $scope.fichaPacienteSelected = fichaHistorico;
    }

    $scope.openModalCrearFicha = function () {

        prepararNuevaFicha();
        $scope.fichaPacienteSelected = null;
        $('#modal-nueva-ficha').modal("show");
    }
    function prepararNuevoHistorico() {
        // var apellidos = $rootScope.pacienteSeleccionado.Apellido.split(" ");
        return {
            ID: 0,

            IdHistoricoFichaCab: "",

            FechaCreacion: "",

            ApellidoPaterno: "",

            ApellidoMaterno: "",

            Nombre: "",

            Edad: "",

            Sexo: "F",

            LugarNacimiento: "",

            FechaNacimiento: "",

            Ocupacion: "",

            Direccion: "",

            Telefono: "",

            GradoInstruccion: "I",

            EstadoCivil: "S",

            NacionesOriginarias: "",

            Idioma: "",

            PersonaInformacion: "",

            ApellidoPaternoPI: "",

            ApellidoMaternoPI: "",

            NombresPI: "",

            DireccionPI: "",

            TelefonoPI: "",

            AntPatFam: "",

            Anemia: false,

            Cardiopatias: false,

            EnfGastricas: false,

            Hepatitis: false,

            Tuberculosis: false,

            Asma: false,

            DiabetesMel: false,

            Epilepsia: false,

            Hipertension: false,

            VIH: true,

            OtrosAntP: "",

            Alergias: false,

            Embarazo: 0,

            TratamientoMedico: "",

            Medicamento: "",

            HemorragiaExtDent: 0,

            ATM: "",

            GangliosLinf: "",

            Respirador: "",

            OtrosExtOral: "",

            Labios: "",

            Lengua: "",

            Paladar: "",

            PisoBoca: "",

            MucosaYugal: "",

            Encias: "",

            isProtesisDental: "",

            FecUltVisita: "",

            isFuma: "",

            isBebe: "",

            OtroHabitos: "",

            isCepilloDental: null,

            isHiloDental: null,

            isEnjuagueBucal: null,

            FrecCepDent: "",

            isSangreEncias: null,

            HigieneBucal: null,

            Observaciones: "",

            Sext17_16: "",

            Sext11: "",

            Sext26_27: "",

            Sext46_47: "",

            Sext31: "",

            Sext37_36: "",

            Cminuscula: "",

            E: "",

            Ominuscula: "",

            TotalCEO: "",

            Cmayuscula: "",

            P: "",

            Ei: "",

            Omayuscula: "",

            TotalCPO: "",

            TotalPiezasSanas: "",

            TotalPiezasDent: "",
            FechaNacimientoString: "",
        }
    }
    function prepararNuevaFicha() {
        $rootScope.fichaHistoricoNueva = {
            ID: 0,
            EstadoString: 'Abierto',
            Estado: true,
            Fecha: "",
            Odontograma: "",
            Titulo: "",
            IDConsultorio: $rootScope.sessionDto.IDConsultorio,
            IDPaciente: $rootScope.sessionDto.IDPacienteSeleccionado,
            HistoricoPaciente: prepararNuevoHistorico()
        };
    }
    $scope.ingresarFicha = function () {
        $rootScope.fichaHistoricoNueva = $scope.fichaPacienteSelected;
        $location.path('/fichaDetalle');
    }

    $scope.crearFicha = function () {
        if ($rootScope.sessionDto.Licencia == -1 || $rootScope.sessionDto.Licencia == 0) {
            $rootScope.fichaHistoricoNueva.ID = 0;
            $("#modal-nueva-ficha").modal("hide");
            $('.modal-backdrop').remove();
            $('body').removeClass("modal-open");
            $location.path('/fichaDetalle');
        } else {
            consultasService.guardarHistoricoFichaPaciente($rootScope.fichaHistoricoNueva).then(function (result) {
                if (result.Success) {
                    toastr.success(result.Message);
                    $rootScope.fichaHistoricoNueva.ID = result.Data;
                    $("#modal-nueva-ficha").modal("hide");
                    $('.modal-backdrop').remove();
                    $('body').removeClass("modal-open");
                    $location.path('/fichaDetalle');

                } else {
                    toastr.error(result.Message);
                }
            });
        }

    };
    $scope.validarCamposHistoricos = function () {
        if (!$scope.historicoNuevo) return;
        return (($scope.historicoNuevo.Istratamiento == 1 && ($scope.historicoNuevo.DescTratamiento == "" || $scope.historicoNuevo.DescTratamiento == null))
                || ($scope.historicoNuevo.ProblemaOdontologico == 1 && ($scope.historicoNuevo.DescProblemaOdon == "" || $scope.historicoNuevo.DescProblemaOdon == null))
            || ($scope.historicoNuevo.OtraEnfermedadCheck && ($scope.historicoNuevo.OtraEnfermedad == "" || $scope.historicoNuevo.OtraEnfermedad == null))
            || ($scope.historicoNuevo.AlergiaOtroCheck && ($scope.historicoNuevo.AlergiaOtro == "" || $scope.historicoNuevo.AlergiaOtro == null))
            || ($scope.historicoNuevo.FechaNacimiento == null || $scope.historicoNuevo.FechaNacimiento == "")
            || ($scope.historicoNuevo.TipoDocumento == null) || ($scope.historicoNuevo.NumeroDocumento == "")
            || ($scope.historicoNuevo.TipoSangre == null || $scope.historicoNuevo.TipoSangre == "")
            || ($scope.historicoNuevo.Nacionalidad == null || $scope.historicoNuevo.Nacionalidad == "")
            || ($scope.historicoNuevo.LugarNacimiento == null || $scope.historicoNuevo.LugarNacimiento == "")
            );
    }
    $scope.limpiar = function () {

        $scope.historicoNuevo = angular.copy($scope.historicoOriginal);

    }

    $scope.openModalCerrarFicha = function () {
        if ($rootScope.sessionDto.IDCitaSeleccionada != "")
            $("#cita-atendida").modal("show");
        else {
            $scope.cerrarFicha(false);
        }

    }
    $scope.cerrarFicha = function (citaAtendida) {
        $("#cita-atendida").modal("hide");
        consultasService.cerrarFichaHistorico($scope.fichaPacienteSelected.ID, citaAtendida).then(function (result) {
            if (result.Success) {
                toastr.success(result.Message);
                inicializarDatos();
                $rootScope.sessionDto.IDCitaSeleccionada = "";
            } else {
                toastr.error(result.Message);
            }
        });
    }
    $scope.guardarHistorico = function () {

        // $scope.historicoNuevo. =$.datepicker.formatDate("dd/mm/yy", $(this).datepicker('getDate'));
        consultasService.guardarHistoricoPaciente($scope.historicoNuevo).then(function (result) {
            if (result.Success) {
                toastr.success(result.Message);
                inicializarDatos();

            } else {
                toastr.error(result.Message);
            }
        });
    }
});