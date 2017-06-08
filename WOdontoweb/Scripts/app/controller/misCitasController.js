app.controller("misCitasController", function (consultasService, $scope, $compile, $rootScope, loginService) {
    init();

    function init() {
        if (!$rootScope.sessionDto) {
            loginService.getSessionDto().then(function (result) {
                $rootScope.sessionDto = result;
                cargarCitasPorCliente();
            });
        } else {
            cargarCitasPorCliente();
        }

    };

    function cargarCitasPorCliente() {
        var listaCitas = [];
        consultasService.obtenerCitasPorCliente($rootScope.sessionDto.loginUsuario).then(function (result) {
            $.each(result, function (i, item) {
                
                var hinicio = item.HoraInicioString.split(":");
                var hfinal = item.HoraFinString.split(":");
                item.HoraInicioString = (hinicio[0] < 10 ? '0' + hinicio[0] : hinicio[0]) + ':' + (hinicio[1] < 10 ? '0' + hinicio[1] : hinicio[1]);
                item.HoraFinString = (hfinal[0] < 10 ? '0' + hfinal[0] : hfinal[0]) + ':' + (hfinal[1] < 10 ? '0' + hfinal[1] : hfinal[1]);
                listaCitas.push(item);
            });
            $scope.ListaCitas = listaCitas;
            console.log(listaCitas);
        });
    }

    $scope.abrirModalCancelarCita = function () {
        $('#modal-cancelar-cita').modal('show');
    };

    $scope.cerrarModalCancelarCita = function () {
        $('#modal-cancelar-cita').modal('hide');
    };

    $scope.seleccionarCita = function (cita) {
        if (cita.EstadoMostrar=="")
            $scope.miCitaSelected = cita;
        else
            $scope.miCitaSelected = null;
    };

    $scope.cancelarCitaPaciente = function () {
        consultasService.cancelarCitaPaciente($scope.miCitaSelected.IdCita).then(function (result) {
            if (result.Success) {
                toastr.success(result.Message);
                cargarCitasPorCliente();
                $("#modal-cancelar-cita").modal("hide");
                $scope.miCitaSelected = null;
            } else {
                toastr.error(result.Message);
            }
        });
    }
});