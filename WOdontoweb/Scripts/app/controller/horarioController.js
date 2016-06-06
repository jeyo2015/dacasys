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

    $scope.openModalConfirmDelele = function () {
        $('#confirm-delete').modal('show');
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
                    $.each(result, function (i, item) {
                        if (item.IDDia == elemento) {
                            listaHorario.push(item);
                        }
                    });
                });

                $scope.ListaHorario = listaHorario;
            });
        }
        else {
            toastr.warning('Selecciones dias para la busqueda');
        }
    };

    $scope.nuevoHorario = function () {
        obtenerHoraActual('horaInicio');
        obtenerHoraActual('horaFin');
    };

    function obtenerHoraActual(element, factor) {
        var fecha = new Date();
        if (factor) {
            fecha = adicionarTiempo(factor, 'h', fecha);
        }
        var hora = fecha.getHours().toString();
        var minuto = fecha.getMinutes().toString();
        $('#' + element).val((hora < 9 ? '0' + hora : hora) + ":" + (minuto < 9 ? '0' + minuto : minuto));
        $('#' + element).timeEntry();
    }

    $scope.seleccionarHorario = function (horario) {
        $scope.horarioSelected = horario;
        $scope.horarioParaGuardar = angular.copy($scope.horarioSelected);
        $scope.numeroSelected = horario.NumHorario;
        seleccionarDia();
    };

    function seleccionarDia() {
        $.each($scope.ListaDias, function (index, elemento) {
            if (elemento.IDDia == $scope.horarioParaGuardar.IDDia) {
                elemento.IsChecked = true;
            }
            else {
                elemento.IsChecked = false;
            }
        });
    }

    //$scope.validadUsuario = function () {
    //    return $scope.horarioParaGuardar == null || $scope.horarioParaGuardar.Nombre.length < 4 || $scope.horarioParaGuardar.Login.length < 4
    //        || $scope.horarioParaGuardar.Password.length < 4 || $scope.horarioParaGuardar.Password != $scope.horarioParaGuardar.ConfirmPass
    //        || $scope.rolSelected == null;
    //};

    $scope.openModalConfirmDelele = function () {
        var r = confirm("Esta seguro que desea eliminar el registro?");
        if (r == true) {
            eliminarRegistro();
        }
    };

    function eliminarRegistro() {        
        horarioService.eliminarHorario($scope.horarioParaGuardar).then(function (result) {
            if (result.Data == 1) {
                $scope.listarHorario();
                prepararNuevoHorario();
                toastr.success(result.Message);
            } else {
                toastr.error(result.Message);
            }
        });
    };


    //$scope.closeWarnig = function () {
    //    $scope.userSelected = null;
    //    $('#confirm-delete').modal('hide');
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