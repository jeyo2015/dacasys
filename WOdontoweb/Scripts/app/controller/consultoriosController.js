app.controller("consultoriosController", function (clinicaService, $scope, $compile, $rootScope, loginService) {
    init();

    $('#fileupload').fileupload({
        dataType: 'json',
        url: 'odontoweb/Empresa/UploadFiles',
        acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
        maxFileSize: 10000000, // 10 MB
        minFileSize: undefined,
        maxNumberOfFiles: 1,
        done: function (event, data) {
            if (data.result.isUploaded) {
                $scope.clinicToSave.NombreArchivo = data.result.name;
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
    function init() {
        $scope.listaMarcadores = [];
        $scope.allClinicas = [];

        $scope.clinicaSelected = null;

        if (!$rootScope.sessionDto) {
            loginService.getSessionDto().then(function (result) {
                $rootScope.sessionDto = result;
                inicializarDatos();
            });
        } else {
            inicializarDatos();
        }

    };
    function inicializarDatos() {
      
        $("#dtpFecha").datepicker({
            dateFormat: 'dd/mm/yy',
            onSelect: function () {
                $scope.clinicToSave.FechaInicioLicencia = $.datepicker.formatDate("dd/mm/yy", $(this).datepicker('getDate'));
                $scope.mostrarInputMeses = true;
                $scope.$apply();
            }
        });
        $('#dtpFecha').val(moment().format('DD/MM/YYYY'));
        prepararNuevaClinica();
        InicializarMapa();
        cargar_todas_clinicas(false);
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

    $scope.salirModal = function () {
        $("#modal-new-consultorio").modal("hide");
        $scope.consultorioToSave = null;
        $scope.consultorioSeleccionado = null;
    };

    $scope.abrirModaleliminarConsultorio = function () {
        $("#confirmar-eliminar-consultorio").modal("show");
    };
    $scope.eliminarConsultorio = function () {
        $("#confirmar-eliminar-consultorio").modal("hide");
        clinicaService.eliminarConsultorio($scope.consultorioSeleccionado.IDConsultorio).then(function (result) {
            if (result.Data == 1) {
                toastr.success(result.Message);
                cargar_todas_clinicas(true);
                $scope.consultorioSeleccionado = null;
            } else {
                toastr.error(result.Message);
            }
        });
    };

    $scope.habilitarConsultorio = function () {
        clinicaService.habilitarConsultorio($scope.consultorioSeleccionado.IDConsultorio).then(function (result) {
            if (result.Data == 1) {
                toastr.success(result.Message);
                //cargarConsultoriosClinica();
                cargar_todas_clinicas(true);

                $scope.consultorioSeleccionado = null;
            } else {
                toastr.error(result.Message);
            }
        });
    };

    $scope.salirModalUbicacion = function () {

        $scope.latlngActual = new google.maps.LatLng($scope.clinicToSave.Latitud, $scope.clinicToSave.Longitud);
        $('#modal-mapa-ubicacion').modal('hide');
    };
    $scope.insertarUbicacionClinica = function () {

        $scope.clinicToSave.Latitud = angular.copy($scope.latlngActual.lat());
        $scope.clinicToSave.Longitud = angular.copy($scope.latlngActual.lng());
        $('#modal-mapa-ubicacion').modal('hide');
    };
    function prepararNuevaClinica() {
        $scope.consultorioSeleccionado = null;
        $scope.trabajoClinicaSelected = null;
        $scope.telefonoClinicaSelected = null;
        $scope.mostrarInputMeses = false;
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
            Status: 1,
            NombreArchivo: ""
          //  FechaInicioLicencia: $('#dtpFecha').datepicker("getDate"),
          //  CantidadMeses:1
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
        $scope.consultorioSeleccionado = null;
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

    $scope.habilitarClinica = function () {
        clinicaService.habilitarClinica($scope.clinicaSelected.IDClinica).then(function (result) {
            if (result.Data == 1) {
                toastr.success(result.Message);
                cargar_todas_clinicas(false);
                $scope.clinicaSelected = null;
                $scope.clinicToSave = null;
            } else {
                toastr.error(result.Message);
            }
        });
    }

    function cargar_todas_clinicas(seleccionarClinica) {
        clinicaService.getAllClinicas().then(function (result) {
            $scope.allClinicas = result.where(function (r) {
                r.Longitud = r.Longitud.replace(".", ",");
                r.Latitud = r.Latitud.replace(".", ",");
                return r;
            });
            if (seleccionarClinica) {
                $scope.clinicToSave = $scope.allClinicas.where(function (ele) {
                    return ele.IDClinica == $scope.clinicaSelected.IDClinica
                })[0];
                $scope.clinicToSave.Status = 2;
                $scope.clinicaSelected = $scope.clinicToSave;
            }
        });
    }

    $scope.selectClinica = function (clinica) {

        $scope.trabajoClinicaSelected = null;
        $scope.telefonoClinicaSelected = null;
        $scope.consultorioSeleccionado = null;
        $scope.primerTrabajo = "";
        $scope.clinicaSelected = clinica;
        if (clinica.Estado) {
            $scope.clinicToSave = angular.copy($scope.clinicaSelected);
            $scope.latlngActual = new google.maps.LatLng(parseFloat($scope.clinicToSave.Latitud.replace(',', '.')), parseFloat($scope.clinicToSave.Longitud.replace(',', '.')));
            $scope.clinicToSave.Status = 2;
        } else {
            toastr.warning("La clinica no esta habilitada");
        }

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
                cargar_todas_clinicas(false);
                $scope.clinicaSelected = null;
                $scope.clinicToSave = null;
            } else {
                toastr.error(result.Message);
            }
        });
    };
    $scope.selectConsultorio = function (consultorio) {
        $scope.consultorioSeleccionado = angular.copy(consultorio);
        $scope.consultorioSeleccionado.State = 2;
    }
    $scope.abrirModalModificarConsultorio = function () {
        $scope.consultorioToSave = angular.copy($scope.consultorioSeleccionado);
        $scope.intervaloSelected = $scope.intervalos.where(function (intervalo) {
            return intervalo.ID == $scope.consultorioToSave.IDIntervalo;
        })[0];
        matchearTrabajoConsultorio();
        $("#modal-new-consultorio").modal("show");
    };
    function matchearTrabajoConsultorio() {
        var CantTrabajos = $scope.clinicToSave.Trabajos.length;
        for (var i = 0; i < CantTrabajos ; i++) {
            var trabajoEnLista = $scope.consultorioToSave.Trabajos.where(function (ele) {
                return ele.ID == $scope.clinicToSave.Trabajos[i].ID;
            });
            if (trabajoEnLista && trabajoEnLista.length > 0)
                $scope.clinicToSave.Trabajos[i].checked = true;
            else
                $scope.clinicToSave.Trabajos[i].checked = false;
        }
    }
    $scope.guardarConsultorio = function () {
        $scope.consultorioToSave.IDIntervalo = $scope.intervaloSelected.ID;
        if ($scope.consultorioToSave.State == 1) {
            clinicaService.insertarConsultorio($scope.consultorioToSave).then(function (result) {
                if (result.Data == 1) {
                    toastr.success(result.Message);
                    $("#modal-new-consultorio").modal("hide");
                    cargar_todas_clinicas(true);
                } else {
                    toastr.error(result.Message);
                }
            });
        }
        else {
            clinicaService.modificarConsultorio($scope.consultorioToSave).then(function (result) {
                if (result.Data == 1) {
                    $scope.consultorioSeleccionado = null;
                    toastr.success(result.Message);
                    $("#modal-new-consultorio").modal("hide");
                    cargar_todas_clinicas(true);
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
                        cargar_todas_clinicas();
                    } else {
                        toastr.error(result.Message);
                    }
                });
            }
        } else if ($scope.clinicToSave.Status == 2) {
            $scope.insertarUbicacionClinica();
            // $scope.consultorioToSave.IDIntervalo = $scope.intervaloSelected == null ? $scope.consultorioToSave.IDIntervalo : angular.copy($scope.intervaloSelected.ID);
            clinicaService.modificarClinica($scope.clinicToSave).then(function (result) {
                if (result.Success) {
                    toastr.success(result.Message);
                    cargar_todas_clinicas(false);
                    prepararNuevaClinica();
                } else {
                    toastr.error(result.Message);
                }
            });
        }
    };
});