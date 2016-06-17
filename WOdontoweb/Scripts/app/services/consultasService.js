app.service("consultasService", function ($http, $q) {
    this.getCitasDelDia = function (fecha, idConsultorio, tiempoConsulta) {
        var d = $q.defer();
        $http.post('Consultas/GetCitasDelDia', { pfecha: fecha, pIdConsultorio: idConsultorio, ptiempoConsulta: tiempoConsulta }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.getHistoricoPaciente = function (idPaciente, idConsultorio) {
        var d = $q.defer();
        $http.post('Consultas/GetHistoricoPaciente', { pIdPaciente: idPaciente, pIdConsultorio: idConsultorio }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.getHistoricoDetalle = function (historicoPaciente) {
        var d = $q.defer();
        $http.post('Consultas/GetHistoricoDetalle', { pHistorico: historicoPaciente }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.aBMHistorico = function (pHistorico) {
        var d = $q.defer();
        $http.post('Consultas/GuardarHistorico', { pHistorico: pHistorico }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.insertarHistoricoDetalle = function (pHistoricoDetalle) {
        var d = $q.defer();
        $http.post('Consultas/InsertarHistoricoDetalle', { pHistoricoDetalle: pHistoricoDetalle }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
     this.insertarCitaPaciente = function (cita, fecha, idCliente) {
        var d = $q.defer();//
        $http.post('Consultas/InsertarCitaPaciente', { pcita: cita,pFecha:fecha,pIdCliente:idCliente  }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.eliminarCitaPaciente = function (cita, libre, motivo) {
        var d = $q.defer();
        $http.post('Consultas/EliminarCitaPaciente', { pcita: cita, pLibre: libre, pMotivo: motivo }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.obtenerCitasPorCliente = function (login) {
        var d = $q.defer();
        $http.get('Consultas/ObtenerCitasPorCliente?loginCliente=' + login).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.cancelarCitaPaciente = function (cita) {
        var d = $q.defer();
        $http.post('Consultas/CancelarCitaPaciente', { idCita: cita }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
});