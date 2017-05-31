app.controller("loginController", function (loginService, $scope, $rootScope, $location, usuariosService, notificacionesConsultorioService, pacienteService, $compile) {
    init();

    function init() {
        $scope.loginEmpresa = "";
        $scope.usuario = "";
        $rootScope.pass = "";
        $rootScope.isAdmin = false;
        $scope.isUser = -1;
        $scope.newPass = "";
        $scope.ConfirmPass = "";
        $scope.message = "";
        prepararNuevoCliente();
        $scope.pass = "";
        $('#passwordIDLog').attr('autocomplete', 'off');
        cleanInputsf();
    };

    function cleanInputsf() {
        consultorioElemento = $("#formGroupConsultorio");
        consultorioElemento.addClass("is-empty");
        $scope.loginEmpresa = null;
        consultorioElemento = $("#formGroupUsuario");
        consultorioElemento.addClass("is-empty");
        $scope.usuario = null;
        consultorioElemento = $("#formGroupPass");
        consultorioElemento.addClass("is-empty");
        $scope.pass = null;

    }
    $scope.cleanInputs = function () {
        cleanInputsf();

    }
    $scope.cerrarModalRegistrar = function () {
        $("#modal-registrarse").modal('hide');
    }
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
            IsPrincipal: true,
            IDEmpresa: -1
        };
    };
    $scope.validarCamposPaciente = function () {

        if ($scope.pacienteParaGuardar && $scope.pacienteParaGuardar.IsPrincipal) {
            return $scope.pacienteParaGuardar == null || $scope.selectSexo == null || $scope.selectTipoSangre == null
              || $scope.pacienteParaGuardar.Ci.length < 5
        || $scope.pacienteParaGuardar.Nombre.length <= 0 || $scope.pacienteParaGuardar.Apellido.length <= 0 || $scope.pacienteParaGuardar.Email.length <= 0;
        } else {
            return $scope.pacienteParaGuardar == null || $scope.selectSexo == null || $scope.selectTipoSangre == null
            || $scope.pacienteParaGuardar.Nombre.length <= 0 || $scope.pacienteParaGuardar.Apellido.length <= 0 || $scope.pacienteParaGuardar.Email.length <= 0;
        }
    };

    $scope.registrarCliente = function () {

        $scope.pacienteParaGuardar.Sexo = $scope.selectSexo;
        $scope.pacienteParaGuardar.TipoSangre = $scope.selectTipoSangre;
        if ($scope.pacienteParaGuardar.State == 1) {
            pacienteService.insertarPaciente($scope.pacienteParaGuardar).then(function (result) {
                if (result.Data == 1) {
                    toastr.success(result.Message);
                    $("#modal-registrarse").modal('hide');
                } else {
                    toastr.error(result.Message);
                }
            });
        }

    };
    $scope.openModalregistrarCliente = function () {
        $scope.usuario = "";
        $scope.pass = "";
        prepararNuevoCliente();
        $("#modal-login").modal('hide');
        $("#modal-login-cliente").modal('hide');
        $("#modal-registrarse").modal('show');
    }
    $rootScope.enterLogIn = function (keyEvent) {
        if (keyEvent.which === 13)
            $scope.ingresar();
    };
    $rootScope.getClass = function (path) {
        return ($location.path().substr(0, path.length) === path) ? 'active' : '';
    };
    function getNotificaciones() {
        if ($rootScope.sessionDto.IDConsultorio != -1)
            notificacionesConsultorioService.getSolicitudesPacientes($rootScope.sessionDto.IDConsultorio, 1).then(function (result) {
                $rootScope.NotificacionesConsultorio = result;
                if ($rootScope.sessionDto.IDRol != null) {
                    $location.path('/' + $rootScope.primerModulo());
                }
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
        init();
        //$scope.loginEmpresa = "";
        //$scope.usuario = "";
        //$scope.pass = "";
        //$rootScope.isAdmin = true;
        //$scope.isUser = -1;
        //$scope.message = "";
        //if ($rootScope.sessionDto.loginUsuario == "")
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
        return listModulo.length <= 0 ? 'inicioCliente' : listModulo[0].NombreModulo;
    };





    $rootScope.openModalChangePass = function (e) {
        e.preventDefault();
        prepararNuevoPerfil();
        usuariosService.getUsuarioConsultorio($rootScope.sessionDto.loginUsuario, $rootScope.sessionDto.IDConsultorio).then(function (result) {
            $scope.userToSave = result;

            $('#modal-mi-perfil').modal('show');
        });
    };

    $scope.ingresar = function () {
        var verificar = loginService.ingresar($scope.loginEmpresa, $scope.usuario, $scope.pass);

        verificar.then(function (result) {

            $scope.message = result.Message;
            $rootScope.sessionDto = result.Data;
            switch (result.Data.Verificar) {
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

                    $('#modal-login-cliente').modal('hide');
                    $('#modal-login').modal('hide');

                    if ($scope.isAdmin) {
                        if ($rootScope.sessionDto.ChangePass) {
                            $('#modal-renovar').modal('show');
                            $scope.showMessage = false;
                        }
                        getNotificaciones();
                    } else {
                        if ($rootScope.comentarioParaGuardar)
                            $rootScope.comentarioParaGuardar.LoginCliente = $rootScope.sessionDto.loginUsuario;
                        if ($rootScope.sessionDto.ChangePass) {
                            $('#modal-renovar').modal('show');
                            $scope.showMessage = false;
                        }
                    }

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
            return $scope.usuario == null || $scope.pass == null || $scope.loginEmpresa == null || $scope.usuario.length == 0 || $scope.pass.length == 0 || $scope.loginEmpresa.length == 0;
        } else return $scope.usuario == null || $scope.pass == null || $scope.usuario.length == 0 || $scope.pass.length == 0;
    };
    $scope.validarCamposInicioCliente = function () {
        return $scope.usuario == null || $scope.pass == null || $scope.usuario.length == 0 || $scope.pass.length == 0;
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