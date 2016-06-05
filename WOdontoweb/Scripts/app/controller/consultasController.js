app.controller("consultasController", function (consultasService, pacienteService, clinicaService, $scope, $compile, $rootScope) {
    init();

    function init() {
        $scope.diasDeSemana = ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"];
        $scope.dateSelected = moment().format('DD/MM/YYYY');
        $scope.dateSelectedString = $scope.dateSelected;
        $scope.citaSeleccionada = null;
        $("#datepicker").datepicker({
            dateFormat: "dd/mm/yy",
            defaultDate: $scope.dateSelected,
            onSelect: function (date) {
                $scope.dateSelected = $.datepicker.formatDate("dd/mm/yy", $(this).datepicker('getDate'));
                var aux = ObtenerFechaDesdeStrint($scope.dateSelected);
                $scope.dateSelectedString = $scope.diasDeSemana[aux.getDay() % 7] + " " + $scope.dateSelected;
                cargarCitasDelDia();
                $scope.$apply();
            }
        });
        $("#datepicker").datepicker("setDate", $scope.dateSelected);
        cargarConsultorio();


    };
    function ObtenerFechaDesdeStrint(dateString) {
        var dateSplit = dateString.split("/");
        return new Date(dateSplit[2], dateSplit[1] - 1, dateSplit[0]);//parseInt($scope.dateSelected.split('/')[0]);
    }
    function cargarConsultorio() {
        if ($rootScope.consultorioActual == undefined)
            clinicaService.getConsultorioByID($rootScope.sessionDto.IDConsultorio).then(function (result) {
                $rootScope.consultorioActual = result;
                cargarCitasDelDia();
            });
        else
            consultasService.getCitasDelDia($scope.dateSelected, $rootScope.sessionDto.IDConsultorio, $rootScope.consultorioActual.TiempoCita).then(function (result) {
                $scope.citasDelDia = result;
            });

    }

    function cargarCitasDelDia() {
        consultasService.getCitasDelDia($scope.dateSelected, $rootScope.sessionDto.IDConsultorio, $rootScope.consultorioActual.TiempoCita).then(function (result) {
            $scope.citasDelDia = result;
        });
    }
    $scope.seleccionaCita = function (cita) {
        $scope.pacienteSeleccionado = null;
        $scope.citaSeleccionada = cita;
        mostrarAdvertenciasEstadoCita();
    }
    function mostrarAdvertenciasEstadoCita() {
        if ($scope.citaSeleccionada.EsTarde)
            toastr.warning("La fecha y hora seleccionada ya no estan diponibles");
        else
            if ($scope.citaSeleccionada.EstaAtendida)
                toastr.warning("La cita seleccionada ya fue atendida");


    }
    $scope.showModalPacientes = function () {
        cargarPacientesEmpresa("#modal-seleccionar-cliente");
    }
    $scope.showModalPacientesCliente = function () {
        getPacientesByCliente("#modal-seleccionar-paciente")
    }
    $scope.showModalCancelarCita = function () {
        $scope.horaLibre = false;
        $scope.motivoCancelacion = "";
        $("#modal-cancelar-cita").modal("show");
    }

    $scope.mostrarHistorico = function (paciente) {
        $scope.pacienteParaAtender = paciente;
        cargarHistoricoPaciente("#modal-historico-paciente");
    }

    function cargarHistoricoPaciente(modalOpen) {
        consultasService.getHistoricoPaciente($scope.pacienteParaAtender.IdPaciente, $rootScope.sessionDto.IDConsultorio).then(function (result) {
            $scope.historicosPaciente = result;
            $("#modal-seleccionar-paciente").modal("hide");
            if (modalOpen.length > 0) {
                $(modalOpen).modal('show');
            }
        });
    }
    function cargarPacientesEmpresa(modalOpen) {
        pacienteService.getPacienteConsultorio($rootScope.sessionDto.IDConsultorio).then(function (result) {
            $scope.pacientesConsultorio = result;
            if (modalOpen.length > 0) {
                $(modalOpen).modal('show');
            }
        });
    }

    $scope.closeModal = function (modal) {
        $("#" + modal).modal("hide");
        $scope.citaSeleccionada = null;

    }
    $scope.closeModalDetalle = function () {
        $("#modal-detalle-historico").modal('hide');
        $("#modal-historico-paciente").modal('show');
    }
    $scope.seleccionarPaciente = function (paciente) {
        $scope.pacienteSeleccionado = paciente;
    }

    $scope.mostrarModalNuevoHistorico = function () {
        prepararNuevoHistoricoDto();
        $("#modal-historico-paciente").modal("hide");
        $("#modal-nuevo-historico").modal("show");
    }

    $scope.crearNuevoHistorico = function () {
        consultasService.aBMHistorico($scope.historicoNuevo).then(function (result) {
            if (result.Success) {
                toastr.success(result.Message);
                $("#modal-nuevo-historico").modal("hide");
                cargarHistoricoPaciente("#modal-historico-paciente");
            } else {
                toastr.error(result.Message);
            }
        });
    }

    function prepararNuevoHistoricoDto() {
        $scope.historicoNuevo = {
            IdConsultorio: $rootScope.sessionDto.IDConsultorio,
            IdPaciente: $scope.pacienteParaAtender.IdPaciente,
            NumeroHistorico: $scope.historicosPaciente.length + 1,
            FechaCreacion: "",
            EstimacionCitas: 0,
            CitasRealizadas: 0,
            Estado: true,
            TituloHistorico: "",
            EstadoABM: 1
        };
    }
    $scope.mostrarDetalleHistorico = function (historico) {
        $scope.historicoPacienteSeleccionado = angular.copy(historico);
        $scope.historicoDetalleSeleccionado = angular.copy($scope.historicoPacienteSeleccionado.DetalleHistorico);
        prepararNuevoHistoricoDetalleDto();
        $("#modal-detalle-historico").modal('show');
        $("#modal-historico-paciente").modal('hide');
    }

    $scope.guardarHistoricoDetalle = function () {
        consultasService.insertarHistoricoDetalle($scope.historicoDetalleNuevo).then(function (result) {
            if (result.Success) {
                toastr.success(result.Message);
                cargarCitasDelDia();
                $("#modal-detalle-historico").modal("hide");
                $scope.pacienteSeleccionado = null;
                $scope.citaSeleccionada = null;
            } else {
                toastr.error(result.Message);
            }
        });
    }

    function prepararNuevoHistoricoDetalleDto() {
        $scope.historicoDetalleNuevo = {
            IdConsultorio: $rootScope.sessionDto.IDConsultorio,
            IdPaciente: $scope.historicoPacienteSeleccionado.IdPaciente,
            NumeroHistorico: $scope.historicoPacienteSeleccionado.NumeroHistorico,
            NumeroDetalle: $scope.historicoDetalleSeleccionado.length + 1,
            FechaCreacion: "",
            TrabajoRealizado: "",
            TrabajoARealizar: "",
            IdCita: $scope.citaSeleccionada.IdCita,
            CerrarHistorico: false
        };
    }
    $scope.agendarCita = function () {
        consultasService.insertarCitaPaciente($scope.citaSeleccionada, $scope.dateSelected, $scope.pacienteSeleccionado.LoginCliente).then(function (result) {
            if (result.Success) {
                toastr.success(result.Message);
                cargarCitasDelDia();
                $("#modal-seleccionar-cliente").modal("hide");
                $scope.pacienteSeleccionado = null;
                $scope.citaSeleccionada = null;
            } else {
                toastr.error(result.Message);
            }

        });
    }

    $scope.eliminarCitaPaciente = function () {
        consultasService.eliminarCitaPaciente($scope.citaSeleccionada, $scope.horaLibre, $scope.motivoCancelacion).then(function (result) {
            if (result.Success) {
                toastr.success(result.Message);
                cargarCitasDelDia();
                $("#modal-cancelar-cita").modal("hide");
                $scope.pacienteSeleccionado = null;
                $scope.citaSeleccionada = null;
            } else {
                toastr.error(result.Message);
            }
        });
    }



    function getPacientesByCliente(modalOpen) {
        pacienteService.getPacientesByCliente($scope.citaSeleccionada.LoginCliente).then(function (result) {
            $scope.pacientesClienteSeleccionado = result;
            if (modalOpen.length > 0) {
                $(modalOpen).modal('show');
            }
        });
    }
});