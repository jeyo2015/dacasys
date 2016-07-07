app.service("comentarioService", function ($http, $q, $rootScope) {
    this.obtenerComentariosPorEmpresa = function (idConsultorio) {
        var d = $q.defer();
        $http.get('Paciente/ObtenerComentariosPorEmpresa?idConsultorio=' + idConsultorio).success(function (data) {
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
});