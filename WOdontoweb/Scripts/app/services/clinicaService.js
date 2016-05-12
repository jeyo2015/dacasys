app.service("clinicaService", function ($http, $q) {
    this.getAllClinicas = function (idEmpresa) {
        var d = $q.defer();
        $http.get('Empresa/GetAllClinicas').success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };

    this.getAllClinicasHabilitadas = function () {
        var d = $q.defer();
        $http.get('Empresa/GetAllClinicasHabilitadas').success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };

    this.getIntervalosTiempo = function (idEmpresa) {
        var d = $q.defer();
        $http.get('Empresa/GetIntervalosTiempo').success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };//GetTrabajosClinica(int pIdClinica)
    this.getTrabajosClinica = function (idClinica) {
        var d = $q.defer();
        $http.get('Empresa/GetTrabajosClinica?pIdClinica=' + idClinica).success(function (data) {
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
    this.insertarConsultorio = function (consultorio) {
        var d = $q.defer();
        $http.post('Empresa/InsertarNuevoConsultorio', { pConsultorio: consultorio }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.eliminarClinica = function (pIDclinica) {
        var d = $q.defer();
        $http.post('Empresa/EliminarClinica', { pIDClinica: pIDclinica }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.insertarClinica = function (clinica) {
        var d = $q.defer();
        $http.post('Empresa/InsertarNuevaClinica', { pClinica: clinica }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.modificarClinica = function (clinica) {
        var d = $q.defer();
        $http.post('Empresa/ModificarClinica', { pClinica: clinica }).success(function (data) {
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