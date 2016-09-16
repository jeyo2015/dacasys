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
    this.obtenerConsultorios = function () {
        var d = $q.defer();
        $http.get('Empresa/ObtenerConsultorios').success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.getIntervalosTiempo = function () {
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
    this.eliminarConsultorio = function (pIdConsultorio) {
        var d = $q.defer();
        $http.post('Empresa/EliminarConsultorio', { idConsultorio: pIdConsultorio }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.habilitarConsultorio = function (pIdConsultorio) {
        var d = $q.defer();
        $http.post('Empresa/HabilitarConsultorio', { idConsultorio: pIdConsultorio }).success(function (data) {
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
    this.habilitarClinica = function (idClinica) {
        var d = $q.defer();
        $http.post('Empresa/HabilitarClinica', { idClinica: idClinica }).success(function (data) {
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
    this.obtenerConsultoriosPorClinica = function (pIdClinica) {
        var d = $q.defer();
        $http.get('Empresa/ObtenerConsultoriosPorClinica?pIDClinica=' + pIdClinica).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.obtenerConsultorioConClinica = function (pIdConsultorio) {
        var d = $q.defer();
        $http.get('Empresa/ObtenerConsultorioConClinica?pIdConsultorio=' + pIdConsultorio).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
});