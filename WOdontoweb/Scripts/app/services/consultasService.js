﻿app.service("consultasService", function ($http, $q, $rootScope) {
    this.getCitasDelDia = function (fecha, idConsultorio, tiempoConsulta) {
        var d = $q.defer();
        $http.post('Consultas/GetCitasDelDia', { pfecha: fecha, pIdConsultorio: idConsultorio, ptiempoConsulta: tiempoConsulta }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };

    this.insertarCitaPaciente = function (cita, fecha, idCliente) {
        var d = $q.defer();
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
});