app.controller("cuentasPorCobrarController", function (cuentasService, pacienteService, $scope, $rootScope, loginService) {
    init();

    function init() {

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
            Estado: 1,
            Login: "",
            State: 1,
            IDConsultorio: $rootScope.sessionDto.IDConsultorio,
        };


        $("#descriptionID").focus();
    }

    $scope.nuevoCuenta = function () {
        prepararNuevaCuenta();
    };

    function cargarCuentasPorCobrar() {
        cuentasService.obtenerCuentasPorCobrarPorConsultorio($rootScope.sessionDto.IDConsultorio).then(function (result) {
            $scope.cuentasPorCobrarConsultorio = result.select(function (cuenta) {
                cuenta.FechaCreacion = moment(cuenta.FechaCreacion).format('DD/MM/YYYY');
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

    $scope.selectUser = function (user) {
        $scope.userSelected = user;
        $scope.userToSave = angular.copy($scope.userSelected);
        $scope.userToSave.State = 2;
        $scope.userToSave.ConfirmPass = angular.copy($scope.userToSave.Password);
        selectRol();
    };

    function selectRol() {
        var selectRolUser = $scope.rolesConsultorio.where(function (item) {
            return item.ID == $scope.userToSave.IDRol;
        });
        $scope.rolSelected = selectRolUser[0];
    }

    $scope.validadUsuario = function () {
        return $scope.userToSave == null || $scope.userToSave.Nombre.length < 4 || $scope.userToSave.Login.length < 4
            || $scope.userToSave.Password.length < 4 || $scope.userToSave.Password != $scope.userToSave.ConfirmPass
            || $scope.rolSelected == null;
    };

    $scope.openModalNewRol = function () {
        $scope.nombrerol = "";
        $scope.message = "";
        $('#new-rol').modal('show');
    };

    $scope.openModalConfirmDelele = function () {
        $('#confirm-delete').modal('show');
    };

    $scope.closeWarnig = function () {
        $scope.userSelected = null;
        $('#confirm-delete').modal('hide');
    };

    $scope.eliminarUsuario = function () {
        usuariosService.eliminarUsuario($scope.userToSave).then(function (result) {
            if (result.Data == 1) {
                $('#confirm-delete').modal('hide');
                cargar_todos_los_usuarios();
                prepararNuevoUsuario();
                toastr.success(result.Message);
            } else {
                toastr.error(result.Message);
            }
        });
    };

    $scope.guardarCuenta = function () {
        $scope.userToSave.IDRol = $scope.rolSelected.ID;
        if ($scope.cuentaParaGuardar.State == 1) {
            cuentasService.insertarCuenta($scope.cuentaParaGuardar).then(function (result) {
                if (result.Data == 1) {
                  toastr.success(result.Message);
                  prepararNuevaCuenta();
                    inicializarDatos();
                } else {
                    toastr.error(result.Message);
                }
            });
        } else {
            cuentasService.modificarCuenta($scope.cuentaParaGuardar).then(function (result) {
                if (result.Data == 1) {
                    inicializarDatos();
                    toastr.success(result.Message);
                    prepararNuevaCuenta();
                } else {
                    toastr.error(result.Message);
                }
            });
        }
    };

    $scope.seleccionarPago = function(pago) {
        $scope.pagoSeleccionado = pago;
    };
});