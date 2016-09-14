app.controller("inicioClienteController", function (clinicaService, comentarioService, $scope, $compile, $rootScope, consultasService, loginService, notificacionesConsultorioService) {
    init();
    var map;
    var infoWindow;
    function baseUrl() {
        var href = window.location.href.split('/');
        return href[0] + '//' + href[2] + '/';
    }
    function init() {


        if (!$rootScope.sessionDto) {
            loginService.getSessionDto().then(function (result) {
                $rootScope.sessionDto = result;

            });
        }
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
    //$scope.agendarCita = function () {
    //    consultasService.insertarCitaPaciente($scope.citaSeleted, $scope.dateSelected, $rootScope.sessionDto.loginUsuario).then(function (result) {
    //        if (result.Success) {
    //            toastr.success(result.Message);
    //            cargarCitasDelDia();
    //        } else {
    //            toastr.error(result.Message);
    //        }
    //    });
    //};
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
                    $('#modalconfirmarCita').modal('show');
                } else {
                    $scope.citaSeleted = null;
                    $("#modal-horarios-consultorio").modal('hide');
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
                $('#modalconfirmarCita').modal('hide');
                $scope.citaSeleted = null;
            } else {
                toastr.error(result.Message);
                $('#modalconfirmarCita').modal('hide');
            }
        });
    };

    function cargar_clinicas() {
        clinicaService.getAllClinicasHabilitadas().then(function (result) {

            $scope.clinicas = result;
            crearTodosLosMarkers();
        });
    }

    function mostrarConsultorios() {
        $("#modal-seleccionar-consultorio").modal('show');
    }
    $scope.abrirModalVerMasClinica = function () {
        debugger;
        $scope.consultorioSeleccionado = null;
        $scope.verConsultorio = true;
        if ($scope.clinicaSeleccionada.Consultorios.length == 1) {
            $("#modal-ver-mas").modal('show');
            $scope.mostrarHorarios();
        } else
            mostrarConsultorios();


    };


    $scope.seleccionarConsultorioClinica = function (consultorio) {
        $scope.consultorioSeleccionado = consultorio;

    };
    $scope.abrirModalVerMasConsultorio = function () {
        $scope.mostrarPanel = 4;
        $("#modal-seleccionar-consultorio").modal('hide');
        cargarCitasDelDia("#modal-ver-mas");
    };

    function cargarCitasDelDia(openModal) {
        consultasService.getCitasDelDia($scope.dateSelected, $scope.consultorioSeleccionado.IDConsultorio, $scope.consultorioSeleccionado.TiempoCita).then(function (result) {
            $scope.citasDelDia = result;
            $scope.citaSeleted = null;
            $scope.verConsultorio = false;
            if (openModal && openModal.length > 0)
                $(openModal).modal('show');
        });

    }

    $scope.cerraModalConfirmCita = function () {
        $scope.citaSeleted = null;
        $('#modalconfirmarCita').modal('hide');
    };
    function openInfoWindow(marker) {

        point = marker.getPosition();

        $scope.telefonosClinicaSeleccionada = "";
        console.log($scope.clinicaSeleccionada);
        for (var i = 0; i < $scope.clinicaSeleccionada.Telefonos.length; i++) {
            if (i > 0)
                $scope.telefonosClinicaSeleccionada = $scope.telefonosClinicaSeleccionada + " - ";
            $scope.telefonosClinicaSeleccionada = $scope.telefonosClinicaSeleccionada + $scope.clinicaSeleccionada.Telefonos[i].Telefono;
        }


        var htmlElement = '<div id="contentInfoWindow" class="row " style="height: 130px; width: 250px">\
                                <div class="row">\
                                     <div class="col-md-4 col-xs-3">\
                                         <img style="height: 48px; width: 65px; border:1px solid #3B3B3C" data-ng-src="{{clinicaSeleccionada.LogoParaMostrar}}" alt="Logo de la clinica">\
                                     </div>\
                                     <div class="col-md-8 col-xs-8">\
                                         <div class="row">\
                                              <h4 title="{{clinicaSeleccionada.Nombre}}" class="cortar" style="font-weight:bold; margin-bottom:0px"> Clinica {{clinicaSeleccionada.Nombre}} </h4>\
                                              <label title="{{clinicaSeleccionada.Descripcion}}" class="cortar">{{clinicaSeleccionada.Descripcion}}</label>\
                                         </div>\
                                     </div>\
                                </div>\
                                <div class="row" >\
                                     <div class="col-md-12 col-xs-12">\
                                         <label title="{{telefonosClinicaSeleccionada}}" class="cortar">{{telefonosClinicaSeleccionada}}</label>\
                                     </div>\
                                </div>\
                                <div class="row" >\
                                     <div class="col-md-4 col-xs-12">\
                                         <a id="test"  ng-click="abrirModalVerMasClinica()" >Ver mas </a>\
                                     </div>\
                                 </div>\
                           </div> ';

        var compiled = $compile(htmlElement)($scope);
        infoWindow.setContent(compiled[0]);
        $scope.$apply();
        infoWindow.open(map, marker);


    }

    //$scope.validarCamposComentario = function () {
    //    return $scope.comentarioParaGuardar == null || $scope.comentarioParaGuardar.Comentario.length < 1;
    //};

    $scope.mostrarModalComentario = function (consultorio) {
        $scope.consultorioSeleccionado = angular.copy(consultorio);
        if ($rootScope.sessionDto.IDConsultorio == -1 && $rootScope.sessionDto.loginUsuario.length == 0) {
            $rootScope.IDConsultorioDesdeMapa = $scope.consultorioSeleccionado.IDConsultorio;
            $scope.loginEmpresa = "";

            $rootScope.modalComentario = true;
            $("#modal-login-cliente").modal('show');
        } else {
            prepararNuevoComentario();
            $("#modal-comentario").modal('show');
        }
    };
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
            marker.setIcon('Content/img/markerselect.png');
        });
    }

    $scope.cerrarModalVerMas = function () {
        $scope.clinicaSeleccionada = null;
        $scope.citaSeleted = null;
        $("#modal-ver-mas").modal('hide');
    };
    $scope.cerrarSolicitud = function () {
        $("#enviar-notificacion").modal('hide');
    };
    $scope.enviarNotificacion = function () {
        var notificacion = {
            IDNotificacion: -1,
            IDConsultorio: $scope.consultorioSeleccionado.IDConsultorio,
            NombreUsuario: "",
            TipoNotificacion: 1,
            LoginUsuario: $rootScope.sessionDto.loginUsuario,
            FechaNotificacion: null,
            EstadoNotificacion: 1
        };

        notificacionesConsultorioService.enviarSolicitudConsultorio(notificacion).then(function (result) {
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
        // marker.setIcon('desarrollo/Content/img/marker.png');
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
            // Cambiar_Imagenes();
        });

    }

    $scope.mostrarModalHorarios = function (consultorio) {
        $scope.consultorioSeleccionado = angular.copy(consultorio);
        cargarCitasDelDia();
    };
    function prepararNuevoComentario() {
        $rootScope.comentarioParaGuardar = {
            LoginCliente: $rootScope.sessionDto.loginUsuario,
            Comentario: '',
            State: 1,
            IsVisible: true,
            IDEmpresa: $scope.consultorioSeleccionado.IDConsultorio
        };
    }

    $scope.guardarComentario = function () {
        if ($rootScope.sessionDto.IDConsultorio == -1 && $rootScope.sessionDto.loginUsuario.length == 0) {
            $rootScope.IDConsultorioDesdeMapa = $scope.consultorioSeleccionado.IDConsultorio;
            $scope.loginEmpresa = "";
            $rootScope.isAdmin = false;
            $("#modal-login-cliente").modal('show');
        } else
            if ($scope.comentarioParaGuardar.State == 1) {
                comentarioService.insertarComentario($scope.comentarioParaGuardar).then(function (result) {
                    if (result.Data == 1) {

                        toastr.success(result.Message);
                        obtenerListaComentarios();
                    } else {
                        toastr.error(result.Message);
                    }
                });
            }
    };
    $scope.cerrarModalHorarios = function () {
        $scope.citaSeleted = null;
        $("#modal-horarios-consultorio").modal('hide');
    };
    $scope.mostrarContactenos = function () {
        $scope.mostrarPanel = 2;
    };
    //$scope.mostrarConsultorios = function () {
    //    $scope.mostrarPanel = 1;
    //};
    function obtenerListaComentarios() {
        comentarioService.obtenerComentariosPorEmpresa($scope.consultorioSeleccionado.IDConsultorio).then(function (result) {
            $scope.ListaComentario = result;
            $scope.ListaComentario = result.select(function (comentario) {
                comentario.FechaCreacion = moment(comentario.FechaCreacion).format('DD/MM/YYYY');
                return comentario;
            });
            prepararNuevoComentario();
        });
    }
    $scope.mostrarComentarios = function () {
        $scope.mostrarPanel = 3;
        obtenerListaComentarios();
    };
    $scope.mostrarHorarios = function () {
        $scope.mostrarPanel = 4;
        $scope.mostrarModalHorarios($scope.clinicaSeleccionada.Consultorios[0]);
    };
});