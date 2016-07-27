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
            Estado: 1,
            Login: "",
            IDConsultorio: $rootScope.sessionDto.IDConsultorio,
            State: 1
        }
        $scope.cuentaSeleccionada = null;
        $scope.pagoSelecionado = null;
        $scope.trabajoSeleccionado = null;
        $scope.clienteSeleccionado = null;
        $("#descripcionId").focus();
    }
    function prepararNuevoPago() {
      
        $scope.pagoParaGuardar = {
            Descripcion: "",
            Monto: 0,
            IDCuentasPorCobrar: $scope.cuentaSeleccionada.ID,
            State: 1
        }

        $scope.pagoSelecionado = null;
        $("#descripcionId").focus();
    }

    $scope.nuevaCuenta = function () {
        prepararNuevaCuenta();
    };

    function cargarCuentasPorCobrar() {
        cuentasService.obtenerCuentasPorCobrarPorConsultorio($rootScope.sessionDto.IDConsultorio).then(function (result) {
            $scope.cuentasPorCobrarConsultorio = result.select(function (cuenta) {
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
    /*
    USE [db_dentista]
GO


    SET ANSI_NULLS ON
    GO

    SET QUOTED_IDENTIFIER ON
    GO

    CREATE TABLE [dbo].[CuentasPorCobrar](
        [Descripcion] [text] NOT NULL,
        [Monto] [decimal](18, 3) NOT NULL,
        [FechaRegistro] [date] NOT NULL,
        [Login] [nvarchar](50) NOT NULL,
        [IDTrabajo] [int] NOT NULL,
        [Saldo] [decimal](18, 3) NOT NULL,
        [IDConsultorio] [int] NOT NULL,
        [Estado] [int] NOT NULL,
        [ID] [int] IDENTITY(1,1) NOT NULL,
     CONSTRAINT [PK_CuentasPorCobrar] PRIMARY KEY CLUSTERED 
    (
        [ID] ASC
    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

    GO
    USE [db_dentista]
GO


    SET ANSI_NULLS ON
    GO

    SET QUOTED_IDENTIFIER ON
    GO

    CREATE TABLE [dbo].[Pago](
        [ID] [int] IDENTITY(1,1) NOT NULL,
        [IDCuentaPorCobrar] [int] NOT NULL,
        [Descripcion] [text] NOT NULL,
        [Monto] [decimal](18, 3) NOT NULL,
        [FechaPago] [date] NOT NULL,
     CONSTRAINT [PK_Pago] PRIMARY KEY CLUSTERED 
    (
        [ID] ASC
    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

    GO





    */
    $scope.seleccionarCuenta = function (cuenta) {
        debugger;
        $scope.cuentaSeleccionada = cuenta;
        $scope.cuentaParaGuardar = angular.copy($scope.cuentaSeleccionada);
        $scope.cuentaParaGuardar.State = 2;
        selectTrabajo();
        selectCliente();
    };
    $scope.seleccionarPago = function (pago) {
        $scope.pagoSeleccionado = pago;
        $scope.pagoParaGuardar = angular.copy($scope.pagoSeleccionado);
        $scope.pagoParaGuardar.State = 2;

    };
    function selectTrabajo() {
        var selectTrabajo = $scope.trabajosConsultorio.where(function (item) {
            return item.ID == $scope.cuentaParaGuardar.IDTrabajo;
        });
        $scope.trabajoSeleccionado = selectTrabajo[0];
    }
    function selectCliente() {
        debugger;
        var selectCliente = $scope.clientesConsultorio.where(function (item) {
            return item.LoginCliente == $scope.cuentaParaGuardar.Login;
        });
        $scope.clienteSeleccionado = selectCliente[0];
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
      
        $('#nuevo-pago').modal('show');
    };
    
    $scope.openModalConfirmDelele = function () {
        $('#eliminar-pago').modal('show');
    };

    $scope.closeWarnig = function (modal) {
        $scope.pagoSeleccionado = null;

        $(modal).modal('hide');
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

    $scope.guardarNuevoPago = function () {
        
        if ($scope.pagoParaGuardar.State == 1) {
            cuentasService.insertarNuevoPago($scope.pagoParaGuardar).then(function (result) {
                if (result.Data) {
                    inicializarDatos();
                    toastr.success(result.Message);
                    prepararNuevaCuenta();
                    $('#nuevo-pago').modal('hide');
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
                } else {
                    toastr.error(result.Message);
                }
            });
        }
    };

    $scope.guardarCuenta = function () {
        debugger;
        $scope.cuentaParaGuardar.IDTrabajo = $scope.trabajoSeleccionado.ID;
        $scope.cuentaParaGuardar.Login = $scope.clienteSeleccionado.LoginCliente;
        if ($scope.cuentaParaGuardar.State == 1) {
            cuentasService.insertarNuevaCuenta($scope.cuentaParaGuardar).then(function (result) {
                if (result.Data) {
                    inicializarDatos();
                    toastr.success(result.Message);
                    prepararNuevaCuenta();
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
                } else {
                    toastr.error(result.Message);
                }
            });
        }
    };
    $scope.eliminarPago = function () {
        cuentasService.eliminarPago($scope.pagoSeleccionado.ID).then(function (result) {
            if (result.Data) {
                inicializarDatos();
                toastr.success(result.Message);
                $('#nuevo-pago').modal('hide');
            } else {
                toastr.error(result.Message);
            }
        });
    }
    //$scope.seleccionarPago = function (pago) {
    //    $scope.pagoSeleccionado = pago;
    //};
});