app.service("rolesService", function ($http, $q) {
    this.getAllRols = function (idEmpresa) {
        var d = $q.defer();// GetAllRolOfClinic(int idClinic)
        $http.get('/Roles/GetAllRolOfClinic?idClinic=' + idEmpresa).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };//InsertarNuevoRol(string nombreRol) 
    this.insertarRol = function (nombreRol) {
        var d = $q.defer();// GetAllRolOfClinic(int idClinic)
        $http.post('/Roles/InsertarNuevoRol', { nombreRol: nombreRol }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
    this.eliminarRol = function (idRol) {
        var d = $q.defer();
        $http.post('/Roles/EliminarRol', { idRol: idRol }).success(function (data) {
            d.resolve(data);
        });
        return d.promise;
    };
});