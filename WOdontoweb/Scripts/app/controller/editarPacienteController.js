app.controller("editarPacienteController", function (pacienteService, $scope, $rootScope) {

    $rootScope.mostrarMenu = true;
    $rootScope.validarPermisoModulo = function (nombreModulo) {
        if ($rootScope.sessionDto && $rootScope.sessionDto.Permisos) {
            var listModulo = $rootScope.sessionDto.Permisos.Modulos.where(function (modulo) {
                return modulo.NombreModulo == nombreModulo;
            });
            return listModulo.length <= 0 ? false : listModulo[0].TienePermiso;
        } else {
            return false;
        }
    };

    $rootScope.validarPermisoFormulario = function (nombreFormulario) {
        if ($rootScope.sessionDto && $rootScope.sessionDto.Permisos) {
            var listFormulario = $rootScope.sessionDto.Permisos.Formularios.where(function (formulario) {
                return formulario.NombreFormulario == nombreFormulario;
            });
            return listFormulario.length <= 0 ? false : listFormulario[0].TienePermiso;
        } else {
            return false;
        }
    };

    $rootScope.validarPermisoComponente = function (nombreComponente) {

        if ($rootScope.sessionDto && $rootScope.sessionDto.Permisos) {
            var listComponente = $rootScope.sessionDto.Permisos.Componentes.where(function (componente) {
                return componente.NombreComponente == nombreComponente;
            });
            return listComponente.length <= 0 ? false : listComponente[0].TienePermiso;
        } else {
            return false;
        }
    };
    $rootScope.primerModulo = function () {
        var listModulo = $rootScope.sessionDto.Permisos.Modulos.where(function (modulo) {
            return modulo.TienePermiso === true;
        });
        return listModulo.length <= 0 ? 'inicioBotonera' : listModulo[0].NombreModulo;
    };


    $rootScope.primerModulo = function () {
        var listModulo = $rootScope.sessionDto.Permisos.Modulos.where(function (modulo) {
            return modulo.TienePermiso === true;
        });
        return listModulo.length <= 0 ? 'inicioBotonera' : listModulo[0].NombreModulo;
    };

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
                $('#modal-editar-paciente').modal('hide');
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