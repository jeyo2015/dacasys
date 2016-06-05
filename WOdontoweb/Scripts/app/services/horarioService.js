app.service("horarioService", function ($http, $q) {
    this.obtenerHorariosPorEmpresa = function (idEmpresa) {
        var d = $q.defer();
        $http.get('Horario/ObtenerHorariosPorEmpresa?idEmpresa=' + idEmpresa).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.obtenerDias = function () {
        var d = $q.defer();
        $http.get('Horario/ObtenerDias').success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };    
    this.listarHorarioPorEmpresa = function (idEmpresa, listDia) {
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