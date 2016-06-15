app.controller("miConsultorioController", function (clinicaService, comentarioService, $scope, $compile, $rootScope) {
    init();

    function init() {
        cargarConsultorioPorCliente();
    };

    function cargarConsultorioPorCliente() {
        clinicaService.obtenerConsultoriosPorCliente($rootScope.sessionDto.loginUsuario).then(function (result) {
            $scope.ListaConsultorio = result;
        });
    }

    $scope.openModalComentario = function () {
        prepararNuevoComentario();
        $('#modal-mi-comentario').modal('show');
    };

    $scope.cerrarModalComentario = function () {        
        $('#modal-mi-comentario').modal('hide');
    };

    $scope.openModalCita = function () {
        $('#modal-mi-cita').modal('show');
    };

    $scope.cerrarModalCita = function () {
        $('#modal-mi-cita').modal('hide');
    };

    $scope.seleccionarConsultorio = function (consultorio) {
        $scope.miConsultorioSelected = consultorio;
    };



    function prepararNuevoComentario() {
        $scope.comentarioParaGuardar = {
            LoginCliente: $rootScope.sessionDto.loginUsuario,
            Comentario: '',
            State: 1,
            IsVisible: true,
            IDEmpresa: $scope.miConsultorioSelected.IDEmpresa
        };
    }

    $scope.validarCamposComentario = function () {
        return $scope.comentarioParaGuardar == null || $scope.comentarioParaGuardar.Comentario.length < 1;
    };

    $scope.guardarComentario = function () {
        if ($scope.comentarioParaGuardar.State == 1) {
            comentarioService.insertarComentario($scope.comentarioParaGuardar).then(function (result) {
                if (result.Data == 1) {
                    toastr.success(result.Message);
                    prepararNuevoComentario();
                    $scope.cerrarModalComentario();
                } else {
                    toastr.error(result.Message);
                }
            });
        }
    };
});