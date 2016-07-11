app.service("rolesService", function ($http, $q) {
    this.getAllRols = function (idEmpresa) {
        var d = $q.defer();
        $http.get('Roles/GetAllRolOfClinic?idClinic=' + idEmpresa).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.insertarRol = function (nombreRol, IdConsultorio) {
        var d = $q.defer();
        $http.post('Roles/InsertarNuevoRol', { nombreRol: nombreRol, pIdConsultorio: IdConsultorio }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.modificarPermisos = function (modulosTree, pIdrol) {
        var d = $q.defer();
        $http.post('Roles/ModificarPermisos', { modulos: modulosTree, pIDRol: pIdrol }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.eliminarRol = function (idRol) {
        var d = $q.defer();
        $http.post('Roles/EliminarRol', { idRol: idRol }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.getModulos = function (pIdRol) {
        var d = $q.defer();
        $http.get('Roles/GetModulos?pidrol=' + pIdRol).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
});