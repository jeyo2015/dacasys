app.controller("horarioController", function (horarioService, $scope, $rootScope, loginService) {
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
        prepararNuevoHorario();
        cargarHorarios();
        cargarDias();
    }
    function prepararNuevoHorario() {
        $scope.horarioParaGuardar = {
            NumHorario: 1,
            State: 1,
            IDDia: 0,
            IDHorario: 0,
            NombreDia: '',
            IDEmpresa: $rootScope.sessionDto.IDConsultorio
        };

        $scope.diaSelected = null;
        $scope.numeroSelected = null;
        
        obtenerHoraActual('horaInicio');
        obtenerHoraActual('horaFin');
        $scope.horarioParaGuardar.HoraInicio = $('#horaInicio').val();
        $scope.horarioParaGuardar.HoraFin = $('#horaFin').val();
    }

    function cargarHorarios() {
        var listaHorario = [];
        horarioService.obtenerHorariosPorEmpresa($rootScope.sessionDto.IDConsultorio).then(function (result) {
            $.each(result, function (i, item) {
                var hinicio = item.HoraInicio.split(":");
                var hfinal = item.HoraFin.split(":");
                item.HoraInicio = (hinicio[0] < 10 ? '0' + hinicio[0] : hinicio[0]) + ':' + (hinicio[1] < 10 ? '0' + hinicio[1] : hinicio[1]);
                item.HoraFin = (hfinal[0] < 10 ? '0' + hfinal[0] : hfinal[0]) + ':' + (hfinal[1] < 10 ? '0' + hfinal[1] : hfinal[1]);
                listaHorario.push(item);
            });
        });
        $scope.ListaHorario = listaHorario;
    }

    function cargarDias() {
        horarioService.obtenerDias().then(function (result) {
            $scope.ListaDias = result;
        });
    }

    $scope.nuevoHorario = function () {
        prepararNuevoHorario();
    };

    $scope.openModalConfirmDelele = function () {
        $('#confirm-delete').modal('show');
    };
    
    function obtenerHoraActual(element) {
        var fecha = new Date();
        var hora = fecha.getHours().toString();
        var minuto = fecha.getMinutes().toString();
        $('#' + element).val((hora < 10 ? '0' + hora : hora) + ":" + (minuto < 10 ? '0' + minuto : minuto));
        $('#' + element).timeEntry();
    }

    $scope.seleccionarHorario = function (horario) {
        $scope.horarioSelected = horario;
        $scope.horarioParaGuardar = angular.copy($scope.horarioSelected);
        $scope.horarioParaGuardar.State = 2;
        $scope.numeroSelected = horario.NumHorario;
        $('#horaInicio').val($scope.horarioParaGuardar.HoraInicio);
        $('#horaFin').val($scope.horarioParaGuardar.HoraFin);
        seleccionarDia();
    };

    function seleccionarDia() {
        var selected = $scope.ListaDias.where(function (item) {
            return item.IDDia == $scope.horarioParaGuardar.IDDia;
        });
        $scope.diaSelected = selected[0];
    }

    $scope.validarCamposHorario = function () {
        return $scope.horarioParaGuardar == null || $scope.diaSelected == null || $scope.numeroSelected == null
            || $scope.horarioParaGuardar.HoraInicio.length != 5 || $scope.horarioParaGuardar.HoraFin.length != 5;
    };

    $scope.openModalConfirmDelele = function () {
        $('#confirm-delete').modal('show');
    };

    $scope.closeWarnig = function () {
        $scope.horarioSelected = null;
        $('#confirm-delete').modal('hide');
    };

    $scope.eliminarRegistro = function () {
        horarioService.eliminarHorario($scope.horarioParaGuardar).then(function (result) {
            if (result.Data == 1) {
                cargarHorarios();
                prepararNuevoHorario();
                toastr.success(result.Message);
                $scope.closeWarnig();
            } else {
                toastr.error(result.Message);
            }
        });
    };

    $scope.guardarHorario = function() {
        $scope.horarioParaGuardar.IDDia = $scope.diaSelected.IDDia;
        $scope.horarioParaGuardar.NombreDia = $scope.diaSelected.NombreCorto;
        $scope.horarioParaGuardar.NumHorario = $scope.numeroSelected;
        $scope.horarioParaGuardar.HoraInicio = $('#horaInicio').val();
        $scope.horarioParaGuardar.HoraFin = $('#horaFin').val();
        if ($scope.horarioParaGuardar.State == 1) {
            horarioService.insertarHorario($scope.horarioParaGuardar).then(function(result) {
                if (result.Data == 1) {
                    cargarHorarios();
                    toastr.success(result.Message);
                    prepararNuevoHorario();
                  
                } else {
                    toastr.error(result.Message);
                }
            });
        } else {
            horarioService.modificarHorario($scope.horarioParaGuardar).then(function(result) {
                if (result.Data == 1) {
                    cargarHorarios();
                    toastr.success(result.Message);
                    prepararNuevoHorario();
                    $scope.horarioSelected = null;
                } else {
                    toastr.error(result.Message);
                }
            });
        }
    };
});