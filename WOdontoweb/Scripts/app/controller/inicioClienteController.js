app.controller("inicioClienteController", function (clinicaService, $scope, $rootScope) {
    init();
    var map;
    function baseUrl() {
        var href = window.location.href.split('/');
        return href[0] + '//' + href[2] + '/';
    }
    function init() {
        $rootScope.path = baseUrl(); 
        $("#mapa").ready(function () {
          
            var h = $(window).height();
            var rest = $("#headerTotal").height();
            $("#mapa").height(h - rest -105);
            InicializarMapa();

        });
        
        cargar_clinicas();
    };
    function cargar_clinicas() {
        clinicaService.getAllClinicasHabilitadas().then(function (result) {
            $scope.clinicas = result;
            crearTodosLosMarkers();
        });
    }

    function crearTodosLosMarkers() {
        angular.forEach($scope.clinicas, function (clinica, index) {
            CrearMarcador(clinica.IDClinica, clinica.Latitud, clinica.Longitud);
        });
    }
    function CrearMarcador(id, latitud, longitud) {

        var marker = new google.maps.Marker({
            map: map,
            position: new google.maps.LatLng(latitud, longitud),
            title: 'Click -- Ver Detalle -- ',
            icon: 'Content/img/marker.png',
            zIndex: id
        });
    }
    function InicializarMapa() {
        var latlng = new google.maps.LatLng(-17.783198, -63.182046);
        var myOptions = {
            zoom: 15,
            center: latlng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById("mapa"), myOptions);
        // infoWindow = new google.maps.InfoWindow();
        //google.maps.event.addListener(map, 'click', function () {
        //    closeInfoWindow();
        //    Cambiar_Imagenes();
        //});

    }
});