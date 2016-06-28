app.controller("inicioClienteController", function (clinicaService, $scope, $rootScope) {
    init();
    var map;
    var infoWindow;
    function baseUrl() {
        var href = window.location.href.split('/');
        return href[0] + '//' + href[2] + '/';
    }
    function init() {
        $rootScope.path = baseUrl();
        $("#mapa").ready(function () {

            var h = $(window).height();
            var rest = $("#headerTotal").height();
            $("#mapa").height(h - rest - 105);
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

    function abrirModalVerMasClinica() {
        console.log($scope.clinicaSeleccionada);
        $("#modal-ver-mas").modal('show');
    }

 

    function openInfoWindow(marker) {
        point = marker.getPosition();
        ///var telefonos = telefono.toString().split('#');
        var htlml = '<div id="contentInfoWindow" class="row ">\
                        <div class="col-md-12">\
                    <span>Nombre:'  + $scope.clinicaSeleccionada.Nombre+
                    '</span>\
                     </div>\
<button id="test" class="btn btn-link"  >Ver mas </button>\
                      </div> ';
        infoWindow.setContent([
           htlml
        ].join(''));
        
        google.maps.event.addListener(infoWindow, 'domready', function () {
            $('#test').click(function() {
                abrirModalVerMasClinica();
            });
        });
        infoWindow.open(map, marker);


    }
    function crearTodosLosMarkers() {
        angular.forEach($scope.clinicas, function (clinica, index) {
            CrearMarcador(clinica);
        });
    }
    function CrearMarcador(clinica) {

        var marker = new google.maps.Marker({
            map: map,
            position: new google.maps.LatLng(clinica.Latitud, clinica.Longitud),
            title: 'Click -- Ver Detalle -- ',
            icon: 'Content/img/marker.png',
            zIndex: clinica.IDClinica
        });
        google.maps.event.addListener(marker, 'click', function () {
           
            $scope.clinicaSeleccionada = $scope.clinicas.where(function (item) {
                return item.IDClinica == marker.zIndex;
            })[0];
            $scope.$apply();
            openInfoWindow(marker);
        });
    }

    function closeInfoWindow() {
        infoWindow.close();
    }
    function InicializarMapa() {
        var latlng = new google.maps.LatLng(-17.783198, -63.182046);
        var myOptions = {
            zoom: 15,
            center: latlng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById("mapa"), myOptions);
        infoWindow = new google.maps.InfoWindow();
        google.maps.event.addListener(map, 'click', function () {
            closeInfoWindow();
            Cambiar_Imagenes();
        });

    }
});