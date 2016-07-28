app.service("cuentasService", function ($http, $q) {
    this.obtenerCuentasPorCobrarPorConsultorio = function (pIdConsultorio) {
        var d = $q.defer();
        $http.get('Cuentas/ObtenerCuentasPorCobrarPorConsultorio?IdConsultorio=' + pIdConsultorio).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.obtenerCuentasPorPagaPorCliente = function (pLogin) {
        var d = $q.defer();
        $http.get('Cuentas/ObtenerCuentasPorPagaPorCliente?pLogin=' + pLogin).success(function (data) {
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
    this.eliminarPago = function (IdPago) {
        var d = $q.defer();
        $http.post('Cuentas/EliminarPago', { pIdPago: IdPago }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.eliminarCuenta = function (IdCuenta) {
        var d = $q.defer();
        $http.post('Cuentas/EliminarCuenta', { pIdCuenta: IdCuenta }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.insertarNuevaCuenta = function (pCuenta) {
        var d = $q.defer();
        $http.post('Cuentas/InsertarNuevaCuenta', { pCuenta: pCuenta }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.insertarNuevoPago = function (pPago) {
        var d = $q.defer();
        $http.post('Cuentas/InsertarNuevoPago', { pPago: pPago }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.modificarPago = function (pPago) {
        var d = $q.defer();
        $http.post('Cuentas/ModificarPago', { pPago: pPago }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.modificarCuenta = function (pCuenta) {
        var d = $q.defer();
        $http.post('Cuentas/ModificarCuenta', { pCuenta: pCuenta }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
});