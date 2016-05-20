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




app.factory('injectorDacasys', ['$rootScope','$q', function ($rootScope, $q) {
    var numLoadings = 0;
    // $rootScope.countRequest = 0;
    var sessionInjector = {
        request: function (config) {
            $("#divLoaderfull").show();
            numLoadings++;
            // $rootScope.countRequest++;
            config.url = $("#basePath").attr("href") + config.url;
            return config;
        },
        response: function(response) {
            if ((--numLoadings) === 0)
                $("#divLoaderfull").hide();
            return response;
            // $rootScope.countRequest--;
            {
                // Hide loader

                //  $rootScope.$broadcast("loader_hide");
                // $rootScope.lastResponse = 0;
                //$(document).ready(function() {
                //    window.setTimeout(function() {
                //        if ($rootScope.countRequest === 0) {
                //            //console.log('la ultima');
                //            var div = $('<div id="continue">');
                //            //div.css('display', 'inline-block');
                //            div.css('background', 'cadetblue');
                //            div.css('width', '1px');
                //            div.css('height', '1px');
                //            $('#logormtools').append(div);
                //        }
                //    }, 2000);
                //});
                //   }
            }
        }
    };
    return sessionInjector;
}]);
app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('injectorDacasys');
}]);

