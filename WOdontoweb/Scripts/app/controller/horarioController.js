app.controller("horarioController", function (horarioService, $scope, $rootScope) {
    init();

    function init() {
        $scope.message = "";
        $scope.userSelected = null;
        // prepararNuevoHorario();
    };

    function prepararNuevoHorario() {
        $scope.horarioParaGuardar = {
            HoraInicio: '',
            HoraFin: '',
            NumHorario: 1,
            State: 1,
            IDDia: 0,
            IDHorario: 0,
            NombreDia: '',
            IDEmpresa: $rootScope.sessionDto.IDConsultorio
        };
    }

    $scope.nuevoHorario = function () {
        prepararNuevoHorario();
    };

    $scope.closeModal = function (nameModal) {
        $(nameModal).modal('hide');
    };

    $rootScope.abrirModalHorario = function (e) {
        $scope.idEmpresa = $rootScope.sessionDto.IDConsultorio;
        e.preventDefault();
        prepararNuevoHorario();

        horarioService.obtenerDias().then(function (result) {
            $('#configurar-horarios').modal('show');
            $scope.ListaDias = result;
        });
    };

    $scope.listarHorario = function () {
        var diasCheck = [];
        var listaHorario = [];
        $.each($scope.ListaDias, function (index, elemento) {
            if (elemento.IsChecked) {
                diasCheck.push(elemento.IDDia);
            }
        });

        if (diasCheck.length > 0) {
            horarioService.obtenerHorariosPorEmpresa($rootScope.sessionDto.IDConsultorio).then(function (result) {
                $.each(diasCheck, function (index, elemento) {
                    var listPlanta = result.where(function (item) {
                        return item.IDDia == elemento;
                    });
                    listaHorario.push(listPlanta);
                });

                $scope.ListaHorario = listaHorario;
            });
        }
        else {
            toastr.warning('Selecciones dias para la busqueda');
        }
    }

    //function cargar_todos_los_usuarios() {
    //    var user = horarioService.getAllUsers($rootScope.sessionDto.IDConsultorio).then(function (result) {
    //        $scope.allUsers = result;
    //    });
    //}

    //$scope.selectUser = function (user) {
    //    $scope.userSelected = user;
    //    $scope.horarioParaGuardar = angular.copy($scope.userSelected);
    //    $scope.horarioParaGuardar.State = 2;
    //    $scope.horarioParaGuardar.ConfirmPass = angular.copy($scope.horarioParaGuardar.Password);
    //    selectRol();
    //};

    //function selectRol() {
    //    var selectRolUser = $scope.rolesConsultorio.where(function (item) {
    //        return item.ID == $scope.horarioParaGuardar.IDRol;
    //    });
    //    $scope.rolSelected = selectRolUser[0];
    //}

    //$scope.validadUsuario = function () {
    //    return $scope.horarioParaGuardar == null || $scope.horarioParaGuardar.Nombre.length < 4 || $scope.horarioParaGuardar.Login.length < 4
    //        || $scope.horarioParaGuardar.Password.length < 4 || $scope.horarioParaGuardar.Password != $scope.horarioParaGuardar.ConfirmPass
    //        || $scope.rolSelected == null;
    //};

    //$scope.openModalConfirmDelele = function () {
    //    $('#confirm-delete').modal('show');
    //};

    //$scope.closeWarnig = function () {
    //    $scope.userSelected = null;
    //    $('#confirm-delete').modal('hide');
    //};

    //$scope.eliminarUsuario = function () {
    //    horarioService.eliminarHorario($scope.horarioParaGuardar).then(function (result) {
    //        if (result.Data == 1) {
    //            $('#confirm-delete').modal('hide');
    //            cargar_todos_los_usuarios();
    //            prepararNuevoHorario();
    //            toastr.success(result.Message);
    //        } else {
    //            toastr.error(result.Message);
    //        }
    //    });
    //};

    //$scope.guardarHorario = function () {
    //    $scope.horarioParaGuardar.IDRol = $scope.rolSelected.ID;
    //    if ($scope.horarioParaGuardar.State == 1) {
    //        horarioService.insertarHorario($scope.horarioParaGuardar).then(function (result) {
    //            if (result.Data == 1) {
    //                $scope.allUsers.push(angular.copy($scope.horarioParaGuardar));
    //                toastr.success(result.Message);
    //                prepararNuevoHorario();
    //            } else {
    //                toastr.error(result.Message);
    //            }
    //        });
    //    } else {
    //        horarioService.modificarHorario($scope.horarioParaGuardar).then(function (result) {
    //            if (result.Data == 1) {
    //                cargar_todos_los_usuarios();
    //                toastr.success(result.Message);
    //                prepararNuevoHorario();
    //            } else {
    //                toastr.error(result.Message);
    //            }
    //        });
    //    }
    //};
});