app.service("notificacionesConsultorioService", function ($http, $q) {
    this.getSolicitudesPacientes = function (idConsultorio, idTipoNotificacion) {
        var d = $q.defer();// GetAllRolOfClinic(int idClinic)
        $http.get('NotificacionConsultorio/GetSolicitudesPacientes?pIdConsultorio=' + idConsultorio + '&pIDTipoNotificacion=' + idTipoNotificacion).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };//InsertarNuevoRol(string nombreRol) 
    
});