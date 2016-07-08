app.service("notificacionesConsultorioService", function ($http, $q) {
    this.getSolicitudesPacientes = function (idConsultorio, idTipoNotificacion) {
        var d = $q.defer();
        $http.get('NotificacionConsultorio/GetSolicitudesPacientes?pIdConsultorio=' + idConsultorio + '&pIDTipoNotificacion=' + idTipoNotificacion).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.deshabilitarNuevasNotificaciones = function (idConsultorio, idTipoNotificacion) {
        var d = $q.defer();// GetAllRolOfClinic(int idClinic)
        $http.post('NotificacionConsultorio/DeshabilitarNuevasNotificaciones', { pIdConsultorio: idConsultorio, pIDTipoNotificacion: idTipoNotificacion }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.aceptarSolicitudPaciente = function (pNotificacion) {
        var d = $q.defer();
        $http.post('NotificacionConsultorio/AceptarSolicitudPaciente', { pNotificacion: pNotificacion }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };

    this.enviarSolicitudConsultorio = function (pNotificacion) {
        var d = $q.defer();
        $http.post('NotificacionConsultorio/EnviarSolicitudConsultorio', { pNotificacion: pNotificacion }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.cancelarSolicitudPaciente = function (pNotificacion) {
        var d = $q.defer();// GetAllRolOfClinic(int idClinic)
        $http.post('NotificacionConsultorio/CancelarSolicitudPaciente', { pNotificacion: pNotificacion }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
});