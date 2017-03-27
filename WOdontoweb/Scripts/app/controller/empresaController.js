app.controller("empresaController", function (loginService, clinicaService, $scope, $rootScope, $compile) {
    function prepararDtoConsultorio() {
        $scope.consultorioParaGuardar = {
            IDConsultorio: $rootScope.sessionDto.IDConsultorio,
            NIT: "",
            Email: "",
            IDIntervalo: -1
        };
    }

    $scope.validarCampos = function () {
        return $scope.consultorioParaGuardar == null || $scope.intervaloSelected == null
            || $scope.consultorioParaGuardar.NIT.length <= 0 || $scope.consultorioParaGuardar.Email.length <= 0;
    };

    $scope.modificarConsultorio = function () {
        $scope.consultorioParaGuardar.IDIntervalo = $scope.intervaloSelected.ID;
        clinicaService.modificarConsultorio($scope.consultorioParaGuardar).then(function (result) {
            if (result.Data == 1) {
                toastr.success(result.Message);
            } else {
                toastr.error(result.Message);
            }
        });
    };
    $('#fileupload').fileupload({
        dataType: 'json',
        url: '/Empresa/UploadFiles',
        acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
        maxFileSize: 10000000, // 10 MB
        minFileSize: undefined,
        maxNumberOfFiles: 1,
        done: function (event, data) {
            if (data.result.isUploaded) {
                $scope.clinicaParaModificar.NombreArchivo = data.result.name;
                $("#files").html(data.result.name);
            }
        },
        fail: function (event, data) {
            if (data.files[0].error) {
                alert(data.files[0].error);
            }
        },
        progressall: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $('#progress .progress-bar').css(
                'width',
                progress + '%'
            );
        }
    });

    var map;

    init();
    function init() {
        $scope.listaMarcadores = [];

        if (!$rootScope.sessionDto) {
            loginService.getSessionDto().then(function (result) {
                $rootScope.sessionDto = result;
                inicializarDatos();
            });
        } else {
            inicializarDatos();
        }



    }

    function inicializarDatos() {

        clinicaService.getIntervalosTiempo().then(function (resultIntervalo) {

            $scope.intervalos = resultIntervalo;
            clinicaService.obtenerConsultorioConClinica($rootScope.sessionDto.IDConsultorio).then(function (result) {
                $scope.clinicaParaModificar = result;
                $scope.clinicaParaModificar.Longitud = $scope.clinicaParaModificar.Longitud.replace(".", ",");
                $scope.clinicaParaModificar.FechaInicioLicenciaString = moment(result.FechaInicioLicencia).format('DD/MM/YYYY');
                $scope.clinicaParaModificar.Latitud = $scope.clinicaParaModificar.Latitud.replace(".", ",");
                $scope.latlngActual = new google.maps.LatLng(parseFloat($scope.clinicaParaModificar.Latitud.replace(',', '.')), parseFloat($scope.clinicaParaModificar.Longitud.replace(',', '.')));
                InicializarMapa();
                seleccionarIntervalo();

            });
        });
    }

    function CrearMarcador(id, latLong) {
        // $scope.latlngActual = new google.maps.LatLng(latitud, longitud);
        var marker = new google.maps.Marker({
            map: map,
            position: latLong,
            title: '',
            icon: 'desarrollo/Content/img/marker.png',
            zIndex: id
        });
        // map.setCenter(latlng);
        map.setZoom(17);
        $scope.listaMarcadores.push(marker);
    }

    $scope.abrirModalDeMapa = function () {
        if (!$scope.clinicaParaModificar.EsConsultorioDefault) return;
        $("#modal-mapa-ubicacion").modal('show');
        removerMarcador();
        CrearMarcador(0, $scope.latlngActual);
        map.setCenter($scope.latlngActual);
    };
    function removerMarcador() {
        if ($scope.listaMarcadores) {
            for (var i = 0 ; i < $scope.listaMarcadores.length; i++) {
                var markerCurrent = $scope.listaMarcadores[i];
                markerCurrent.setMap(null);
            }
        }
    }
    function InicializarMapa() {
        var latlng = new google.maps.LatLng(-17.783198, -63.182046);
        var myOptions = {
            zoom: 15,
            center: latlng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById("mapaConsultorio"), myOptions);

        google.maps.event.addListener(map, 'click', function (event) {
            removerMarcador();
            CrearMarcador(0, event.latLng)
            $scope.latlngActual = event.latLng;
        });

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
    $scope.salirModalUbicacion = function () {

        $scope.latlngActual = new google.maps.LatLng($scope.clinicaParaModificar.Latitud, $scope.clinicaParaModificar.Longitud);
        $('#modal-mapa-ubicacion').modal('hide');
    };
    $scope.insertarUbicacionClinica = function () {

        $scope.clinicaParaModificar.Latitud = angular.copy($scope.latlngActual.lat());
        $scope.clinicaParaModificar.Longitud = angular.copy($scope.latlngActual.lng());
        $('#modal-mapa-ubicacion').modal('hide');
    };
    
   
    $scope.selectTrabajoConsultorio = function (trabajo, event) {
        if (event.currentTarget.checked) {
            if ($scope.consultorioToSave.Trabajos == null)
                $scope.consultorioToSave.Trabajos = [{ ID: trabajo.ID, IDConsultorio: -1, State: 1 }];
            else {
                var existeElTrabajo = false;
                var trabajoEnLista = $scope.consultorioToSave.Trabajos.where(function (element) {
                    if (element.ID == trabajo.ID)
                        if (element.State == 3) {
                            existeElTrabajo = true;
                            element.State = 0;
                        }
                    return element;
                });
                if (existeElTrabajo)
                    $scope.consultorioToSave.Trabajos = angular.copy(trabajoEnLista);
                else
                    $scope.consultorioToSave.Trabajos.push({ ID: trabajo.ID, IDConsultorio: -1, State: 1 });
            }

        } else {
            var trabajoEnLista = $scope.consultorioToSave.Trabajos.where(function (element) {
                if (element.ID == trabajo.ID)
                    if (element.State == 0) {
                        element.State = 3;
                        return element;
                    }
            });
            $scope.consultorioToSave.Trabajos = angular.copy(trabajoEnLista);
        }

    };
    $scope.addTelefono = function () {
        $scope.telefonoToSave.State = 1;
        $scope.consultorioToSave.Telefonos.push($scope.telefonoToSave);
        $("#newTelefonoId").remove();
    };

    $scope.selectTelefonoClinica = function (telefonoClinica, index) {
        $scope.telefonoClinicaSelected = telefonoClinica;
        $scope.indexTelefonoClinicaSelected = index;
        $scope.trabajoClinicaSelected = null;
        $scope.consultorioSeleccionado = null;
    };

    $scope.cancelAddTelefonoClinica = function () {
        $("#newTelefonoClinicaId").remove();
    };

    $scope.nuevaClinica = function () {
        prepararNuevaClinica();
        prepararNuevoTelefonoClinica();
        $("#inpNombreClinicaid").focus();
    };

    function prepararNuevoTelefonoClinica() {
        $scope.telefonoClinicaTemp = { ID: -1, IDConsultorio: -1, IDClinica: -1, Telefono: "", Nombre: "", State: 1 };
    }

    $scope.showNewRowTelefonoClinica = function () {
        $scope.consultorioSeleccionado = null;
        prepararNuevoTelefonoClinica();
        var template = "<tr id = \"newTelefonoClinicaId\"> <td><input type=\"text\" class=\"form-control\" id=\"nombreClinicaID\" ng-model=\"telefonoClinicaTemp.Nombre\"> </td><td><input type=\"text\" class=\"form-control\" id=\"telefonoClinicaID\" ng-model=\" telefonoClinicaTemp.Telefono\"></td><td><span ng-click=\"addNewTelefonoClinica()\"><i class=\"fa fa-check\"></i></span><span  ng-click=\"cancelAddTelefonoClinica()\"><i class=\"fa fa-times\"></i></span></td></tr>";
        var linkFn = $compile(template);
        var content = linkFn($scope);

        $("#tableTelefonosClinicaID").append(content);
        $("#nombreClinicaID").focus();
    };

    $scope.showUpdateTelefonoClinica = function () {
        $scope.consultorioSeleccionado = null;
        $scope.clinicaParaModificar.Telefonos.splice($scope.indexTelefonoClinicaSelected, 1);
        var template = "<tr id = \"updateTelefonoClinicaId\"> <td><input type=\"text\" class=\"form-control\" id=\"nombreClinicaID\" ng-model=\"telefonoClinicaSelected.Nombre\"> </td><td><input type=\"text\" class=\"form-control\" id=\"telefonoClinicaID\" ng-model=\" telefonoClinicaSelected.Telefono\"></td><td><span ng-click=\"updateTelefonoClinica()\"><i class=\"fa fa-check\"></i></span><span  ng-click=\"cancelUpdateTelefonoClinica()\"><i class=\"fa fa-times\"></i></span></td></tr>";
        var linkFn = $compile(template);
        var content = linkFn($scope);

        $("#tableTelefonosClinicaID").append(content);
        $("#nombreClinicaID").focus();
    };

    $scope.addNewTelefonoClinica = function () {
        if ($scope.telefonoClinicaTemp.Nombre.length == 0) {
            $("#nombreClinicaID").focus();
        } else {
            if ($scope.telefonoClinicaTemp.Telefono.length == 0) {
                $("#telefonoClinicaID").focus();
            } else {
                $scope.clinicaParaModificar.Telefonos.push(angular.copy($scope.telefonoClinicaTemp));

                $("#newTelefonoClinicaId").remove();
            }
        }
    };

    $scope.updateTelefonoClinica = function () {
        if ($scope.telefonoClinicaSelected.Nombre.length == 0) {
            $("#nombreClinicaID").focus();
        } else {
            if ($scope.telefonoClinicaSelected.Telefono.length == 0) {
                $("#telefonoClinicaID").focus();
            } else {
                $scope.telefonoClinicaSelected.State = 2;
                $scope.clinicaParaModificar.Telefonos.splice($scope.indexTelefonoClinicaSelected, 0, angular.copy($scope.telefonoClinicaSelected));
                $scope.telefonoClinicaSelected = null;
                $("#updateTelefonoClinicaId").remove();
            }
        }
    };

    $scope.cancelUpdateTelefonoClinica = function () {
        $scope.clinicaParaModificar.Telefonos.splice($scope.indexTelefonoClinicaSelected, 0, angular.copy($scope.telefonoClinicaSelected));
        $scope.telefonoClinicaSelected = null;
        $("#updateTelefonoClinicaId").remove();
    };

    $scope.deleteTelefonoClinica = function () {
        $scope.telefonoClinicaSelected.State = 3;
        $scope.clinicaParaModificar.Telefonos.splice($scope.indexTelefonoClinicaSelected, 1);
        $scope.clinicaParaModificar.Telefonos.splice($scope.indexTelefonoClinicaSelected, 0, angular.copy($scope.telefonoClinicaSelected));
        $scope.telefonoClinicaSelected = null;
        $scope.consultorioSeleccionado = null;
    };

    $scope.selectTrabajoClinica = function (trabajoClinica, index) {
        $scope.trabajoClinicaSelected = trabajoClinica;
        $scope.indexTrabajoClinicaSelected = index;
        $scope.telefonoClinicaSelected = null;
        $scope.consultorioSeleccionado = null;
    };

    $scope.addTrabajoClinica = function () {
        if ($scope.primerTrabajo.length > 0) {
            $scope.addTrabajo();
            $("#newTrabajoClinicaId").remove();
        } else {
            $("#tDescripcionClinicaID").focus();
        }
    };

    $scope.cancelAddTrabajoClinica = function () {
        $("#newTrabajoClinicaId").remove();
    };

    $scope.updateTrabajoClinica = function () {
        if ($scope.trabajoClinicaSelected.Descripcion.length > 0) {
            $scope.trabajoClinicaSelected.State = 2;
            $scope.clinicaParaModificar.Trabajos.splice($scope.indexTrabajoClinicaSelected, 0, angular.copy($scope.trabajoClinicaSelected));
            $scope.trabajoClinicaSelected = null;
            $("#updateTrabajoClinicaId").remove();
        } else {
            $("#tDescripcionClinicaID").focus();
        }
    };

    $scope.cancelUpdateTrabajoClinica = function () {
        $scope.clinicaParaModificar.Trabajos.splice($scope.indexTrabajoClinicaSelected, 0, angular.copy($scope.trabajoClinicaSelected));
        $scope.trabajoClinicaSelected = null;
        $("#updateTrabajoClinicaId").remove();
    };

    $scope.deleteTrabajoClinica = function () {
        $scope.trabajoClinicaSelected.State = 3;
        $scope.consultorioSeleccionado = null;
        //$scope.clinicaParaModificar.Trabajos.splice($scope.indexTrabajoClinicaSelected, 1);
        $scope.clinicaParaModificar.Trabajos.splice($scope.indexTrabajoClinicaSelected, 0, angular.copy($scope.trabajoClinicaSelected));
        $scope.trabajoClinicaSelected = null;
    };

    $scope.addTrabajo = function () {
        if ($scope.primerTrabajo.length > 0) {
            $scope.clinicaParaModificar.Trabajos.push({ IDClinica: -1, ID: -1, IDConsultorio: [], Descripcion: angular.copy($scope.primerTrabajo), State: 1 });
        }
    };

    $scope.showNewRowTrabajoClinica = function () {
        $scope.primerTrabajo = "";
        $scope.consultorioSeleccionado = null;
        $scope.trabajoClinicaSelected = null;
        var template = "<tr id = \"newTrabajoClinicaId\"> <td><input type=\"text\"  class=\"form-control\" id=\"tDescripcionClinicaID\" ng-model=\" primerTrabajo\"> </td><td><span  ng-click=\"addTrabajoClinica()\"><i class=\"fa fa-check\"></i></span><span  ng-click=\"cancelAddTrabajoClinica()\"><i class=\"fa fa-times\"></i></span></td></tr>";
        var linkFn = $compile(template);
        var content = linkFn($scope);

        $("#tablaTrabajosClinicaId").append(content);
        $("#tDescripcionClinicaID").focus();
    };

    $scope.showUpdateRowTrabajoClinica = function () {
        $scope.consultorioSeleccionado = null;
        $scope.clinicaParaModificar.Trabajos.splice($scope.indexTrabajoClinicaSelected, 1);
        var template = "<tr id = \"updateTrabajoClinicaId\"> <td><input type=\"text\"  class=\"form-control\" id=\"tDescripcionClinicaID\" ng-model=\" trabajoClinicaSelected.Descripcion\"> </td><td><span  ng-click=\"updateTrabajoClinica()\"><i class=\"fa fa-check\"></i></span><span  ng-click=\"cancelUpdateTrabajoClinica()\"><i class=\"fa fa-times\"></i></span></td></tr>";
        var linkFn = $compile(template);
        var content = linkFn($scope);

        $("#tablaTrabajosClinicaId").append(content);
        $("#tDescripcionClinicaID").focus();
    };

    $scope.addRowTable = function () {
        var template = "<tr id = \"newTelefonoId\"> <td><input type=\"text\" class=\"form-control\" id=\"nombreID\" ng-model=\" telefonoToSave.Nombre\"> </td><td><input type=\"text\" class=\"form-control\" ng-model=\" telefonoToSave.Telefono\"></td><td><span  ng-click=\"addTelefono()\"><i class=\"fa fa-check\"></i></span><span  ng-click=\"cancelAddTelefonoConsultorio()\"><i class=\"fa fa-times\"></i></span></td></tr>";
        var linkFn = $compile(template);
        var content = linkFn($scope);

        $("#tableTelefonosID").append(content);
        $("#nombreID").focus();
    };

    $scope.addRowTableFromClinica = function () {
        var template = "<tr id = \"newTelefonoId\"> <td><input type=\"text\" class=\"form-control\" id=\"nombreID\" ng-model=\" telefonoToSave.Nombre\"> </td><td><input type=\"text\" class=\"form-control\" ng-model=\" telefonoToSave.Telefono\"></td><td><span  ng-click=\"addTelefono()\"><i class=\"fa fa-check\"></i></span><span  ng-click=\"cancelAddTelefonoConsultorio()\"><i class=\"fa fa-times\"></i></span></td></tr>";
        var linkFn = $compile(template);
        var content = linkFn($scope);

        $("#tableTelefonosID").append(content);
        $("#nombreID").focus();
    };
    function matchearTrabajoConsultorio() {
        var CantTrabajos = $scope.clinicaParaModificar.Trabajos.length;
        for (var i = 0; i < CantTrabajos ; i++) {
            var trabajoEnLista = $scope.consultorioToSave.Trabajos.where(function (ele) {
                return ele.ID == $scope.clinicaParaModificar.Trabajos[i].ID;
            });
            if (trabajoEnLista && trabajoEnLista.length > 0)
                $scope.clinicaParaModificar.Trabajos[i].checked = true;
            else
                $scope.clinicaParaModificar.Trabajos[i].checked = false;
        }
    }
    function seleccionarIntervalo() {
        var selected = $scope.intervalos.where(function (item) {
            return item.ID == $scope.clinicaParaModificar.Consultorios[0].IDIntervalo;
        });
        $scope.intervaloSelected = selected[0];
    }

    $scope.guardarClinica = function () {
        $scope.clinicaParaModificar.Longitud = $scope.clinicaParaModificar.Longitud.replace(".", ",");
        $scope.clinicaParaModificar.Latitud = $scope.clinicaParaModificar.Latitud.replace(".", ",");
        $scope.clinicaParaModificar.Consultorios[0].IDIntervalo = angular.copy($scope.intervaloSelected.ID);
        clinicaService.modificarClinica($scope.clinicaParaModificar).then(function (result) {
            if (result.Success) {
                toastr.success(result.Message);

            } else {
                toastr.error(result.Message);
            }
        });

    };
});