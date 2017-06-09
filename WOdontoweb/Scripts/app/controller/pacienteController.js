app.controller("pacienteController", function (pacienteService, $scope, $rootScope, loginService) {
    init();

    function init() {
        $scope.message = "";
        $scope.userSelected = null;
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

        prepararNuevoCliente();
        $scope.pacienteParaGuardar.IsPrincipal = true;
        cargarClientes();
    }
    function prepararNuevoCliente() {
        $scope.userSelected = null;
        $scope.clienteDeotraEmpresa = false;
        $scope.pacienteDeotraEmpresa = false;
        $scope.agregarPaciente = false;
        $scope.pacienteParaGuardar = {
            LoginCliente: '',
            Nombre: '',
            Apellido: '',
            Ci: '',
            Telefono: '',
            Email: '',
            Direccion: '',
            TipoSangre: '',
            Sexo: '',
            Antecedentes: '',
            State: 1,
            IdPaciente: 0,
            IsPrincipal: false,
            IDEmpresa: $rootScope.sessionDto.IDConsultorio
        };
    }

    function cargarClientes() {
        pacienteService.obtenerClientesPorEmpresa($rootScope.sessionDto.IDConsultorio).then(function (result) {
            $scope.ListaCliente = result;
        });
    }

    function cargarPacientes() {
        pacienteService.obtenerPacientesPorCliente($scope.pacienteParaGuardar.LoginCliente).then(function (result) {
            $scope.ListaPaciente = result;
        });
    }

    $scope.nuevoCliente = function () {
        prepararNuevoCliente();
        $scope.pacienteParaGuardar.IsPrincipal = true;
    };

    $scope.openModalConfirmDelele = function () {
        $('#confirm-delete').modal('show');
    };

    $scope.seleccionarCliente = function (cliente) {
        $scope.pacienteSelected = null;
        $rootScope.LoginCliente = cliente.LoginCliente;
        $scope.clienteSelected = cliente;
        $scope.pacienteParaGuardar = angular.copy($scope.clienteSelected);
        $scope.pacienteParaGuardar.State = 2;
        $scope.selectSexo = cliente.Sexo;
        $scope.selectTipoSangre = cliente.TipoSangre;
        $scope.pacienteParaGuardar.IsPrincipal = true;
        cargarPacientes();
    };

    $scope.seleccionarPaciente = function (paciente) {
        $scope.pacienteSelected = paciente;
        $scope.pacienteSelected.LoginCliente = $rootScope.LoginCliente;
        $scope.pacienteParaGuardar = angular.copy($scope.pacienteSelected);
        $scope.pacienteParaGuardar.State = 2;
        $scope.selectSexo = paciente.Sexo;
        $scope.pacienteParaGuardar.IsPrincipal = false;
        $scope.selectTipoSangre = paciente.TipoSangre;
    };

    function prepararNuevoPaciente() {
        console.log($scope.clienteSelected);
        $scope.agregarPaciente = true;
        $scope.pacienteParaGuardar = {
            LoginCliente: $rootScope.LoginCliente,
            Nombre: '',
            Apellido: '',
            Ci: '',
            Telefono: '',
            Email: '',
            Direccion: '',
            TipoSangre: '',
            Sexo: '',
            Antecedentes: '',
            State: 1,
            IdPaciente: 0,
            IsPrincipal: false,
            IDEmpresa: $rootScope.sessionDto.IDConsultorio
        };
        $scope.selectSexo = null;
        $scope.selectTipoSangre = null;
    }

    $scope.nuevoPaciente = function () {
        prepararNuevoPaciente();
        $scope.pacienteParaGuardar.LoginCliente = $rootScope.LoginCliente;
        $scope.pacienteParaGuardar.IsPrincipal = false;
    };

    $scope.validarCliente = function () {
        return $scope.pacienteParaGuardar && $scope.pacienteParaGuardar.IsPrincipal && $scope.pacienteParaGuardar.State == 1;
    };

    $scope.validarCamposPaciente = function () {
        if ($scope.pacienteParaGuardar && $scope.pacienteParaGuardar.IsPrincipal && !$scope.agregarPaciente) {
            return $scope.pacienteParaGuardar == null || $scope.selectSexo == null || $scope.selectTipoSangre == null
       
        || $scope.pacienteParaGuardar.Nombre.length <= 0 || $scope.pacienteParaGuardar.Apellido.length <= 0
                || $scope.pacienteParaGuardar.Email.length <= 0 || !$rootScope.validarPermisoComponente('btnModificarCliente');
        } else {
            return $scope.pacienteParaGuardar == null || $scope.selectSexo == null || $scope.selectTipoSangre == null
            || $scope.pacienteParaGuardar.Nombre.length <= 0 || $scope.pacienteParaGuardar.Apellido.length <= 0
                || !$rootScope.validarPermisoComponente('btnModificarPaciente');
        }
    };

    $scope.openModalConfirmDelele = function () {
        $('#confirm-delete').modal('show');
    };

    $scope.closeWarnig = function () {
        $scope.clienteSelected = null;
        $scope.pacienteSelected = null;
        $('#confirm-delete').modal('hide');
    };

    $scope.eliminarRegistro = function () {
        pacienteService.eliminarPaciente($scope.pacienteParaGuardar).then(function (result) {
            if (result.Data == 1) {
                cargarClientes();
                prepararNuevoCliente();
                toastr.success(result.Message);
                $scope.closeWarnig();
            } else {
                if(result.Data==0)
                    toastr.error(result.Message);
                else {
                    cargarClientes();
                    prepararNuevoCliente();
                    toastr.warning(result.Message);
                    $scope.closeWarnig();
                }
            }
        });
    };

    $scope.guardarPaciente = function () {
        $scope.pacienteParaGuardar.Sexo = $scope.selectSexo;
        $scope.pacienteParaGuardar.TipoSangre = $scope.selectTipoSangre;
        //$scope.clienteDeotraEmpresa = result.LoginCliente == "" ? false : true;
        //$scope.pacienteDeotraEmpresa = true;
        if ($scope.clienteDeotraEmpresa) {
            pacienteService.insertarclienteExistente($scope.pacienteParaGuardar).then(function (result) {
                if (result.Data == 1) {
                    cargarClientes();
                    toastr.success(result.Message);
                    prepararNuevoCliente();
                } else {
                    toastr.error(result.Message);
                }
            });
            return;
        }
        if ($scope.pacienteDeotraEmpresa) {
            pacienteService.insertarClientePacienteAntiguo($scope.pacienteParaGuardar).then(function (result) {
                if (result.Data == 1) {
                    cargarClientes();
                    toastr.success(result.Message);
                    prepararNuevoCliente();
                } else {
                    toastr.error(result.Message);
                }
            });
            return;
        }

        if ($scope.pacienteParaGuardar.State == 1) {
            pacienteService.insertarPaciente($scope.pacienteParaGuardar).then(function (result) {
                if (result.Data == 1) {
                    cargarClientes();
                    toastr.success(result.Message);
                    prepararNuevoCliente();
                } else {
                    toastr.error(result.Message);
                }
            });
        } else {
            pacienteService.modificarPaciente($scope.pacienteParaGuardar).then(function (result) {
                if (result.Data == 1) {
                    cargarClientes();
                    toastr.success(result.Message);
                    prepararNuevoCliente();
                    $scope.clienteSelected = null;
                    $scope.pacienteSelected = null;
                } else {
                    toastr.error(result.Message);
                }
            });
        }
    };

    $('#inputCi').blur(function () {
        buscarPaciente();
        $scope.$apply();
    });

    function buscarPaciente() {

        var ciTempo = angular.copy($scope.pacienteParaGuardar.Ci);
        pacienteService.obtenerPacientesPorCi($scope.pacienteParaGuardar.Ci).then(function (result) {
          
            if (result.length == 0) {
                prepararNuevoCliente();

                $scope.pacienteParaGuardar.IsPrincipal = true;
                $scope.pacienteParaGuardar.Ci = ciTempo;
                return;
            }
            if (result != null) {
                var existCliente = $scope.ListaCliente.where(function (item) {
                    return item.LoginCliente == result.LoginCliente;
                });
                if (existCliente.length == 1) {
                    toastr.warning("Este usuario ya se encuentra registrado en su consultorio");
                    prepararNuevoCliente();
                    return;
                }

                $scope.pacienteParaGuardar.Nombre = result.Nombre;
                $scope.pacienteParaGuardar.Apellido = result.Apellido;
                $scope.pacienteParaGuardar.Direccion = result.Direccion;
                $scope.pacienteParaGuardar.Email = result.Email;
                $scope.pacienteParaGuardar.Antecedentes = result.Antecedentes;
                $scope.pacienteParaGuardar.Telefono = result.Telefono;
                $scope.selectSexo = result.Sexo;
                $scope.selectTipoSangre = result.TipoSangre;
                $scope.pacienteParaGuardar.LoginCliente = result.LoginCliente;
                $scope.pacienteParaGuardar.IdPaciente = result.IdPaciente;
                $scope.clienteDeotraEmpresa = result.LoginCliente == "" ? false : true;
                $scope.pacienteDeotraEmpresa = true;
            }
        });
    }
});