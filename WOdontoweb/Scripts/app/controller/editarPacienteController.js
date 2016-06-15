app.controller("editarPacienteController", function (pacienteService, $scope, $rootScope) {
    function prepararDtoPaciente() {
        $scope.editarPacienteParaGuardar = {
            IDEmpresa: $rootScope.sessionDto.IDConsultorio
        };
        $scope.selectSexoEditarPaciente = null;
        $scope.selectTipoSangreEditarPaciente = null;
        $scope.selectPassword = null;
    }

    $scope.validarCamposEditarPaciente = function () {
        return $scope.editarPacienteParaGuardar == null || $scope.selectSexoEditarPaciente == null || $scope.selectTipoSangreEditarPaciente == null
             || $scope.editarPacienteParaGuardar.Nombre.length <= 0 || $scope.editarPacienteParaGuardar.Apellido.length <= 0
             || $scope.editarPacienteParaGuardar.Ci.length < 7 || $scope.editarPacienteParaGuardar.Email.length <= 0
    || ($scope.selectPassword != null && $scope.selectPassword.length < 4);
    };

    $scope.modificarEditarPaciente = function () {
        $scope.editarPacienteParaGuardar.TipoSangre = $scope.selectTipoSangreEditarPaciente;
        $scope.editarPacienteParaGuardar.Sexo = $scope.selectSexoEditarPaciente;
        $scope.editarPacienteParaGuardar.Password = $scope.selectPassword;
        pacienteService.modificarPaciente($scope.editarPacienteParaGuardar).then(function (result) {
            if (result.Data == 1) {
                toastr.success(result.Message);
            } else {
                toastr.error(result.Message);
            }
        });
    };

    $rootScope.openModalEditarPaciente = function (e) {
        e.preventDefault();
        prepararDtoPaciente();
        pacienteService.obtenerPacientePorId($rootScope.sessionDto.loginUsuario).then(function (result) {
            $scope.editarPacienteParaGuardar = result;
            $scope.selectSexoEditarPaciente = result.Sexo;
            $scope.selectTipoSangreEditarPaciente = result.TipoSangre;
            $('#modal-editar-paciente').modal('show');
        });
    };

    $scope.closeModal = function (nameModal) {
        $(nameModal).modal('hide');
    };
});