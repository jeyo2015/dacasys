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
           when('/horario', {
               templateUrl: 'Scripts/app/partials/horario.html'
           }).
           when('/miConsultorioPerfil', {
               templateUrl: 'Scripts/app/partials/miConsultorioPerfil.html',
               controller: 'empresaController'
           }).
          when('/paciente', {
              templateUrl: 'Scripts/app/partials/paciente.html'
          }).
          when('/comentario', {
              templateUrl: 'Scripts/app/partials/comentario.html'
          }).
          when('/miConsultorio', {
              templateUrl: 'Scripts/app/partials/miConsultorio.html'
          }).
          when('/misCitas', {
              templateUrl: 'Scripts/app/partials/misCitas.html'
          }).
      when('/consultas', {
          templateUrl: 'Scripts/app/partials/consultas.html',
          controller: 'consultasController'
      }).
        otherwise({
            redirectTo: 'inicioCliente'
        });
  }]);

app.factory('injectorDacasys', ['$rootScope', '$q', function ($rootScope, $q) {
    var numLoadings = 0;
    var sessionInjector = {
        request: function (config) {

            var urlRequest = config.url.split('/');
            if (urlRequest[urlRequest.length - 1] != 'DeshabilitarNuevasNotificaciones') {
                $("#divLoaderfull").show();
                numLoadings++;
            }

            config.url = $("#basePath").attr("href") + config.url;
            return config;
        },
        response: function (response) {
            numLoadings = numLoadings > 0 ? --numLoadings : 0;
            if (numLoadings === 0) {
                numLoadings = 0;
                setTimeout(function () { $("#divLoaderfull").hide(); }, 500);
            }

            return response;

        }
    };
    return sessionInjector;
}]);

app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('injectorDacasys');
}]);

