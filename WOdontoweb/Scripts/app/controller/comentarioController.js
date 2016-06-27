app.controller("comentarioController", function (comentarioService, $scope, $rootScope) {
    init();

    function init() {
        $scope.message = "";
        $scope.userSelected = null;
        prepararNuevoComentario();
        cargarComentarios();
    };

    function prepararNuevoComentario() {
        $scope.comentarioParaGuardar = {
            LoginCliente: 'BB',
            Comentario: '',
            State: 1,
            IsVisible: true,
            IDEmpresa: $rootScope.sessionDto.IDConsultorio
        };
    }

    function cargarComentarios() {
        comentarioService.obtenerComentariosPorEmpresa($rootScope.sessionDto.IDConsultorio).then(function (result) {
            $scope.ListaComentario = result;
            $scope.ListaComentario = result.select(function (comentario) {
                comentario.FechaCreacion = moment(comentario.FechaCreacion).format('DD/MM/YYYY');
                return comentario;
            });
        });
    }

    $scope.validarCampos = function () {
        return $scope.comentarioParaGuardar == null || $scope.comentarioParaGuardar.Comentario.length < 1;
    };

    $scope.guardarComentario = function () {
        if ($scope.comentarioParaGuardar.State == 1) {
            comentarioService.insertarComentario($scope.comentarioParaGuardar).then(function (result) {
                if (result.Data == 1) {
                    cargarComentarios();
                    toastr.success(result.Message);
                    prepararNuevoComentario();
                } else {
                    toastr.error(result.Message);
                }
            });
        }
    };
});