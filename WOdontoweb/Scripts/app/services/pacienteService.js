app.service("pacienteService", function ($http, $q, $rootScope) {
    this.obtenerClientesPorEmpresa = function (idConsultorio) {
        var d = $q.defer();
        $http.post('Paciente/ObtenerClientesPorEmpresa', { idConsultorio: idConsultorio }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.obtenerPacientesPorCliente = function (loginCliente) {
        var d = $q.defer();
        $http.post('Paciente/ObtenerPacientesPorCliente', { loginCliente: loginCliente }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.eliminarPaciente = function (paciente) {
        var d = $q.defer();
        $http.post('Paciente/EliminarPaciente', { pacienteDto: paciente }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.insertarPaciente = function (paciente) {
        var d = $q.defer();
        $http.post('Paciente/InsertarNuevoPaciente', { pacienteDto: paciente }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.modificarPaciente = function (paciente) {
        var d = $q.defer();
        $http.post('Paciente/ModificarPaciente', { pacienteDto: paciente }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
});