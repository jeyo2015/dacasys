var app = angular.module('WOdontoweb', []);

app.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
        when('/miconsultorio', {
            templateUrl: 'Scripts/app/partials/miconsultorio.html',
            controller: 'miconsultorioController'
        }).
        when('/consultorios', {
            templateUrl: 'Scripts/app/partials/consultorios.html',
            controller: 'consultoriosController'
        }).
           when('/miperfil', {
               templateUrl: 'Scripts/app/partials/miperfil.html',
               controller: 'miperfilController'
           }).
           when('/roles', {
               templateUrl: 'Scripts/app/partials/roles.html',
               controller: 'rolesController'
           }).
           when('/consultas', {
               templateUrl: 'Scripts/app/partials/consultas.html',
               controller: 'consultasController'
           }).
          when('/inicioCliente', {
              templateUrl: 'Scripts/app/partials/inicioCliente.html',
              controller: 'inicioClienteController'
          }).
           when('/usuarios', {
               templateUrl: 'Scripts/app/partials/usuarios.html',
               controller: 'usuariosController'
           }).
        otherwise({
            redirectTo: 'inicioCliente'
        });
  }]);
app.factory('injectorDacasys', ['$q', function ($q) {
    var sessionInjector = {
        request: function (config) {
            config.url = $("#basePath").attr("href") + config.url;
            return config;
        }
    };
    return sessionInjector;
}]);
app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('injectorDacasys');
}]);