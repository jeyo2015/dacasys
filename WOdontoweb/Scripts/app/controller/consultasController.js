app.controller("consultasController", function (consultasService, pacienteService, clinicaService, $scope, $compile, $rootScope) {
    init();

    function init() {
        $scope.dateSelected = new Date();
        $scope.citaSeleccionada = null;
        $("#datepicker").datepicker({
            dateFormat: "yy-mm-dd",
            defaultDate: $scope.dateSelected,
            onSelect: function (date) {
                $scope.dateSelected = $.datepicker.formatDate("yy-mm-dd", $(this).datepicker('getDate'));
            }
        });
        $("#datepicker").datepicker("setDate", $scope.dateSelected);
        cargarConsultorio();


    };

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
    }

    $scope.showModalPacientes = function () {
        cargarPacientesEmpresa("#modal-seleccionar-cliente");
    }
    $scope.showModalCancelarCita = function () {
        $scope.horaLibre = false;
        $scope.motivoCancelacion = "";
        $("#modal-cancelar-cita").modal("show");
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

    $scope.seleccionarPaciente = function (paciente) {
        $scope.pacienteSeleccionado = paciente;
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
});