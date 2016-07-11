app.controller("inicioClienteController", function (clinicaService, $scope, $rootScope, consultasService, loginService, notificacionesConsultorioService) {
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
        if (cita.EsTarde) {
            toastr.warning("La fecha y hora seleccionada ya no estan diponibles");
            return;
        }
        else
            if (cita.EstaOcupada) {
                toastr.warning("La fecha y hora seleccionado se encuentra ocupado");
                return;
            }

        if ($rootScope.sessionDto.IDConsultorio == -1 && $rootScope.sessionDto.loginUsuario.length == 0) {
            $rootScope.IDConsultorioDesdeMapa = $scope.consultorioSeleccionado.IDConsultorio;
            $scope.loginEmpresa = "";

            $rootScope.isAdmin = false;
            $("#modal-login-cliente").modal('show');
        } else {
            consultasService.verificarClienteEnConsultorio($scope.consultorioSeleccionado.IDConsultorio, $rootScope.sessionDto.loginUsuario).then(function (result) {

                if (result == "true") {
                    $scope.citaSeleted = cita;

                } else {
                    $scope.citaSeleted = null;
                    $("#enviar-notificacion").modal('show');
                }

            });




        }
    };

    $scope.agendarCita = function () {
        consultasService.insertarCitaPaciente($scope.citaSeleted, $scope.dateSelected, $rootScope.sessionDto.loginUsuario).then(function (result) {
            if (result.Success) {
                toastr.success(result.Message);
                cargarCitasDelDia();

                $scope.citaSeleted = null;
            } else {
                toastr.error(result.Message);
            }
        });
    };

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

    $scope.cerrarModalVerMas = function() {
        $scope.clinicaSeleccionada = null;
        $scope.citaSeleted = null;
        $("#modal-ver-mas").modal('hide');
    };
    $scope.cerrarSolicitud = function() {
        $("#enviar-notificacion").modal('hide');
    };
    $scope.enviarNotificacion = function() {
        var notificacion = {
            IDNotificacion: -1,
            IDConsultorio: $scope.consultorioSeleccionado.IDConsultorio,
            NombreUsuario: "",
            TipoNotificacion: 1,
            LoginUsuario: $rootScope.sessionDto.loginUsuario,
            FechaNotificacion: null,
            EstadoNotificacion: 1
        };

        notificacionesConsultorioService.enviarSolicitudConsultorio(notificacion).then(function(result) {
            if (result.Success) {
                toastr.success(result.Message);
                $scope.citaSeleted = null;
              
            } else {
                toastr.error(result.Message);
            }
            $("#enviar-notificacion").modal('hide');
        });
    };

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