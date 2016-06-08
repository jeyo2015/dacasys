app.service("horarioService", function ($http, $q) {
    this.obtenerDias = function () {
        var d = $q.defer();
        $http.get('Horario/ObtenerDias').success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };    
    this.obtenerHorariosPorEmpresa = function (idEmpresa) {
        var d = $q.defer();
        $http.get('Horario/ObtenerHorariosPorEmpresa?idEmpresa=' + idEmpresa).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.eliminarHorario = function (horario) {
        var d = $q.defer();
        $http.post('Horario/EliminarHorario', { horarioDto: horario }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.insertarHorario = function (horario, dias) {
        var d = $q.defer();
        $http.post('Horario/InsertarNuevoHorario', { horarioDto: horario, listaDias: dias }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.modificarHorario = function (horario) {
        var d = $q.defer();
        $http.post('Horario/ModificarHorario', { horarioDto: horario }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
});