app.controller("cuentasPorPagarController", function (cuentasService, pacienteService, $scope, $rootScope, loginService) {
    init();

    function init() {
        $rootScope.mostrarMenu = true;
        if (!$rootScope.sessionDto) {
            loginService.getSessionDto().then(function (result) {
                $rootScope.sessionDto = result;
                cargarCuentasPorPagar();
            });
        } else {
            cargarCuentasPorPagar();
        }

    };
    $scope.closeWarnig = function (modal) {
        $scope.pagoSeleccionado = null;

        $(modal).modal('hide');
    };
    $scope.seleccionarCuenta = function(cuenta) {
        $scope.cuentaSeleccionada = cuenta;

    };

    $scope.mostrarDetalleCuenta = function() {
        $('#detalle-cuenta').modal('show');
    };

    function cargarCuentasPorPagar() {
        cuentasService.obtenerCuentasPorPagaPorCliente($rootScope.sessionDto.loginUsuario).then(function (result) {
            $scope.cuentasPorPagar = result.select(function (cuenta) {
                cuenta.FechaCreacion = moment(cuenta.FechaCreacion).format('DD/MM/YYYY');
                cuenta.Detalle = cuenta.Detalle.select(function (detalle) {
                    detalle.FechaCreacion = moment(detalle.FechaCreacion).format('DD/MM/YYYY');
                    return detalle;
                });
                return cuenta;
            });
        });
    }
});