app.service("comentarioService", function ($http, $q, $rootScope) {
    function noCache() {
        return '?nochace=' + new Date().getTime();
    }

    function noCacheParameter() {
        return '&nochace=' + new Date().getTime();
    }
    this.obtenerComentariosPorEmpresa = function (idConsultorio) {
        var d = $q.defer();
        $http.get('Paciente/ObtenerComentariosPorEmpresa?idConsultorio=' + idConsultorio+noCacheParameter()).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.insertarComentario = function (comentario) {
        var d = $q.defer();
        $http.post('Paciente/InsertarNuevoComentario', { comentarioDto: comentario }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.eliminarComentario = function (comentario) {
        var d = $q.defer();
        $http.post('Paciente/EliminarComentario', { comentarioDto: comentario }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
});