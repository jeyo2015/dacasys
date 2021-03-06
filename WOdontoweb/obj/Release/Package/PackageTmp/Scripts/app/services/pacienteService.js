﻿app.service("pacienteService", function ($http, $q, $rootScope) {
    this.obtenerClientesPorEmpresa = function (idConsultorio) {
        var d = $q.defer();
        $http.get('Paciente/ObtenerClientesPorEmpresa?idConsultorio=' + idConsultorio).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.obtenerPacientesPorCliente = function (loginCliente) {
        var d = $q.defer();
        $http.get('Paciente/ObtenerPacientesPorCliente?loginCliente=' + loginCliente).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.obtenerPacientesPorClienteCita = function (ploginCliente) {
        var d = $q.defer();
        $http.get('Paciente/ObtenerPacientesPorClienteCita?loginCliente=' + ploginCliente).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.obtenerPacientesPorCi = function (pCi) {
        var d = $q.defer();
        $http.get('Paciente/ObtenerPacientesPorCi?pCi=' + pCi).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.obtenerPacientePorId = function (loginCliente) {
        var d = $q.defer();
        $http.get('Paciente/ObtenerPacientesPorId?loginCliente=' + loginCliente).success(function (data) {
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
    this.insertarclienteExistente = function (paciente) {
        var d = $q.defer();
        $http.post('Paciente/InsertarclienteExistente', { pacienteDto: paciente }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.insertarClientePacienteAntiguo = function (paciente) {
        var d = $q.defer();
        $http.post('Paciente/InsertarClientePacienteAntiguo', { pacienteDto: paciente }).success(function (data) {
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