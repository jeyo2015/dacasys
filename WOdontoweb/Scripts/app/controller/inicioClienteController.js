﻿app.controller("inicioClienteController", function (clinicaService, comentarioService, $scope, $compile, $rootScope, consultasService, loginService, notificacionesConsultorioService) {
    init();
    var map;
    var infoWindow;
    var baseURL = "";
    function baseUrl() {
        var href = window.location.href.split('/');
        return href[0] + '//' + href[2] + '/';
    }

    $("#pac-input").focus(function () {
        $scope.mostrarConsultorios = true;
        $scope.$apply();
    });

    function init() {
       $scope.baseURL = $("#basePath").attr("href");
     
        $scope.mostrarConsultorios = false;
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
            $scope.isAdmin = false;
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
            clinicaService.obtenerConsultorios().then(function (result) {
                $scope.consultoriosBuscador = result;
            });
        });

    }

    function mostrarConsultorios() {
        $("#modal-seleccionar-consultorio").modal('show');
    }
    $scope.abrirModalVerMasClinica = function () {
        $scope.consultorioSeleccionado = null;
        $scope.verConsultorio = true;
        if ($scope.clinicaSeleccionada.Consultorios.length == 1) {
            $scope.consultorioSeleccionado = $scope.clinicaSeleccionada.Consultorios[0];
           // $compile("#modal-ver-mas")($scope);
            $("#modal-ver-mas").modal('show');
            $scope.mostrarInformacion();
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
        markers.select(function (item) {
            if (item.zIndex != marker.zIndex)
                item.setIcon($scope.baseURL + 'Content/img/marker.png');
        });
        $scope.telefonosClinicaSeleccionada = "";
        
        for (var i = 0; i < $scope.clinicaSeleccionada.Telefonos.length; i++) {
            if (i > 0)
                $scope.telefonosClinicaSeleccionada = $scope.telefonosClinicaSeleccionada + " - ";
            $scope.telefonosClinicaSeleccionada = $scope.telefonosClinicaSeleccionada + $scope.clinicaSeleccionada.Telefonos[i].Telefono;
        }
        map.setCenter(point);

        var htmlElement = '<div id="contentInfoWindow" class="row " style="height: 130px; width: 250px">\
                                <div class="row">\
                                     <div class="col-md-4 col-xs-3">\
                                         <img style="height: 48px; width: 65px; border:1px solid #3B3B3C" data-ng-src="{{clinicaSeleccionada.LogoParaMostrar}}" alt="Logo de la clinica">\
                                     </div>\
                                     <div class="col-md-8 col-xs-8">\
                                         <div class="row">\
                                              <h4 title="{{clinicaSeleccionada.Nombre}}" class="cortar" style="font-weight:bold; margin-bottom:0px"> {{clinicaSeleccionada.Nombre}} </h4>\
                                              <label title="{{clinicaSeleccionada.Descripcion}}" class="cortar">{{clinicaSeleccionada.Descripcion}}</label>\
                                         </div>\
                                     </div>\
                                </div>\
                                <div class="row" >\
                                <div class="col-md-4 col-xs-4">\
                                         <label  style="color: #3B3B3C;">Direccion: </label>\
                                     </div>\
                                     <div class="col-md-8 col-xs-8">\
                                         <label  class="cortar">{{clinicaSeleccionada.Direccion}}</label>\
                                     </div>\
                                </div>\
                                <div class="row" >\
                                <div class="col-md-4 col-xs-4">\
                                         <label  style="color: #3B3B3C;">Telefonos:</label>\
                                     </div>\
                                     <div class="col-md-8 col-xs-8">\
                                         <label  title="{{telefonosClinicaSeleccionada}}" class="cortar">{{telefonosClinicaSeleccionada}}</label>\
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
      //  $scope.$apply();
        infoWindow.open(map, marker);


    }



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
    $scope.seleccionarConsultorioBuscador = function (consultorio) {
        var markerSelect = markers.where(function (item) {
            return consultorio.IDClinica == item.zIndex;
        })[0];
        $scope.clinicaSeleccionada = $scope.clinicas.where(function (item) {
            return item.IDClinica == consultorio.IDClinica;
        })[0];
        if ($scope.clinicaSeleccionada.LogoParaMostrar== null)
        clinicaService.obtenerLogoClinica($scope.clinicaSeleccionada.IDClinica).then(function (result) {
            $scope.clinicaSeleccionada.LogoParaMostrar = result.LogoParaMostrar;
            $scope.consultorioBuscar = "";
            $scope.mostrarConsultorios = false;
            openInfoWindow(markerSelect);
            markerSelect.setIcon($scope.baseURL + 'Content/img/markerselect.png');
        });

        else {
            $scope.consultorioBuscar = "";
            $scope.mostrarConsultorios = false;
            openInfoWindow(markerSelect);
            markerSelect.setIcon($scope.baseURL + 'Content/img/markerselect.png');
        }

    }
    var markers = [];
    function CrearMarcador(clinica) {
       
        var marker = new google.maps.Marker({
            map: map,
            position: new google.maps.LatLng(clinica.Latitud, clinica.Longitud),
            title: 'Click -- Ver Detalle -- ',
            icon: $scope.baseURL + 'Content/img/marker.png',
            zIndex: clinica.IDClinica
        });
        markers.push(marker);
        google.maps.event.addListener(marker, 'click', function () {

            $scope.clinicaSeleccionada = $scope.clinicas.where(function (item) {
                return item.IDClinica == marker.zIndex;
            })[0];
          
            if ($scope.clinicaSeleccionada.LogoParaMostrar == null)
                clinicaService.obtenerLogoClinica($scope.clinicaSeleccionada.IDClinica).then(function(result) {
                    $scope.clinicaSeleccionada.LogoParaMostrar = result.LogoParaMostrar;
                    openInfoWindow(marker);
                    marker.setIcon($scope.baseURL + 'Content/img/markerselect.png');
                    $scope.mostrarConsultorios = false;
                });

            else {
                openInfoWindow(marker);
                marker.setIcon($scope.baseURL + 'Content/img/markerselect.png');
                $scope.mostrarConsultorios = false;
            }
            $scope.$apply();
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
        markers.select(function (item) {

            item.setIcon($scope.baseURL + 'Content/img/marker.png');
        });
        // marker.setIcon('odontoweb/Content/img/marker.png');
    }
    function InicializarMapa() {
        var latlng = new google.maps.LatLng(-17.783198, -63.182046);
        var myOptions = {
            zoom: 15,
            center: latlng,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            mapTypeControl: true,
            mapTypeControlOptions: {
                style: google.maps.MapTypeControlStyle.HORIZONTAL_BAR,
                position: google.maps.ControlPosition.BOTTOM_LEFT
            },
        };
        map = new google.maps.Map(document.getElementById("mapa"), myOptions);
        var input = document.getElementById('buscadorClinica');
        map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);
        infoWindow = new google.maps.InfoWindow();
        google.maps.event.addListener(map, 'click', function () {
            closeInfoWindow();
            $scope.consultorioBuscar = "";
            $scope.mostrarConsultorios = false;
            $scope.$apply();
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

            consultasService.verificarClienteEnConsultorio($scope.consultorioSeleccionado.IDConsultorio, $rootScope.sessionDto.loginUsuario).then(function (result) {

                if (result == "true") {
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
                } else {

                    $("#enviar-notificacion").modal('show');
                    prepararNuevoComentario();
                }

            });

    };
    $scope.cerrarModalHorarios = function () {
        $scope.citaSeleted = null;
        $("#modal-horarios-consultorio").modal('hide');
    };
    $scope.mostrarContactenos = function () {
        $scope.mostrarPanel = 2;
        $scope.emailDe = "";
        $scope.mensajeContactenos = "";
        $scope.asunto = "";
    };
    $scope.enviarContactenos = function () {
        clinicaService.enviarContactenos($scope.mensajeContactenos, $scope.emailDe, $scope.asunto).then(function (result) {
            if (result.Success) {
                toastr.success(result.Message);
                $scope.emailDe = "";
                $scope.mensajeContactenos = "";
                $scope.asunto = "";
            } else {
                toastr.error(result.Message);
            }
        });
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

    $scope.mostrarInformacion = function () {
        $scope.mostrarPanel = 1;
      
    };
});