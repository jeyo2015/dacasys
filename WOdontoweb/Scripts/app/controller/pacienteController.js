app.controller("pacienteController", function (pacienteService, $scope, $rootScope) {
    init();

    function init() {
        $scope.message = "";
        $scope.userSelected = null;
        prepararNuevoCliente();
        cargarClientes();
    };

    function prepararNuevoCliente() {
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

        $scope.diaSelected = null;
        $scope.numeroSelected = null;
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
        $scope.pacienteParaGuardar.IsPrincipal = false;
        $rootScope.LoginCliente = cliente.LoginCliente;
        $scope.clienteSelected = cliente;
        $scope.pacienteParaGuardar = angular.copy($scope.clienteSelected);
        $scope.pacienteParaGuardar.State = 2;
        $scope.selectSexo = cliente.Sexo;
        $scope.selectTipoSangre = cliente.TipoSangre;
        cargarPacientes();
    };

    $scope.seleccionarPaciente = function (paciente) {
        $scope.pacienteParaGuardar.IsPrincipal = false;
        $scope.pacienteSelected = paciente;
        $scope.pacienteSelected.LoginCliente = $rootScope.LoginCliente;
        $scope.pacienteParaGuardar = angular.copy($scope.pacienteSelected);
        $scope.pacienteParaGuardar.State = 2;
        $scope.selectSexo = paciente.Sexo;
        $scope.selectTipoSangre = paciente.TipoSangre;
    };

    function prepararNuevoPaciente() {
        $scope.pacienteParaGuardar = {
            LoginCliente : $rootScope.LoginCliente,
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

        $scope.diaSelected = null;
        $scope.numeroSelected = null;
    }

    $scope.nuevoPaciente = function () {
        prepararNuevoPaciente();
        $scope.pacienteParaGuardar.LoginCliente = $rootScope.LoginCliente;
        $scope.pacienteParaGuardar.IsPrincipal = false;
    };

    $scope.validarCliente = function () {
        return !$scope.pacienteParaGuardar.IsPrincipal;
    }

    $scope.validarCamposPaciente = function () {
        return $scope.pacienteParaGuardar == null || $scope.selectSexo == null || $scope.selectTipoSangre == null
        || $scope.pacienteParaGuardar.Ci.length < 7;
    };

    $scope.openModalConfirmDelele = function () {
        $('#confirm-delete').modal('show');
    };

    $scope.closeWarnig = function () {
        $scope.clienteSelected = null;
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
                toastr.error(result.Message);
            }
        });
    };

    $scope.guardarPaciente = function () {
        $scope.pacienteParaGuardar.Sexo = $scope.selectSexo;
        $scope.pacienteParaGuardar.TipoSangre = $scope.selectTipoSangre;
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
                } else {
                    toastr.error(result.Message);
                }
            });
        }
    };
});