app.controller("miConsultorioController", function (clinicaService, comentarioService, consultasService, $scope, $compile, $rootScope) {
    init();
    var map;
    function init() {
        $scope.listaMarcadores = [];
        cargarConsultorioPorCliente();
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
        InicializarMapa();
        $('#dtpFecha').val(moment().format('DD/MM/YYYY'));
    };

    function cargarConsultorioPorCliente() {
        clinicaService.obtenerConsultoriosPorCliente($rootScope.sessionDto.loginUsuario).then(function (result) {
            $scope.ListaConsultorio = result.where(function (r) {
                r.Longitud = r.Longitud.replace(".", ",");
                r.Latitud = r.Latitud.replace(".", ",");
                return r;
            });
        });
    }

    function InicializarMapa() {
        var latlng = new google.maps.LatLng(-17.783198, -63.182046);
        var myOptions = {
            zoom: 15,
            center: latlng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById("mapaConsultorio"), myOptions);
       
    }
    google.maps.event.addDomListener(window, "resize", resizingMap());

    $('#modal-mapa-ubicacion').on('show.bs.modal', function () {
        //Must wait until the render of the modal appear, thats why we use the resizeMap and NOT resizingMap!! ;-)
        resizeMap();
    });

    function resizeMap() {
        if (typeof map == "undefined") return;
        setTimeout(function () { resizingMap(); }, 400);
    }

    function resizingMap() {
        if (typeof map == "undefined") return;
        var center = map.getCenter();
        google.maps.event.trigger(map, "resize");
        map.setCenter(center);
    }
    function CrearMarcador(id, latLong) {
        var marker = new google.maps.Marker({
            map: map,
            position: latLong,
            title: '',
            icon: 'desarrollo/Content/img/marker.png',
            zIndex: id
        });
        map.setZoom(17);
        $scope.listaMarcadores.push(marker);
    }

    function removerMarcador() {
        if ($scope.listaMarcadores) {
            for (var i = 0 ; i < $scope.listaMarcadores.length; i++) {
                var markerCurrent = $scope.listaMarcadores[i];
                markerCurrent.setMap(null);
            }
        }
    }

    $scope.abrirModalMapa = function (consultorio) {
        $('#modal-mapa-ubicacion').modal('show');
        $scope.latlngActual = new google.maps.LatLng(parseFloat(consultorio.Latitud.replace(',', '.')), parseFloat(consultorio.Longitud.replace(',', '.')));
        removerMarcador();
        CrearMarcador(0, $scope.latlngActual);
        map.setCenter($scope.latlngActual);
    };

    $scope.cerrarModalMapa = function () {
        $('#modal-mapa-ubicacion').modal('hide');
    };

    $scope.seleccionarConsultorio = function (consultorio) {
        $scope.miConsultorioSelected = consultorio;
    };

    $scope.abrirModalCita = function () {
        cargarCitasDelDia();
    };

    $scope.cerrarModalCita = function () {
        $('#modal-mi-cita').modal('hide');
    };

    function cargarCitasDelDia() {
        if ($scope.miConsultorioSelected.IDEmpresa && $scope.miConsultorioSelected.TiempoCita) {
            consultasService.getCitasDelDia($scope.dateSelected, $scope.miConsultorioSelected.IDEmpresa, $scope.miConsultorioSelected.TiempoCita).then(function (result) {
                $scope.citasDelDia = result;
                $scope.citaSeleted = null;
                $('#modal-mi-cita').modal('show');
            });
        }
    }

    $scope.seleccionaCita = function (cita) {
        $scope.citaSeleted = cita;
        alertaEstadoCita();
    };

    function alertaEstadoCita() {
        if ($scope.citaSeleted.EsTarde)
            toastr.warning("La fecha y hora seleccionada ya no estan diponibles");
        else
            if ($scope.citaSeleted.EstaAtendida)
                toastr.warning("La cita seleccionada ya fue atendida");
    }

    $scope.validarCamposCita = function () {
        return $scope.citaSeleted == null || $scope.citaSeleted.EstaOcupada;
    };

    $scope.agendarCita = function () {
        consultasService.insertarCitaPaciente($scope.citaSeleted, $scope.dateSelected, $rootScope.sessionDto.loginUsuario).then(function (result) {
            if (result.Success) {
                toastr.success(result.Message);
                $('#modal-mi-cita').modal('hide');
                $scope.citaSeleted = null;
            } else {
                toastr.error(result.Message);
            }
        });
    };

    $scope.abrirModalComentario = function () {
        prepararNuevoComentario();
        $('#modal-mi-comentario').modal('show');
    };

    $scope.cerrarModalComentario = function () {
        $('#modal-mi-comentario').modal('hide');
    };

    function prepararNuevoComentario() {
        $scope.comentarioParaGuardar = {
            LoginCliente: $rootScope.sessionDto.loginUsuario,
            Comentario: '',
            State: 1,
            IsVisible: true,
            IDEmpresa: $scope.miConsultorioSelected.IDEmpresa
        };
    }

    $scope.validarCamposComentario = function () {
        return $scope.comentarioParaGuardar == null || $scope.comentarioParaGuardar.Comentario.length < 1;
    };

    $scope.guardarComentario = function () {
        if ($scope.comentarioParaGuardar.State == 1) {
            comentarioService.insertarComentario($scope.comentarioParaGuardar).then(function (result) {
                if (result.Data == 1) {
                    toastr.success(result.Message);
                    prepararNuevoComentario();
                    $scope.cerrarModalComentario();
                } else {
                    toastr.error(result.Message);
                }
            });
        }
    };
});