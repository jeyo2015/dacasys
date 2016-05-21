app.controller("loginController", function (loginService, $scope, $rootScope, $location, usuariosService, notificacionesConsultorioService) {
    init();

    function init() {
        $scope.loginEmpresa = "";
        $scope.usuario = "";
        $scope.pass = "";
        $scope.isAdmin = true;
        $scope.isUser = -1;
        $scope.newPass = "";
        $scope.ConfirmPass = "";
        $scope.message = "";
        loginService.getSessionDto().then(function (result) {
            $rootScope.sessionDto = result;
            if ($rootScope.sessionDto.ChangePass) {
                $scope.showMessage = false;
                $('#modal-renovar').modal('show');
                prepararNuevoPerfil();
            }
            getNotificaciones();

        });
    };

    function getNotificaciones() {
        if ($rootScope.sessionDto.IDConsultorio != -1)
            notificacionesConsultorioService.getSolicitudesPacientes($rootScope.sessionDto.IDConsultorio, 1).then(function (result) {
                $rootScope.NotificacionesConsultorio = result;
            });
    }

    $rootScope.cerrarSesion = function (e) {
        e.preventDefault();
        loginService.cerrarSesion().then(function (result) {
            $rootScope.sessionDto = result;
            $location.path('/inicioCliente');
        });
    };
    
    $rootScope.showModal = function (e) {
        e.preventDefault();
        $scope.loginEmpresa = "";
        $scope.usuario = "";
        $scope.pass = "";
        $scope.isAdmin = true;
        $scope.isUser = -1;
        $scope.message = "";
        if ($rootScope.sessionDto.loginUsuario == "")
            $('#modal-login').modal('show');
    };
    
    function prepararNuevoPerfil() {
        $scope.userToSave = {
            Nombre: angular.copy($rootScope.sessionDto.Nombre),
            Login: angular.copy($rootScope.sessionDto.Login),
            Password: "",
            ConfirmPass: "",
            IDEmpresa: angular.copy($rootScope.sessionDto.IDConsultorio),
            IDRol: angular.copy($rootScope.sessionDto.IDRol),
            Estado: true
        };
    }

    $rootScope.openModalChangePass = function (e) {
        e.preventDefault();
        prepararNuevoPerfil();
        usuariosService.getUsuarioConsultorio($rootScope.sessionDto.loginUsuario, $rootScope.sessionDto.IDConsultorio).then(function (result) {
            $scope.userToSave = result;

            $('#modal-mi-perfil').modal('show');
        });
    };
    
    $scope.ingresar = function () {
        var verificar = loginService.logIn($scope.loginEmpresa, $scope.usuario, $scope.pass);

        verificar.then(function (result) {
            $rootScope.sessionDto = result.Data;
            $scope.message = result.Message;

            switch ($rootScope.sessionDto.Verificar) {
                case 1:
                    $("#consultorioID").focus();
                    $scope.loginEmpresa = "";
                    $scope.usuario = "";
                    $scope.pass = "";
                    break;
                case 0:
                    $("#consultorioID").focus();
                    $scope.loginEmpresa = "";
                    $scope.usuario = "";
                    $scope.pass = "";
                    break;
                case 2:
                    $("#usuarioID").focus();
                    $scope.usuario = "";
                    $scope.pass = "";
                    break;
                case 4:
                    $("#passwordID").focus();

                    $scope.pass = "";
                    break;
                case 3:
                    $('#modal-login').modal('hide');
                    if ($rootScope.sessionDto.ChangePass) {
                        $('#modal-renovar').modal('show');
                        $scope.showMessage = false;
                    }
                    getNotificaciones();
                    break;
            }

        });
    };

    $scope.renovarContrasena = function () {
        if ($scope.isAdmin) {
            if ($scope.loginEmpresa.length == 0) {
                $scope.message = "Ingrese consultorio por favor";
                $rootScope.sessionDto.Verificar = 1;
                $("#consultorioID").focus();
                return;
            }
        }
        if ($scope.usuario.length == 0) {
            $scope.message = "Ingrese su login de usuario por favor";
            $rootScope.sessionDto.Verificar = 2;
            $("#usuarioID").focus();
            return;
        }

        loginService.forgotPass($scope.loginEmpresa, $scope.usuario).then(function (result) {
            $scope.message = result.Message;

            switch (result.Data) {
                case 3:
                    toastr.success(result.Message);
                    $scope.loginEmpresa = "";
                    $scope.usuario = "";
                    $scope.pass = "";
                    $('#modal-login').modal('hide');
                    break;
                case 2:
                    toastr.error(result.Message);
                    $scope.loginEmpresa = "";
                    $scope.usuario = "";
                    $scope.pass = "";
                    break;
                case 1:
                    $scope.sessionDto.Verificar = 2;

                    $("#usuarioID").focus();
                    $scope.usuario = "";
                    $scope.pass = "";
                    break;
                case 0:
                    $scope.sessionDto.Verificar = 1;
                    $("#consultorioID").focus();
                    $scope.loginEmpresa = "";
                    $scope.usuario = "";
                    $scope.pass = "";
                    break;
            }
        });
    };
    
    $scope.changePassUser = function () {
        if ($scope.newPass != $scope.ConfirmPass) {
            $scope.message = "Las contrasenas no coinciden";
            $scope.newPass = "";
            $scope.ConfirmPass = "";
            $scope.showMessage = true;
            $("#newPasswordID").focus();
            return;
        }
        if ($scope.newPass.length <= 3) {
            $scope.message = "Contrasena muy corta, debe ser mayor a 4 caracteres";
            $scope.newPass = "";
            $scope.ConfirmPass = "";
            $scope.showMessage = true;
            $("#newPasswordID").focus();
            return;
        }

        loginService.renovarContrasena($scope.isAdmin, $scope.usuario, $scope.newPass, $scope.loginEmpresa).then(function (result) {
            if (result.Data == 1) {
                toastr.success(result.Message);
            } else {
                toastr.error(result.Message);
            }
            $('#modal-renovar').modal('hide');
            $scope.newPass = "";
            $scope.ConfirmPass = "";
        });
    };
    
    $scope.updateUser = function () {
        if ($scope.userToSave.ConfirmPass != $scope.userToSave.Password) {
            $scope.message = "Las contrasenas no coinciden";
            $scope.userToSave.Password = "";
            $scope.userToSave.ConfirmPass = "";
            $scope.showMessage = true;
            $("#newPasswordID").focus();
            return;
        }
        if ($scope.userToSave.Password.length <= 3) {
            $scope.message = "Contrasena muy corta, debe ser mayor a 4 caracteres";
            $scope.userToSave.Password = "";
            $scope.userToSave.ConfirmPass = "";
            $scope.showMessage = true;
            $("#newPasswordID").focus();
            return;
        }

        usuariosService.modificarUsuario($scope.userToSave).then(function (result) {
            if (result.Data == 1) {
                toastr.success(result.Message);
                $rootScope.sessionDto.Nombre = angular.copy($scope.userToSave.Nombre);
            } else {
                toastr.error(result.Message);
            }
            $('#modal-mi-perfil').modal('hide');
            prepararNuevoPerfil();
        });
    };
    
    $scope.validarCampos = function () {
        if ($scope.isAdmin) {
            return $scope.usuario.length == 0 || $scope.pass.length == 0 || $scope.loginEmpresa == 0;
        } else return $scope.usuario.length == 0 || $scope.pass.length == 0;
    };
    
    $scope.validarCamposPerfil = function () {
        if ($scope.userToSave) {
            return $scope.userToSave.Nombre.length == 0 || $scope.userToSave.Password.length == 0 || $scope.userToSave.ConfirmPass.length == 0;
        } else return false;
    };

    $scope.closeModal = function (nameModal) {
        $(nameModal).modal('hide');
    };
    
    $rootScope.desabilitarNuevasNotificaciones = function (e) {
        e.preventDefault();
        notificacionesConsultorioService.deshabilitarNuevasNotificaciones($rootScope.sessionDto.IDConsultorio, 1).then(function (result) {
            if (result.Success)
                $rootScope.NotificacionesConsultorio = result.Data;
        });
    };

    $rootScope.confirmarSolicitud = function (notificacion, e) {
        e.preventDefault();
        notificacionesConsultorioService.aceptarSolicitudPaciente(notificacion).then(function (result) {
            if (result.Success)
                $rootScope.NotificacionesConsultorio = result.Data;
        });
    };
    
    $rootScope.cancelarSolicitud = function (notificacion, e) {
        e.preventDefault();
        notificacionesConsultorioService.cancelarSolicitudPaciente(notificacion).then(function (result) {
            if (result.Success)
                $rootScope.NotificacionesConsultorio = result.Data;
        });
    };
});