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
    this.obtenerHistoricoFichaCab = function (pIdHistoricoFichaCab) {
        var d = $q.defer();
        $http.post('Consultas/ObtenerHistoricoFichaCab', { pIdHistoricoFichaCab: pIdHistoricoFichaCab }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.guardarHistoricoPaciente = function (pHistorico) {
        var d = $q.defer();
        $http.post('Consultas/GuardarHistoricoPaciente', { pHistorico: pHistorico }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.cerrarFichaHistorico = function (idFichaHistorico, citaAtendida) {
        var d = $q.defer();
        $http.post('Consultas/CerrarFichaHistorico', { idFichaHistorico: idFichaHistorico, citaAtendida: citaAtendida }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.guardarHistoricoCompleto = function (HistoricoFichaCabDto) {
        var d = $q.defer();
        $http.post('Consultas/GuardarHistoricoCompleto', { pHistoricoFichaString: JSON.stringify(HistoricoFichaCabDto) }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.guardarHistoricoFichaDetalle = function (fichaDetalle, citaAtendida) {
        var d = $q.defer();
        $http.post('Consultas/GuardarHistoricoFichaDetalle', { fichaDetalle: JSON.stringify(fichaDetalle), citaAtendida: citaAtendida }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.guardarHistoricoFichaTrabajo = function (fichaTrabajo) {
        var d = $q.defer();
        $http.post('Consultas/GuardarHistoricoFichaTrabajo', { fichaTrabajo:JSON.stringify(fichaTrabajo) }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.guardarHistoricoFichaPaciente = function (pHistoricoFicha) {
        var d = $q.defer();
        $http.post('Consultas/GuardarHistoricoFichaPaciente', { pfichaHistorico: pHistoricoFicha }).success(function (data) {
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
       
        $http.post('Consultas/InsertarCitaPaciente', { pcita: cita, pFechastring: fecha, pIdCliente: idCliente }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.habilitarHora = function (cita) {
        var d = $q.defer();

        $http.post('Consultas/HabilitarHora', { pcita: cita }).success(function (data) {
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
    this.obtenerValoresPeriodontal = function (login) {
        var d = $q.defer();
        $http.get('Consultas/ObtenerValoresPeriodontal').success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.obtenerCriterios = function (login) {
        var d = $q.defer();
        $http.get('Consultas/ObtenerCriterios').success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.obtenerHistoricoFichaTrabajo = function (pIdHistoricoFichaCab) {
        var d = $q.defer();
        $http.get('Consultas/ObtenerHistoricoFichaTrabajo?pIdHistoricoFichaCab=' + pIdHistoricoFichaCab).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.obtenerHistoricoFichaDetalle= function (pIdHistoricoFichaCab) {
        var d = $q.defer();
        $http.get('Consultas/ObtenerHistoricoFichaDetalle?pIdHistoricoFichaCab=' + pIdHistoricoFichaCab).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.obtenerHistoricoFichaCabs = function (pIdPaciente, pIdConsultorio, citaSeleccionada) {
        var d = $q.defer();
        $http.get('Consultas/ObtenerHistoricoFichaCabs?idPaciente=' + pIdPaciente + '&idConsultorio=' + pIdConsultorio + '&citaSeleccionada=' + citaSeleccionada + noCacheParameter()).success(function (data) {
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