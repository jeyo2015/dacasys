app.controller("inicioClienteController", function (clinicaService, $scope, $rootScope, consultasService, loginService) {
    init();
    var map;
    var infoWindow;
    function baseUrl() {
        var href = window.location.href.split('/');
        return href[0] + '//' + href[2] + '/';
    }
    function init() {
        loginService.getSessionDto().then(function (result) {
            $rootScope.sessionDto = result;
            if ($rootScope.sessionDto.ChangePass) {
                $scope.showMessage = false;
                $('#modal-renovar').modal('show');
                prepararNuevoPerfil();
            }
        });
        $scope.consultorioSeleccionado = null;
        $scope.verConsultorio = true;
        $scope.dateSelected = moment().format('DD/MM/YYYY');
        $.datepicker.setDefaults($.datepicker.regional["es"]);
        $("#dtpFecha").datepicker({
            dateFormat: 'dd/mm/yy',
            onSelect: function () {
                $scope.dateSelected = $.datepicker.formatDate("dd/mm/yy", $(this).datepicker('getDate'));
                cargarCitasDelDia();
                $scope.$apply();
            }
        });

        $('#dtpFecha').val(moment().format('DD/MM/YYYY'));
        $rootScope.path = baseUrl();
        $("#mapa").ready(function () {

            var h = $(window).height();
            var rest = $("#headerTotal").height();
            $("#mapa").height(h - rest - 105);
            InicializarMapa();

        });

        cargar_clinicas();
    };




    $scope.seleccionaCita = function (cita) {
        if ($rootScope.sessionDto.IDConsultorio == -1 && $rootScope.sessionDto.loginUsuario.length == 0) {
            $rootScope.IDConsultorioDesdeMapa = $scope.consultorioSeleccionado.IDConsultorio;
            $rootScope.isAdmin = false;
            $("#modal-login-cliente").modal('show');
        } else {
            $scope.citaSeleted = cita;
            alertaEstadoCita();
        }
    };

    function alertaEstadoCita() {
        if ($scope.citaSeleted.EsTarde)
            toastr.warning("La fecha y hora seleccionada ya no estan diponibles");
        else
            if ($scope.citaSeleted.EstaAtendida)
                toastr.warning("La cita seleccionada ya fue atendida");
    }
    function cargar_clinicas() {
        clinicaService.getAllClinicasHabilitadas().then(function (result) {
            $scope.clinicas = result;
            crearTodosLosMarkers();
        });
    }

    function abrirModalVerMasClinica() {
        $scope.verConsultorio = true;
        $("#modal-ver-mas").modal('show');
    }

    $scope.mostrarHorarios = function (consultorio) {
        $scope.consultorioSeleccionado = consultorio;

        cargarCitasDelDia();
    }
    function cargarCitasDelDia() {
        //if ($scope.miConsultorioSelected.IDEmpresa && $scope.miConsultorioSelected.TiempoCita) {
        consultasService.getCitasDelDia($scope.dateSelected, $scope.consultorioSeleccionado.IDConsultorio, $scope.consultorioSeleccionado.TiempoCita).then(function (result) {
            $scope.citasDelDia = result;
            $scope.citaSeleted = null;
            $scope.verConsultorio = false;
        });
        //}
    }

    function openInfoWindow(marker) {
        point = marker.getPosition();
        ///var telefonos = telefono.toString().split('#');
        var htlml = '<div id="contentInfoWindow" class="row ">\
                        <div class="col-md-12">\
                    <span>Nombre:'  + $scope.clinicaSeleccionada.Nombre +
                    '</span>\
                     </div>\
     <button id="test" class="btn btn-link"  >Ver mas </button>\
                      </div> ';
        infoWindow.setContent([
           htlml
        ].join(''));

        google.maps.event.addListener(infoWindow, 'domready', function () {
            $('#test').click(function () {
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
    $scope.cerrarModalVerMas = function () {
        $scope.clinicaSeleccionada = null;
        $scope.citaSeleted = null;
        $("#modal-ver-mas").modal('hide');
    }
    $scope.validarCamposCita = function () {
        return $scope.citaSeleted == null || $scope.citaSeleted.EstaOcupada;
    };
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