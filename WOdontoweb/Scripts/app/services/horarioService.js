app.service("horarioService", function ($http, $q) {
    this.obtenerHorariosPorEmpresa = function (idEmpresa) {
        var d = $q.defer();
        $http.get('Horario/ObtenerHorariosPorEmpresa?idEmpresa=' + idEmpresa).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.obtenerHorariosPorDia = function (idDia, idEmpresa) {
        var d = $q.defer();
        $http.get('Horario/ObtenerHorariosPorDia?idDia=' + idDia + '&idEmpresa=' + idEmpresa).success(function (data) {
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
    this.insertarHorario = function (horario) {
        var d = $q.defer();
        $http.post('Horario/InsertarNuevoHorario', { horarioDto: horario }).success(function (data) {
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