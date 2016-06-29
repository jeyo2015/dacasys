app.service("loginService", function ($http, $q, $rootScope) {
    this.ingresar = function (loginEmpresa, usuario, pass) {
        var d = $q.defer();//(string nameEmpresa, string usuario, string pass)
        $http.post('Login/Ingresar', { nameEmpresa: loginEmpresa, usuario: usuario, pass: pass }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.forgotPass = function (loginEmpresa, usuario) {
        var d = $q.defer();
        $http.post('Login/ForgotPass', { loginEmpresa: loginEmpresa, loginUsuario: usuario }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.renovarContrasena = function (pIsAdmin, loginUsuario, pPass, ploginConsultorio) {
        var d = $q.defer();
        $http.post('Login/RenovarContrasena', { pIsAdmin: pIsAdmin, loginUsuario: loginUsuario, pPass: pPass, ploginConsultorio: ploginConsultorio }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.getSessionDto = function () {
        var d = $q.defer();
        $http.get('Login/GetSessionDto').success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.cerrarSesion = function () {
        var d = $q.defer();
        $http.get('Login/CerrarSesion').success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
});