app.controller("cuentasPorCobrarController", function (cuentasService, pacienteService, $scope, $rootScope, loginService, consultasService) {
    init();

    function init() {
        $rootScope.mostrarMenu = true;
        if (!$rootScope.sessionDto) {
            loginService.getSessionDto().then(function (result) {
                $rootScope.sessionDto = result;
                inicializarDatos();
            });
        } else {
            inicializarDatos();
        }

    };

    function inicializarDatos() {
        prepararNuevaCuenta();
        cargarCuentasPorCobrar();
        cargarTrabajosConsultorio();
        cargarClientesPaciente();
    }
    function prepararNuevaCuenta() {
        $scope.cuentaSeleccionada = null;
        $scope.cuentaParaGuardar = {
            ID: -1,
            Descripcion: "",
            Monto: 0,
            IDTrabajo: -1,
            Saldo: 0,
            Estado: 0,
            Login: "",
            IDConsultorio: $rootScope.sessionDto.IDConsultorio,
            State: 1
        };
        $scope.cuentaSeleccionada = null;
        $scope.pagoSelecionado = null;
        $scope.trabajoSeleccionado = null;
        $scope.clienteSeleccionado = null;
        $("#descripcionId").focus();
    }
    function prepararNuevoPago() {
        $scope.mostrarLabelMonto = false;

        $scope.pagoParaGuardar = {
            Descripcion: "",
            Monto: 0,
            IDCuentasPorCobrar: $scope.cuentaSeleccionada.ID,
            State: 1
        };

        $scope.pagoSelecionado = null;
        $("#descripcionId").focus();
    }

    $scope.nuevaCuenta = function () {
        prepararNuevaCuenta();
    };
    $scope.abrirModalNuevaCuenta = function () {
        prepararNuevaCuenta();
        $('#nueva-cuenta').modal('show');
    };

    function cargarCuentasPorCobrar() {
        cuentasService.obtenerCuentasPorCobrarPorConsultorio($rootScope.sessionDto.IDConsultorio).then(function (result) {

            $scope.cuentasPorCobrarConsultorio = result.select(function (cuenta) {
                cuenta.sumaPagos = cuenta.Monto - cuenta.Saldo;
                cuenta.FechaCreacion = moment(cuenta.FechaCreacion).format('DD/MM/YYYY');
                cuenta.Detalle = cuenta.Detalle.select(function (detalle) {
                    detalle.FechaCreacion = moment(detalle.FechaCreacion).format('DD/MM/YYYY');
                    return detalle;
                });
                return cuenta;
            });

        });
    }

    function cargarClientesPaciente() {
        pacienteService.obtenerClientesPorEmpresa($rootScope.sessionDto.IDConsultorio).then(function (result) {
            $scope.clientesConsultorio = result;
        });
    }
    function cargarTrabajosConsultorio() {
        cuentasService.getTrabajosConsultorio($rootScope.sessionDto.IDConsultorio).then(function (result) {
            $scope.trabajosConsultorio = result;
        });
    }
    $scope.seleccionarCuenta = function (cuenta) {
        console.log(cuenta);
        $scope.cuentaSeleccionada = cuenta;
        $scope.cuentaParaGuardar = angular.copy($scope.cuentaSeleccionada);
        $scope.cuentaParaGuardar.State = 2;
        selectTrabajo();
        selectCliente();
        $scope.pagoSelecionado = null;
    };

    $scope.seleccionarPago = function (pago) {
        if ($scope.cuentaParaGuardar.Estado == 0) {
            $scope.pagoSeleccionado = pago;
            $scope.pagoParaGuardar = angular.copy($scope.pagoSeleccionado);
            $scope.pagoParaGuardar.State = 2;
        }
    };

    function selectTrabajo() {
        var selectTrabajoitem = $scope.trabajosConsultorio.where(function (item) {
            return item.ID == $scope.cuentaParaGuardar.IDTrabajo;
        });
        $scope.trabajoSeleccionado = selectTrabajoitem[0];
    }
    function selectCliente() {
        var selectClienteitem = $scope.clientesConsultorio.where(function (item) {
            return item.LoginCliente == $scope.cuentaParaGuardar.Login;
        });
        $scope.clienteSeleccionado = selectClienteitem[0];
    }

    $scope.validadUsuario = function () {
        return $scope.userToSave == null || $scope.userToSave.Nombre.length < 4 || $scope.userToSave.Login.length < 4
            || $scope.userToSave.Password.length < 4 || $scope.userToSave.Password != $scope.userToSave.ConfirmPass
            || $scope.rolSelected == null;
    };

    $scope.mostrarModalPago = function () {
        prepararNuevoPago();
        $('#nuevo-pago').modal('show');
    };
    $scope.mostrarModalEditarPago = function () {
        $scope.mostrarLabelMonto = false;
        $('#nuevo-pago').modal('show');
    };

    $scope.openModalConfirmDelele = function () {
        $('#eliminar-cuenta').modal('show');
    };

    $scope.openModalConfirmDelelePago = function () {
        $('#eliminar-pago').modal('show');
    };

    $scope.closeWarnig = function (modal) {
        $scope.pagoSeleccionado = null;

        $(modal).modal('hide');
    };

    $scope.guardarNuevoPago = function () {
        if ($rootScope.sessionDto.Licencia == -1 || $rootScope.sessionDto.Licencia == 0) {
            $('#modal-licencia-free').modal("show");
        } else {
            if ($scope.pagoParaGuardar.Monto > $scope.cuentaSeleccionada.Saldo) {
                $scope.mostrarLabelMonto = true;
                $('#montoInputp').focus();
                return;
            }
            if ($scope.pagoParaGuardar.State == 1) {
                cuentasService.insertarNuevoPago($scope.pagoParaGuardar).then(function (result) {
                    if (result.Data) {
                        inicializarDatos();
                        toastr.success(result.Message);
                        prepararNuevaCuenta();
                        $('#nuevo-pago').modal('hide');
                        $('#detalle-cuenta').modal('hide');
                        $scope.cuentaSeleccionada = null;
                    } else {
                        toastr.error(result.Message);
                    }
                });
            } else {
                cuentasService.modificarPago($scope.pagoParaGuardar).then(function (result) {
                    if (result.Data) {
                        inicializarDatos();
                        toastr.success(result.Message);
                        $('#nuevo-pago').modal('hide');
                        $('#detalle-cuenta').modal('hide');
                    } else {
                        toastr.error(result.Message);
                    }
                });
            }
        }
    };

    $scope.abrirModalPagos = function () {
        $('#detalle-cuenta').modal('show');
    };
    $scope.abrirModalTratamiento = function () {
        consultasService.obtenerHistoricoFichaCab($scope.cuentaSeleccionada.IDHistoricoFichaCab).then(function (result) {
            $scope.listaTratamiento = result.FichaTrabajo;
            $('#detalle-ficha').modal("show");

        })

    }
    $scope.guardarCuenta = function () {
        if ($rootScope.sessionDto.Licencia == -1 || $rootScope.sessionDto.Licencia == 0) {
            $('#modal-licencia-free').modal("show");
        } else {
            if ($scope.cuentaParaGuardar.Monto <= 0) {
                $scope.mensajeErrorMonto = "El monto debe ser mayor a 0";
                $scope.mostrarErrorMensajeMonto = true;
                $('#montoInput').focus();
                return;
            }
            if ($scope.cuentaParaGuardar.Monto < $scope.cuentaParaGuardar.sumaPagos) {
                $scope.mensajeErrorMonto = "Monto invalido, debe ser mayor o igual a " + $scope.cuentaParaGuardar.sumaPagos;
                $scope.mostrarErrorMensajeMonto = true;
                $('#montoInput').focus();
                return;
            }
            $scope.cuentaParaGuardar.IDTrabajo = $scope.trabajoSeleccionado.ID;
            $scope.cuentaParaGuardar.Login = $scope.clienteSeleccionado.LoginCliente;
            if ($scope.cuentaParaGuardar.State == 1) {
                cuentasService.insertarNuevaCuenta($scope.cuentaParaGuardar).then(function (result) {
                    if (result.Data) {
                        inicializarDatos();
                        toastr.success(result.Message);
                        $('#nueva-cuenta').modal('hide');
                    } else {
                        toastr.error(result.Message);
                    }
                });
            } else {
                cuentasService.modificarCuenta($scope.cuentaParaGuardar).then(function (result) {
                    if (result.Data) {
                        inicializarDatos();
                        toastr.success(result.Message);
                        prepararNuevaCuenta();
                        $('#nueva-cuenta').modal('hide');
                    } else {
                        toastr.error(result.Message);
                    }
                });
            }
        }
    };
    $scope.abrirModalEditarCuenta = function () {
        $scope.cuentaSeleccionada.State = 2;
        $('#nueva-cuenta').modal('show');
    }
    $scope.eliminarPago = function () {
        if ($rootScope.sessionDto.Licencia == -1 || $rootScope.sessionDto.Licencia == 0) {
            $('#modal-licencia-free').modal("show");
        } else {
            cuentasService.eliminarPago($scope.pagoSeleccionado.ID).then(function (result) {
                if (result.Data) {
                    inicializarDatos();
                    toastr.success(result.Message);
                    $('#eliminar-pago').modal('hide');
                    $('#detalle-cuenta').modal('hide');
                } else {
                    toastr.error(result.Message);
                }
            });
        }
    };
    $scope.validarGuardarCuenta = function () {
        if ($scope.cuentaParaGuardar == undefined) return true;
        return $scope.cuentaParaGuardar.Descripcion.length == 0 || $scope.cuentaParaGuardar.Monto == 0 ||
            $scope.clienteSeleccionado == null || $scope.trabajoSeleccionado == null || $scope.cuentaParaGuardar.Estado != 0;
    };
    $scope.eliminarCuenta = function () {
        if ($rootScope.sessionDto.Licencia == -1 || $rootScope.sessionDto.Licencia == 0) {
            $('#modal-licencia-free').modal("show");
        } else {
            cuentasService.eliminarCuenta($scope.cuentaSeleccionada.ID).then(function (result) {
                if (result.Data) {
                    inicializarDatos();
                    toastr.success(result.Message);
                    $('#eliminar-cuenta').modal('hide');
                } else {
                    toastr.error(result.Message);
                }
            });
        }
    };

    $scope.closeModalLicencia = function () {

    }
    //$scope.seleccionarPago = function (pago) {
    //    $scope.pagoSeleccionado = pago;
    //};
});