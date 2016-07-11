app.service("consultasService", function ($http, $q) {
    function noCache() {
        return '?nochace=' + new Date().getTime();
    }

    function noCacheParameter() {
        return '&nochace=' + new Date().getTime();
    }

    this.getCitasDelDia = function (fecha, idConsultorio, tiempoConsulta) {
        var d = $q.defer();
        $http.get('Consultas/GetCitasDelDia?pfecha=' + fecha + '&pIdConsultorio=' + idConsultorio+'&ptiempoConsulta=' + tiempoConsulta + noCacheParameter()).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.getHistoricoPaciente = function (idPaciente, idConsultorio) {
        var d = $q.defer();
        $http.get('Consultas/GetHistoricoPaciente?pIdPaciente=' + idPaciente + '&pIdConsultorio=' + idConsultorio).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.verificarClienteEnConsultorio = function (idConsultorio, loginCliente) {
        var d = $q.defer();
        $http.get('Consultas/VerificarClienteEnConsultorio?pIdConsultorio=' + idConsultorio + '&pLoginCliente=' + loginCliente).success(function (data) {
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
        var d = $q.defer();
        var fechaSplit = fecha.split('/');
        var fechaConvertida = fechaSplit ? new Date(fechaSplit[2], fechaSplit[1] - 1, fechaSplit[0]) : new Date();

        $http.post('Consultas/InsertarCitaPaciente', { pcita: cita, pFecha: fechaConvertida, pIdCliente: idCliente }).success(function (data) {
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