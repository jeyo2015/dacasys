app.service("clinicaService", function ($http, $q) {
    this.getAllClinicas = function (idEmpresa) {
        var d = $q.defer();
        $http.get('Empresa/ObtenerClinicas').success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };

    this.getAllClinicasHabilitadas = function () {
        var d = $q.defer();
        $http.get('Empresa/ObtenerClinicasHabilitadas').success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.getConsultorioByID = function (idConsultorio) {
        var d = $q.defer();
        $http.get('Empresa/ObtenerConsultorioPorId?idConsultorio=' + idConsultorio).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.getIntervalosTiempo = function (idEmpresa) {
        var d = $q.defer();
        $http.get('Empresa/ObtenerIntervalosDeTiempo').success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.getTrabajosClinica = function (idClinica) {
        var d = $q.defer();
        $http.get('Empresa/ObtenerTrabajosClinica?idClinica=' + idClinica).success(function (data) {
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
        $http.post('Empresa/InsertarNuevoConsultorio', { consultorioDto: consultorio }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.eliminarClinica = function (idClinica) {
        var d = $q.defer();
        $http.post('Empresa/EliminarClinica', { idClinica: idClinica }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.insertarClinica = function (clinica) {
        var d = $q.defer();
        $http.post('Empresa/InsertarNuevaClinica', { clinicaDto: clinica }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.modificarClinica = function (clinica) {
        var d = $q.defer();
        $http.post('Empresa/ModificarClinica', { clinicaDto: clinica }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    
    this.modificarConsultorio = function (consultorio) {
        var d = $q.defer();
        $http.post('Empresa/ModificarConsultorio', { consultorioDto: consultorio }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.obtenerConsultoriosPorCliente = function (login) {
        var d = $q.defer();
        $http.get('Empresa/ObtenerConsultoriosPorCliente?loginCliente=' + login).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
});