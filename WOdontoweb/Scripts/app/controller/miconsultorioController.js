app.controller("miConsultorioController", function (clinicaService, comentarioService, $scope, $compile, $rootScope) {
    init();

    function init() {
        $scope.listaMarcadores = [];
        cargarConsultorioPorCliente();
    };

    function cargarConsultorioPorCliente() {
        clinicaService.obtenerConsultoriosPorCliente($rootScope.sessionDto.loginUsuario).then(function (result) {
            $scope.ListaConsultorio = result;
        });
    }

    $scope.abrirModalComentario = function () {
        prepararNuevoComentario();
        $('#modal-mi-comentario').modal('show');
    };

    $scope.cerrarModalComentario = function () {        
        $('#modal-mi-comentario').modal('hide');
    };

    $scope.abrirModalCita = function () {
        $('#modal-mi-cita').modal('show');
    };

    $scope.cerrarModalCita = function () {
        $('#modal-mi-cita').modal('hide');
    };

    function InicializarMapa() {
        var latlng = new google.maps.LatLng(-17.783198, -63.182046);
        var myOptions = {
            zoom: 15,
            center: latlng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById("mapaConsultorio"), myOptions);
        CrearMarcador(0, latlng);
        google.maps.event.addListener(map, 'click', function (event) {
            removerMarcador();
            CrearMarcador(0, event.latLng)
            $scope.latlngActual = event.latLng;
        });
    }

    function CrearMarcador(id, latLong) {
        var marker = new google.maps.Marker({
            map: map,
            position: latLong,
            title: '',
            icon: 'Content/img/marker.png',
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

    $scope.abrirModalMapa = function () {
        $('#modal-mapa-ubicacion').modal('show');
        InicializarMapa();
        //removerMarcador();
        //CrearMarcador(0, $scope.latlngActual);
        //map.setCenter($scope.latlngActual);
    };

    $scope.cerrarModalMapa = function () {
        $('#modal-mapa-ubicacion').modal('hide');
    };

    $scope.seleccionarConsultorio = function (consultorio) {
        $scope.miConsultorioSelected = consultorio;
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