app.controller("consultoriosController", function (clinicaService, $scope, $compile) {
    init();
    // var listaMarcadores = [];
    var map;
    function init() {
        $scope.listaMarcadores = [];
        $scope.allClinicas = [];
        cargar_todas_clinicas();
        $scope.clinicaSelected = null;
        prepararNuevaClinica();
        InicializarMapa();

    };

    function CrearMarcador(id, latLong) {
       // $scope.latlngActual = new google.maps.LatLng(latitud, longitud);
        var marker = new google.maps.Marker({
            map: map,
            position: latLong,
            title: '',
            icon: 'Content/img/marker.png',
            zIndex: id
        });
        // map.setCenter(latlng);
        map.setZoom(17);
        $scope.listaMarcadores.push(marker);
    }

    $scope.abrirModalDeMapa = function () {
        debugger;
        $("#modal-mapa-ubicacion").modal('show');
        removerMarcador();
        CrearMarcador(0, $scope.latlngActual)
        map.setCenter($scope.latlngActual);
    }
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
        CrearMarcador(0, latlng);
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
    })

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
        debugger;
        $scope.latlngActual = new google.maps.LatLng($scope.clinicToSave.Latitud, $scope.clinicToSave.Longitud);
        $('#modal-mapa-ubicacion').modal('hide');
    };
    $scope.insertarUbicacionClinica = function () {
        debugger;
        $scope.clinicToSave.Latitud = angular.copy($scope.latlngActual.lat());
        $scope.clinicToSave.Longitud = angular.copy($scope.latlngActual.lng());
        $('#modal-mapa-ubicacion').modal('hide');
    }
    function prepararNuevaClinica() {
        $scope.trabajoClinicaSelected = null;
        $scope.telefonoClinicaSelected = null;
        $scope.primerTrabajo = "";
        $scope.clinicToSave = {
            IDClinica: -1,
            Nombre: "",
            Login: "",
            Latitud: -17.783198,
            Longitud: -63.182046,
            Descripcion: "",
            Nit: "",
            Direccion: "",
            CantidadConsultorios: 0,
            Consultorios: [],
            Trabajos: [],
            Telefonos: [],
            Status: 1
        };
        $scope.latlngActual = new google.maps.LatLng($scope.clinicToSave.Latitud, $scope.clinicToSave.Longitud);
        $scope.clinicaSelected = null;
        prepararNuevoTelefonoClinica();
        prepararNuevoConsultorio();
    }

    function cargar_intervalos() {
        clinicaService.getIntervalosTiempo().then(function (result) {
            $scope.intervalos = result;
        });
    }

    $scope.selectTrabajoConsultorio = function (trabajo, event) {
        $scope.consultorioToSave.Trabajos.push({ ID: trabajo.ID, IDConsultorio: -1, State: 1 });
    };

    $scope.openModalNewConsultorio = function () {
        prepararNuevoConsultorio();
        $("#modal-new-consultorio").modal("show");
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
        prepararNuevoTelefonoClinica();
        var template = "<tr id = \"newTelefonoClinicaId\"> <td><input type=\"text\" class=\"form-control\" id=\"nombreClinicaID\" ng-model=\"telefonoClinicaTemp.Nombre\"> </td><td><input type=\"text\" class=\"form-control\" id=\"telefonoClinicaID\" ng-model=\" telefonoClinicaTemp.Telefono\"></td><td><span ng-click=\"addNewTelefonoClinica()\"><i class=\"fa fa-check\"></i></span><span  ng-click=\"cancelAddTelefonoClinica()\"><i class=\"fa fa-times\"></i></span></td></tr>";
        var linkFn = $compile(template);
        var content = linkFn($scope);

        $("#tableTelefonosClinicaID").append(content);
        $("#nombreClinicaID").focus();
    };

    $scope.showUpdateTelefonoClinica = function () {
        $scope.clinicToSave.Telefonos.splice($scope.indexTelefonoClinicaSelected, 1);
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
                $scope.clinicToSave.Telefonos.push(angular.copy($scope.telefonoClinicaTemp));

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
                $scope.clinicToSave.Telefonos.splice($scope.indexTelefonoClinicaSelected, 0, angular.copy($scope.telefonoClinicaSelected));
                $scope.telefonoClinicaSelected = null;
                $("#updateTelefonoClinicaId").remove();
            }
        }
    };

    $scope.cancelUpdateTelefonoClinica = function () {
        $scope.clinicToSave.Telefonos.splice($scope.indexTelefonoClinicaSelected, 0, angular.copy($scope.telefonoClinicaSelected));
        $scope.telefonoClinicaSelected = null;
        $("#updateTelefonoClinicaId").remove();
    };

    $scope.deleteTelefonoClinica = function () {
        $scope.telefonoClinicaSelected.State = 3;
        $scope.clinicToSave.Telefonos.splice($scope.indexTelefonoClinicaSelected, 1);
        $scope.clinicToSave.Telefonos.splice($scope.indexTelefonoClinicaSelected, 0, angular.copy($scope.telefonoClinicaSelected));
        $scope.telefonoClinicaSelected = null;
    };

    $scope.selectTrabajoClinica = function (trabajoClinica, index) {
        $scope.trabajoClinicaSelected = trabajoClinica;
        $scope.indexTrabajoClinicaSelected = index;
        $scope.telefonoClinicaSelected = null;
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
            $scope.clinicToSave.Trabajos.splice($scope.indexTrabajoClinicaSelected, 0, angular.copy($scope.trabajoClinicaSelected));
            $scope.trabajoClinicaSelected = null;
            $("#updateTrabajoClinicaId").remove();
        } else {
            $("#tDescripcionClinicaID").focus();
        }
    };

    $scope.cancelUpdateTrabajoClinica = function () {
        $scope.clinicToSave.Trabajos.splice($scope.indexTrabajoClinicaSelected, 0, angular.copy($scope.trabajoClinicaSelected));
        $scope.trabajoClinicaSelected = null;
        $("#updateTrabajoClinicaId").remove();
    };

    $scope.deleteTrabajoClinica = function () {
        $scope.trabajoClinicaSelected.State = 3;
        //$scope.clinicToSave.Trabajos.splice($scope.indexTrabajoClinicaSelected, 1);
        $scope.clinicToSave.Trabajos.splice($scope.indexTrabajoClinicaSelected, 0, angular.copy($scope.trabajoClinicaSelected));
        $scope.trabajoClinicaSelected = null;
    };

    $scope.addTrabajo = function () {
        if ($scope.primerTrabajo.length > 0) {
            $scope.clinicToSave.Trabajos.push({ IDClinica: -1, ID: -1, IDConsultorio: [], Descripcion: angular.copy($scope.primerTrabajo), State: 1 });
        }
    };

    $scope.showNewRowTrabajoClinica = function () {
        $scope.primerTrabajo = "";
        $scope.trabajoClinicaSelected = null;
        var template = "<tr id = \"newTrabajoClinicaId\"> <td><input type=\"text\"  class=\"form-control\" id=\"tDescripcionClinicaID\" ng-model=\" primerTrabajo\"> </td><td><span  ng-click=\"addTrabajoClinica()\"><i class=\"fa fa-check\"></i></span><span  ng-click=\"cancelAddTrabajoClinica()\"><i class=\"fa fa-times\"></i></span></td></tr>";
        var linkFn = $compile(template);
        var content = linkFn($scope);

        $("#tablaTrabajosClinicaId").append(content);
        $("#tDescripcionClinicaID").focus();
    };

    $scope.showUpdateRowTrabajoClinica = function () {
        $scope.clinicToSave.Trabajos.splice($scope.indexTrabajoClinicaSelected, 1);
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

    function prepararNuevoConsultorio() {
        cargar_intervalos();
        $scope.consultorioToSave = {
            IDConsultorio: -1,
            Login: $scope.clinicToSave.Status == 1 ? $scope.clinicToSave.Login : $scope.clinicToSave.Login + $scope.clinicToSave.Consultorios.length,
            NIT: "",
            FechaCreacion: "",
            FechaModificacion: "",
            IDUsuarioCreador: "",
            Estado: true,
            Email: "",
            IDIntervalo: -1,
            IDClinica: $scope.clinicToSave.IDClinica,
            Telefonos: [],
            Trabajos: [],
            State: 1
        };
    }

    function cargar_todas_clinicas() {
        clinicaService.getAllClinicasHabilitadas().then(function (result) {
            $scope.allClinicas = result.where(function (r) {
                r.Longitud = r.Longitud.replace(".", ",");
                r.Latitud = r.Latitud.replace(".", ",");
                return r;
            });
        });
    }

    $scope.selectClinica = function (clinica) {
        debugger;
        $scope.trabajoClinicaSelected = null;
        $scope.telefonoClinicaSelected = null;
        $scope.primerTrabajo = "";
        $scope.clinicaSelected = clinica;
        $scope.clinicToSave = angular.copy($scope.clinicaSelected);
        $scope.latlngActual = new google.maps.LatLng(parseFloat($scope.clinicToSave.Latitud.replace(',','.')), parseFloat($scope.clinicToSave.Longitud.replace(',','.')));
        $scope.clinicToSave.Status = 2;
    };

    $scope.validarDatosClinica = function () {
        var exits = $scope.allClinicas.where(function (clinica) {
            return clinica.Login == $scope.clinicToSave.Login;
        });
        if (exits.length > 0) {
            $("#grpLoginClinic").addClass("has-error");
            $("#inpLoginClinic").focus();
            return false;
        }
        return true;
    };

    $scope.validadUsuario = function () {
        return $scope.userToSave == null || $scope.userToSave.Nombre.length < 4 || $scope.userToSave.Login.length < 4
            || $scope.userToSave.Password.length < 4 || $scope.userToSave.Password != $scope.userToSave.ConfirmPass
            || $scope.rolSelected == null;
    };

    $scope.openModalNewRol = function () {
        $scope.nombrerol = "";
        $scope.message = "";
        $('#new-rol').modal('show');
    };

    $scope.openModalConfirmDelele = function () {
        $('#confirm-delete').modal('show');
    };

    $scope.closeWarnig = function () {
        $scope.clinicaSelected = null;
        $('#confirm-delete').modal('hide');
    };

    $scope.eliminarClinica = function () {
        $("#confirm-delete").modal("hide");
        clinicaService.eliminarClinica($scope.clinicaSelected.IDClinica).then(function (result) {
            if (result.Data == 1) {
                toastr.success(result.Message);
                cargar_todas_clinicas();
            } else {
                toastr.error(result.Message);
            }
        });
    };

    $scope.guardarConsultorio = function () {
        $scope.consultorioToSave.IDIntervalo = $scope.intervaloSelected.ID;
        if ($scope.consultorioToSave.State == 1) {
            clinicaService.insertarConsultorio($scope.consultorioToSave).then(function (result) {
                if (result.Data == 1) {
                    toastr.success(result.Message);
                    $("#modal-new-consultorio").modal("hide");
                } else {
                    toastr.error(result.Message);
                }
            });
        } else {
            usuariosService.modificarUsuario($scope.userToSave).then(function (result) {
                if (result.Data == 1) {
                    cargar_todos_los_usuarios();
                    toastr.success(result.Message);
                    prepararNuevoUsuario();
                } else {
                    toastr.error(result.Message);
                }
            });
        }
    };

    $scope.guardarClinica = function () {
        if ($scope.clinicToSave.Status == 1) { //nueva 
            if ($scope.validarDatosClinica()) {
                $scope.consultorioToSave.IDIntervalo = angular.copy($scope.intervaloSelected.ID);
                $scope.clinicToSave.Consultorios.push(angular.copy($scope.consultorioToSave));
                clinicaService.insertarClinica($scope.clinicToSave).then(function (result) {
                    if (result.Success) {
                        toastr.success(result.Message);
                        prepararNuevaClinica();
                    } else {
                        toastr.error(result.Message);
                    }
                });
            }
        } else if ($scope.clinicToSave.Status == 2) {

            $scope.consultorioToSave.IDIntervalo = $scope.intervaloSelected == null ? $scope.consultorioToSave.IDIntervalo : angular.copy($scope.intervaloSelected.ID);
            clinicaService.modificarClinica($scope.clinicToSave).then(function (result) {
                if (result.Success) {
                    toastr.success(result.Message);
                    cargar_todas_clinicas();
                    prepararNuevaClinica();
                } else {
                    toastr.error(result.Message);
                }
            });
        }
    };
});