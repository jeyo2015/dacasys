app.service("pacienteService", function ($http, $q, $rootScope) {
    this.getPacienteConsultorio = function (idConsultorio) {
        var d = $q.defer();
        $http.post('Pacientes/GetPacienteConsultorio', { pIdConsultorio: idConsultorio }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };


});