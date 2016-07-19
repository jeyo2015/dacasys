app.service("cuentasService", function ($http, $q) {
    this.obtenerCuentasPorCobrarPorConsultorio = function (pIdConsultorio) {
        var d = $q.defer();
        $http.get('Cuentas/ObtenerCuentasPorCobrarPorConsultorio?IdConsultorio=' + pIdConsultorio).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.getTrabajosConsultorio = function (pIdConsultorio) {
        var d = $q.defer();
        $http.get('Cuentas/GetTrabajosConsultorio?IdConsultorio=' + pIdConsultorio ).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.eliminarUsuario = function (usario) {
        var d = $q.defer();
        $http.post('Usuarios/EliminarUsuario', { pUsuario: usario }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.insertarUsuario = function (usuario) {
        var d = $q.defer();
        $http.post('Usuarios/InsertarNuevoUsuario', { pUsuario: usuario }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.modificarUsuario = function (usuario) {
        var d = $q.defer();
        $http.post('Usuarios/ModificarUsuario', { pUsuario: usuario }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
});