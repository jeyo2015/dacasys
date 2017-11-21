app.controller("loginController", function (loginService, $scope, $rootScope, $location, usuariosService, notificacionesConsultorioService, clinicaService, pacienteService, $compile) {
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
            $scope.$apply();
        });

    }

    google.maps.event.addDomListener(window, "resize", resizingMap());

    $('#mapadiv').on('show.bs.collapse', function () {
        //alert("collapse");
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
    var map;

    function CrearMarcador(id, latLong) {
        // $scope.latlngActual = new google.maps.LatLng(latitud, longitud);
        $scope.clinicToSave.Latitud = latLong.lat().toFixed(5);
        $scope.clinicToSave.Longitud = latLong.lng().toFixed(5);
        var marker = new google.maps.Marker({
            map: map,
            position: latLong,
            title: '',
            icon: $scope.baseURL + 'Content/img/marker.png',
            zIndex: id
        });
        // map.setCenter(latlng);
        map.setZoom(17);
        $scope.listaMarcadores.push(marker);

    }

    $scope.abrirModalDeMapa = function () {

        $("#modal-mapa-ubicacion").modal('show');
        removerMarcador();
        if ($scope.clinicaSelected == null) {
            getLocation();
        } else {
            CrearMarcador(0, $scope.latlngActual);
            map.setCenter($scope.latlngActual);
        }

    };
    function showPosition(position) {
        //markerCurrent = new google.maps.Marker({
        //    map: map,
        //    position: new google.maps.LatLng(position.coords.latitude, position.coords.longitude),
        //    title: 'Tu posicion actual',
        //    icon: $scope.baseURL + 'Content/img/marker.png',
        //    zIndex: -10000
        //});
        //markers.push(markerCurrent);
        //map.setCenter(new google.maps.LatLng(position.coords.latitude, position.coords.longitude));
        //$scope.latlngActual = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
        $scope.latlngActual = position;//new google.maps.LatLng(-17.783198, -63.182046);
        CrearMarcador(0, $scope.latlngActual);
        map.setCenter($scope.latlngActual);

    }
    function onError(error) {
        $scope.latlngActual = new google.maps.LatLng(-17.783198, -63.182046);
        CrearMarcador(0, $scope.latlngActual);
        map.setCenter($scope.latlngActual);
    }

    function getLocation() {
        showPosition(new google.maps.LatLng(-17.783198, -63.182046));
        //if (navigator.geolocation) {
        //    navigator.geolocation.getCurrentPosition(showPosition, onError);
        //} else {
        //    x.innerHTML = "Geolocation is not supported by this browser.";
        //}
    }


    init();
    $rootScope.irAtrasBotonera = function (e) {
        e.preventDefault();
        $rootScope.irAtras = false;
        $location.path('/inicioBotonera');
    }
    function init() {
        prepararNuevaClinica();
        $scope.baseURL = $("#basePath").attr("href");
        $scope.listaMarcadores = [];
        InicializarMapa();
        $rootScope.irAtras = false;
        $scope.loginEmpresa = "";
        $scope.usuario = "";
        $rootScope.pass = "";
        $rootScope.isAdmin = false;
        $scope.isUser = -1;
        $scope.newPass = "";
        $scope.ConfirmPass = "";
        $scope.message = "";
        prepararNuevoCliente();
        $rootScope.selectSexo = "F";
        $rootScope.selectTipoSangre = "O+";
        $rootScope.mostrarMenu = false;
        $scope.pass = "";
        $('#passwordIDLog').attr('autocomplete', 'off');
        cleanInputsf();
        $rootScope.sessionDto = null;
        cargar_intervalos();
    };


    function cleanInputsf() {
        consultorioElemento = $("#formGroupConsultorio");
        consultorioElemento.addClass("is-empty");
        $scope.loginEmpresa = null;
        consultorioElemento = $("#formGroupUsuario");
        consultorioElemento.addClass("is-empty");
        $scope.usuario = null;
        consultorioElemento = $("#formGroupPass");
        consultorioElemento.addClass("is-empty");
        $scope.pass = null;

    }
    $scope.cleanInputs = function () {
        cleanInputsf();

    }
    $rootScope.cerrarModalRegistrar = function () {
        $("#modal-registrarse").modal('hide');
    }
    function prepararNuevoCliente() {
        $rootScope.pacienteParaGuardar = {
            LoginCliente: '',
            Nombre: '',
            Apellido: '',
            Ci: '',
            Telefono: '',
            Email: '',
            Direccion: '',
            TipoSangre: '',
            Sexo: '',
            Antecedentes: '',
            State: 1,
            IdPaciente: 0,
            IsPrincipal: true,
            IDEmpresa: -1
        };
    };

    $rootScope.validarCamposPaciente = function () {

        if ($rootScope.pacienteParaGuardar && $rootScope.pacienteParaGuardar.IsPrincipal) {
            return $rootScope.pacienteParaGuardar == null || $rootScope.selectSexo == null || $rootScope.selectTipoSangre == null

        || $rootScope.pacienteParaGuardar.Nombre.length <= 0 || $rootScope.pacienteParaGuardar.Apellido.length <= 0 || $rootScope.pacienteParaGuardar.LoginCliente.length <= 0;
        } else {
            return $rootScope.pacienteParaGuardar == null || $rootScope.selectSexo == null || $rootScope.selectTipoSangre == null
            || $rootScope.pacienteParaGuardar.Nombre.length <= 0 || $rootScope.pacienteParaGuardar.Apellido.length <= 0 || $rootScope.pacienteParaGuardar.LoginCliente.length <= 0;
        }
    };

    $rootScope.registrarCliente = function () {

        $rootScope.pacienteParaGuardar.Sexo = $rootScope.selectSexo;
        $rootScope.pacienteParaGuardar.TipoSangre = $rootScope.selectTipoSangre;
        $rootScope.pacienteParaGuardar.Email = $rootScope.pacienteParaGuardar.LoginCliente;
        if ($rootScope.pacienteParaGuardar.State == 1) {
            pacienteService.insertarPaciente($rootScope.pacienteParaGuardar).then(function (result) {
                if (result.Data == 1) {
                    toastr.success(result.Message);
                    $("#modal-registrarse").modal('hide');
                } else {
                    toastr.error(result.Message);
                }
            });
        }

    };
    $scope.cerrarModalNuevoConsultorio = function () {
        $("#registrar-consultorio").modal("hide");
        prepararNuevaClinica();
    }


    $(window).resize(function () {
        setTimeout(function () {
            
            menuClick();
        }, 300);

    });
    function coseno(ra) {
        return Math.cos(40) * ra;
    }
    function seno(ra) {
        return Math.sin(40) * ra;
    }

    function circleText(text, x, y, radius, startRotation, eangulo, ctx) {
         var numDegreesPerLetter = eangulo / text.length;
        ctx.save();
        ctx.translate(x, y);
        ctx.rotate(startRotation);

        for (var i = 0; i < text.length; i++) {
            ctx.save();
            ctx.translate(radius, 0);

            ctx.translate(10, -10);

            ctx.rotate(1.4)
            ctx.translate(-10, 10);
            ctx.fillText(text[i], 0, 0);
            ctx.restore();
            ctx.rotate(numDegreesPerLetter);
        }
        ctx.restore();
    }
    function dibujarMenu(c, xc, yc, r, r2, hr, wr) {
        var ctx = c.getContext("2d");
        c.height = hr;
        c.width = wr;
        ctx.beginPath();
        ctx.fillStyle = '#ff0000';
        ctx.strokeStyle = '#ff0000';
        ctx.arc(xc, yc, r - 5, 0, 2 * Math.PI);
        ctx.fill();
        ctx.stroke();
        var sangulo = 133 * (Math.PI / 180);
        var eangulo = 227 * (Math.PI / 180);
        ctx.strokeStyle = '#04B45F';
        for (var i = r; i < r2; i++) {
            ctx.beginPath();
            ctx.arc(xc, yc, i, sangulo, eangulo);
            ctx.stroke();
        }
        sangulo = 229 * (Math.PI / 180);
        eangulo = 311 * (Math.PI / 180);
        ctx.strokeStyle = '#18114F';
        for (var i = r; i < r2; i++) {
            ctx.beginPath();
            ctx.arc(xc, yc, i, sangulo, eangulo);
            ctx.stroke();
        }
        sangulo = 313 * (Math.PI / 180);
        eangulo = 47 * (Math.PI / 180);
       
        ctx.strokeStyle = '#18114F';
        for (var i = r; i < r2; i++) {
            ctx.beginPath();
            ctx.arc(xc, yc, i, sangulo, eangulo);
            ctx.stroke();
        }



        sangulo = 49 * (Math.PI / 180);
        eangulo = 131 * (Math.PI / 180);
       
        for (var i = r; i < r2; i++) {
            ctx.beginPath();
            ctx.strokeStyle = '#04B45F';
           // ctx.translate(1, 0);
            ctx.arc(xc, yc, i, sangulo, eangulo);
            ctx.stroke();
        }


        //textos

        
        ctx.font = "bold 15px Roboto";
        ctx.fillStyle = '#fff';
        var px = getTextWidth("BUSCA TU CONSULTORIO", "bold 15px Roboto");
        var eanguloC = 50 * (Math.PI / 180);
        var espacioCr = 0;
        if (px >= ((r * 2) - 20)) {
            px = getTextWidth("BUSCA ", "bold 12px Roboto");
            var espacio = (px / 2);
            ctx.font = "bold 12px Roboto";
            ctx.fillText("BUSCA ", xc - espacio, yc - 11);
            px = getTextWidth("TU ", "bold 12px Roboto");
            espacio = (px / 2);
            ctx.fillText("TU ", xc - espacio, yc);
            px = getTextWidth("CONSULTORIO ", "bold 12px Roboto");
            espacio = (px / 2);
            ctx.fillText("CONSULTORIO ", xc - espacio, yc + 11);
            eanguloC = 65 * (Math.PI / 180);
            espacioCr = 5;
        } else {
            var espacio = (px / 2);
            
            ctx.fillText("BUSCA TU CONSULTORIO", xc - espacio, yc);
        }
        sangulo = 165 * (Math.PI / 180);
        eangulo = 50 * (Math.PI / 180);
        circleText("REGISTRAR ", xc, yc, r + (((r2 - r) / 2) + 10), sangulo, eangulo, ctx);
        circleText("USUARIO ", xc, yc, r + (((r2 - r) / 2) - 10), sangulo, eangulo, ctx);

        sangulo = 349 * (Math.PI / 180);
        eangulo = 50 * (Math.PI / 180);
        circleText("INGRESAR ", xc, yc, r + (((r2 - r) / 2) + 10), sangulo, eangulo, ctx);
        circleText("USUARIO ", xc, yc, r + (((r2 - r) / 2) - 10), sangulo, eangulo, ctx);

        sangulo = (76.5 - espacioCr) * (Math.PI / 180) ;
       
        circleText("REGISTRAR ", xc, yc, r + (((r2 - r) / 2) + 10), sangulo, eanguloC, ctx);
        circleText("CONSULTORIO ", xc, yc, r + (((r2 - r) / 2) - 10), sangulo, eanguloC, ctx);
        sangulo = (255 - espacioCr) * (Math.PI / 180);
       
        circleText("INGRESAR ", xc, yc, r + (((r2 - r) / 2) + 10), sangulo, eanguloC, ctx);
        circleText("CONSULTORIO ", xc, yc, r + (((r2 - r) / 2) - 10), sangulo, eanguloC, ctx);

        
    }

    function getTextWidth(text, font) {
        // re-use canvas object for better performance
        var canvas = getTextWidth.canvas || (getTextWidth.canvas = document.createElement("canvas"));
        var context = canvas.getContext("2d");
        context.font = font;
        var metrics = context.measureText(text);
        return metrics.width;
    }
    function menuClick() {
        var c = document.getElementById("canvas");
        if (c == null) return;
        var wr = $("#divcanvas").width();
        var h = $(window).height();
        var hr = h - $("#headerTotal").height() - 102;

        var yc = hr / 2;
        var xc = wr / 2;
        var minor = wr < hr ? wr : hr;
        var r2 = (minor * 0.9) / 2;
        var r = (minor * 0.4) / 2;

        dibujarMenu(c, xc, yc, r, r2, hr, wr);
        c.onmousedown = function (e) {
            var cl = c.getContext("2d");
            var rect = this.getBoundingClientRect(),
              x = e.clientX - rect.left,
              y = e.clientY - rect.top;
            // cl.clearRect(0, 0, c.width, c.height);
            cl.beginPath();
            var dx = x - xc;
            var dy = y - yc;

            var isInsideCentro = dx * dx + dy * dy <= (r - 5) * (r - 5);
            if (isInsideCentro) {
                $scope.abrirMapaInicio();
                $scope.$apply();
            } else {

                if (dx * dx + dy * dy >= r * r && dx * dx + dy * dy <= r2 * r2)// esta afuera de circulo pequeno
                {
                    var ca = (x - xc);
                    var co = (y - yc);
                    var h = Math.sqrt((ca * ca) + (co * co));
                    var angulo = (Math.acos(Math.abs(ca) / h)) / (Math.PI / 180);
                    if (ca < 0 && co < 0)//2Registrar usuario o 3 Ingrear consultorio 4 Ingreserar usuario 5 registrar consltorio
                    {
                        if (angulo <= 40)
                            $scope.openModalregistrarCliente();
                        else
                            $scope.showModalIngresar(true);
                    } else {
                        if (ca > 0 && co < 0)//3 o 4
                        {
                            if (angulo <= 40)
                                $scope.showModalIngresar(false);
                            else
                                $scope.showModalIngresar(true);
                        } else {
                            if (ca > 0 && co > 0) {
                                if (angulo <= 40)
                                    $scope.showModalIngresar(false);
                                else
                                    $scope.openModalregistrarClinica();
                            } else
                                if (ca < 0 && co > 0) {

                                    if (angulo <= 40)
                                        $scope.openModalregistrarCliente();
                                    else
                                        $scope.openModalregistrarClinica();
                                }

                        }
                    }
                }
                $scope.$apply();
            }



        }
    }
    function cargar_intervalos() {
        clinicaService.getIntervalosTiempo().then(function (result) {
            $rootScope.intervalos = result;

            menuClick();
            $rootScope.intervaloSelected = result.where(function (ele) {
                return ele.ID == 2;
            })[0];
        });
    }
    $scope.showUpdateTelefonoClinica = function () {
        $scope.consultorioSeleccionado = null;
        $scope.clinicToSave.Telefonos.splice($scope.indexTelefonoClinicaSelected, 1);
        var template = "<tr id = \"updateTelefonoClinicaId\"> <td><input type=\"text\" class=\"form-control\" id=\"nombreClinicaID\" ng-model=\"telefonoClinicaSelected.Nombre\"> </td><td><input type=\"text\" class=\"form-control\" id=\"telefonoClinicaID\" ng-model=\" telefonoClinicaSelected.Telefono\"></td><td><span ng-click=\"updateTelefonoClinica()\"><i class=\"fa fa-check\"></i></span><span  ng-click=\"cancelUpdateTelefonoClinica()\"><i class=\"fa fa-times\"></i></span></td></tr>";
        var linkFn = $compile(template);
        var content = linkFn($scope);

        $("#tableTelefonosClinicaID").append(content);
        $("#nombreClinicaID").focus();
    };

    $scope.validarCamposClinica = function () {
        return (!$scope.clinicToSave || $scope.clinicToSave.Telefonos.length == 0 || $scope.clinicToSave.Trabajos.length == 0
              || $scope.clinicToSave.Direccion == "" || $scope.clinicToSave.Email == "" || $scope.clinicToSave.Login == ""
              || $scope.clinicToSave.Nombre == "" || $scope.intervaloSelected == null);
    }
    $scope.selectTelefonoClinica = function (telefonoClinica, index) {
        $scope.telefonoClinicaSelected = telefonoClinica;
        $scope.indexTelefonoClinicaSelected = index;
        $scope.trabajoClinicaSelected = null;
        $scope.consultorioSeleccionado = null;
    };
    $scope.crearNuevaClinicaGratis = function () {
        if ($scope.clinicToSave.Status == 1) { //nueva 
            //if ($scope.validarDatosClinica()) {
            $scope.consultorioToSave.IDIntervalo = angular.copy($scope.intervaloSelected.ID);
            $scope.clinicToSave.Consultorios.push(angular.copy($scope.consultorioToSave));
            $scope.clinicToSave.Longitud = $scope.clinicToSave.Longitud.toString().replace(",", ".");
            $scope.clinicToSave.Latitud = $scope.clinicToSave.Latitud.toString().replace(",", ".");
            clinicaService.insertarClinica($scope.clinicToSave).then(function (result) {
                if (result.Success) {
                    toastr.success("Su consultorio se ha registrado exitosamente, en unos minutos le llegara a su email los datos de acceso");
                    $("#registrar-consultorio").modal("hide");
                } else {
                    if (result.Data == -10) {
                        toastr.error("El nombre de acceso ya se encuentra registrado");
                        $("#inpLoginClinic").focus();
                    } else {
                        toastr.error(result.Message);
                    }
                }
            });
            // }
        }
    };
    function prepararNuevaClinica() {
        $rootScope.trabajoClinicaSelected = null;
        $rootScope.telefonoClinicaSelected = null;
        $rootScope.primerTrabajo = "";
        $rootScope.clinicToSave = {
            IDClinica: -1,
            Nombre: "",
            Login: "",
            TipoLicencia: 1,
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
            NombreArchivo: "",
            FechaInicioLicencia: new Date(),
            FechaInicioLicenciaString: moment(new Date()).format('DD/MM/YYYY'),
            CantidadMeses: 1
        };

    }
    $rootScope.openModalregistrarClinica = function () {
        prepararNuevaClinica();
        $("#registrar-consultorio").modal('show');
    }
    $rootScope.openModalregistrarCliente = function () {
        $scope.usuario = "";
        $scope.pass = "";
        prepararNuevoCliente();
        $("#modal-login").modal('hide');
        $("#modal-login-cliente").modal('hide');
        $("#modal-registrarse").modal('show');
    }
    $rootScope.enterLogIn = function (keyEvent) {
        console.log("ingresar");
        if (keyEvent.which === 13)
            $scope.ingresar();
    };
    $rootScope.getClass = function (path) {
        return ($location.path().substr(0, path.length) === path) ? 'active' : '';
    };
    function getNotificaciones() {
        if ($rootScope.sessionDto.IDConsultorio != -1)
            notificacionesConsultorioService.getSolicitudesPacientes($rootScope.sessionDto.IDConsultorio, 1).then(function (result) {
                $rootScope.NotificacionesConsultorio = result;
                if ($rootScope.sessionDto.IDRol != null) {
                    $('.modal-backdrop').remove();
                    $('body').removeClass("modal-open");

                    $location.path('/' + $rootScope.primerModulo());
                }
            });
    }


    $rootScope.cerrarSesion = function (e) {
        e.preventDefault();
        loginService.cerrarSesion().then(function (result) {
            $rootScope.sessionDto = result;
            $location.path('/inicioBotonera');



        });
    };

    $rootScope.abrirMapaInicio = function () {
        $rootScope.irAtras = true;
        $location.path('/inicioCliente');
    }
    $rootScope.showModal = function (e) {

        e.preventDefault();
        init();

        $('#modal-login').modal('show');
    };
    $rootScope.showModalIngresar = function (isadmin) {
        //init();
        $scope.loginEmpresa = null;
        $scope.usuario = null; $scope.pass = null;
        cleanInputsf();
        $rootScope.isAdmin = Boolean(isadmin);
        $('#modal-login').modal('show');
    }

    function prepararNuevoPerfil() {
        $scope.userToSave = {
            Nombre: angular.copy($rootScope.sessionDto.Nombre),
            Login: angular.copy($rootScope.sessionDto.Login),
            Password: "",
            ConfirmPass: "",
            IDEmpresa: angular.copy($rootScope.sessionDto.IDConsultorio),
            IDRol: angular.copy($rootScope.sessionDto.IDRol),
            Estado: true
        };
    }


    $rootScope.validarPermisoModulo = function (nombreModulo) {
        if ($rootScope.sessionDto && $rootScope.sessionDto.Permisos) {
            var listModulo = $rootScope.sessionDto.Permisos.Modulos.where(function (modulo) {
                return modulo.NombreModulo == nombreModulo;
            });
            return listModulo.length <= 0 ? false : listModulo[0].TienePermiso;
        } else {
            return false;
        }
    };

    $rootScope.validarPermisoFormulario = function (nombreFormulario) {
        if ($rootScope.sessionDto && $rootScope.sessionDto.Permisos) {
            var listFormulario = $rootScope.sessionDto.Permisos.Formularios.where(function (formulario) {
                return formulario.NombreFormulario == nombreFormulario;
            });
            return listFormulario.length <= 0 ? false : listFormulario[0].TienePermiso;
        } else {
            return false;
        }
    };

    $rootScope.validarPermisoComponente = function (nombreComponente) {

        if ($rootScope.sessionDto && $rootScope.sessionDto.Permisos) {
            var listComponente = $rootScope.sessionDto.Permisos.Componentes.where(function (componente) {
                return componente.NombreComponente == nombreComponente;
            });
            return listComponente.length <= 0 ? false : listComponente[0].TienePermiso;
        } else {
            return false;
        }
    };







    $rootScope.openModalChangePass = function (e) {
        e.preventDefault();
        prepararNuevoPerfil();
        usuariosService.getUsuarioConsultorio($rootScope.sessionDto.loginUsuario, $rootScope.sessionDto.IDConsultorio).then(function (result) {
            $scope.userToSave = result;

            $('#modal-mi-perfil').modal('show');
        });
    };

    $scope.ingresar = function () {
        var verificar = loginService.ingresar($scope.loginEmpresa, $scope.usuario, $scope.pass);

        verificar.then(function (result) {

            $scope.message = result.Message;
            $rootScope.sessionDto = result.Data;
            switch (result.Data.Verificar) {
                case 1:
                    $("#consultorioID").focus();
                    $scope.loginEmpresa = "";
                    $scope.usuario = "";
                    $scope.pass = "";

                    break;
                case 0:
                    $("#consultorioID").focus();
                    $scope.loginEmpresa = "";
                    $scope.usuario = "";
                    $scope.pass = "";
                    break;
                case 2:
                    $("#usuarioID").focus();
                    $scope.usuario = "";
                    $scope.pass = "";
                    break;
                case 4:
                    $("#passwordID").focus();

                    $scope.pass = "";
                    break;
                case 3:
                    $rootScope.mostrarMenu = true;
                    $('#modal-login-cliente').modal('hide');
                    $('#modal-login').modal('hide');

                    if ($scope.isAdmin) {
                        if ($rootScope.sessionDto.ChangePass) {
                            $('#modal-renovar').modal('show');
                            $scope.showMessage = false;
                        }

                        getNotificaciones();

                    } else {
                        if ($rootScope.comentarioParaGuardar)
                            $rootScope.comentarioParaGuardar.LoginCliente = $rootScope.sessionDto.loginUsuario;
                        if ($rootScope.sessionDto.ChangePass) {
                            $('#modal-renovar').modal('show');
                            $scope.showMessage = false;
                        } else {
                            $location.path('/inicioCliente');

                        }

                    }

                    break;
            }

        });
    };

    $scope.renovarContrasena = function () {
        if ($scope.isAdmin) {
            if ($scope.loginEmpresa.length == 0) {
                $scope.message = "Ingrese consultorio por favor";
                $rootScope.sessionDto.Verificar = 1;
                $("#consultorioID").focus();
                return;
            }
        }
        if ($scope.usuario.length == 0) {
            $scope.message = "Ingrese su login de usuario por favor";
            $rootScope.sessionDto.Verificar = 2;
            $("#usuarioID").focus();
            return;
        }

        loginService.forgotPass($scope.loginEmpresa, $scope.usuario).then(function (result) {
            $scope.message = result.Message;

            switch (result.Data) {
                case 3:
                    toastr.success(result.Message);
                    $scope.loginEmpresa = "";
                    $scope.usuario = "";
                    $scope.pass = "";
                    $('#modal-login').modal('hide');
                    break;
                case 2:
                    toastr.error(result.Message);
                    $scope.loginEmpresa = "";
                    $scope.usuario = "";
                    $scope.pass = "";
                    break;
                case 1:
                    $scope.sessionDto.Verificar = 2;

                    $("#usuarioID").focus();
                    $scope.usuario = "";
                    $scope.pass = "";
                    break;
                case 0:
                    $scope.sessionDto.Verificar = 1;
                    $("#consultorioID").focus();
                    $scope.loginEmpresa = "";
                    $scope.usuario = "";
                    $scope.pass = "";
                    break;
            }
        });
    };

    $scope.changePassUser = function () {
        if ($scope.newPass != $scope.ConfirmPass) {
            $scope.message = "Las contrasenas no coinciden";
            $scope.newPass = "";
            $scope.ConfirmPass = "";
            $scope.showMessage = true;
            $("#newPasswordID").focus();
            return;
        }
        if ($scope.newPass.length <= 3) {
            $scope.message = "Contrasena muy corta, debe ser mayor a 4 caracteres";
            $scope.newPass = "";
            $scope.ConfirmPass = "";
            $scope.showMessage = true;
            $("#newPasswordID").focus();
            return;
        }

        loginService.renovarContrasena($scope.isAdmin, $scope.usuario, $scope.newPass, $scope.loginEmpresa).then(function (result) {
            if (result.Data == 1) {

                toastr.success(result.Message);
                $('#modal-renovar').modal('hide');
                $scope.newPass = "";
                $scope.ConfirmPass = "";
                $('.modal-backdrop').remove();
                $('body').removeClass("modal-open");
                $location.path('/inicioCliente');
            } else {
                toastr.error(result.Message);
            }

        });
    };

    $scope.updateUser = function () {
        if ($scope.userToSave.ConfirmPass != $scope.userToSave.Password) {
            $scope.message = "Las contrasenas no coinciden";
            $scope.userToSave.Password = "";
            $scope.userToSave.ConfirmPass = "";
            $scope.showMessage = true;
            $("#newPasswordID").focus();
            return;
        }
        if ($scope.userToSave.Password.length <= 3) {
            $scope.message = "Contrasena muy corta, debe ser mayor a 4 caracteres";
            $scope.userToSave.Password = "";
            $scope.userToSave.ConfirmPass = "";
            $scope.showMessage = true;
            $("#newPasswordID").focus();
            return;
        }

        usuariosService.modificarUsuario($scope.userToSave).then(function (result) {
            if (result.Data == 1) {
                toastr.success(result.Message);
                $rootScope.sessionDto.Nombre = angular.copy($scope.userToSave.Nombre);
            } else {
                toastr.error(result.Message);
            }
            $('#modal-mi-perfil').modal('hide');
            prepararNuevoPerfil();
        });
    };

    $scope.validarCampos = function () {
        if ($scope.isAdmin) {
            return $scope.usuario == null || $scope.pass == null || $scope.loginEmpresa == null || $scope.usuario.length == 0 || $scope.pass.length == 0 || $scope.loginEmpresa.length == 0;
        } else return $scope.usuario == null || $scope.pass == null || $scope.usuario.length == 0 || $scope.pass.length == 0;
    };
    $scope.validarCamposInicioCliente = function () {
        return $scope.usuario == null || $scope.pass == null || $scope.usuario.length == 0 || $scope.pass.length == 0;
    };
    $scope.validarCamposPerfil = function () {
        if ($scope.userToSave) {
            return $scope.userToSave.Nombre.length == 0 || $scope.userToSave.Password.length == 0 || $scope.userToSave.ConfirmPass.length == 0;
        } else return false;
    };

    $scope.closeModal = function (nameModal) {
        $(nameModal).modal('hide');
    };

    $rootScope.desabilitarNuevasNotificaciones = function (e) {
        e.preventDefault();
        notificacionesConsultorioService.deshabilitarNuevasNotificaciones($rootScope.sessionDto.IDConsultorio, 1).then(function (result) {
            if (result.Success)
                $rootScope.NotificacionesConsultorio = result.Data;
        });
    };

    $rootScope.confirmarSolicitud = function (notificacion, e) {
        e.preventDefault();
        notificacionesConsultorioService.aceptarSolicitudPaciente(notificacion).then(function (result) {
            if (result.Success)
                $rootScope.NotificacionesConsultorio = result.Data;
        });
    };

    $rootScope.cancelarSolicitud = function (notificacion, e) {
        e.preventDefault();
        notificacionesConsultorioService.cancelarSolicitudPaciente(notificacion).then(function (result) {
            if (result.Success)
                $rootScope.NotificacionesConsultorio = result.Data;
        });
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


    $scope.cancelUpdateTrabajoClinica = function () {
        $scope.clinicToSave.Trabajos.splice($scope.indexTrabajoClinicaSelected, 0, angular.copy($scope.trabajoClinicaSelected));
        $scope.trabajoClinicaSelected = null;
        $("#updateTrabajoClinicaId").remove();
    };
    $scope.showUpdateRowTrabajoClinica = function () {
        $scope.consultorioSeleccionado = null;
        $scope.clinicToSave.Trabajos.splice($scope.indexTrabajoClinicaSelected, 1);
        var template = "<tr id = \"updateTrabajoClinicaId\"> <td><input type=\"text\"   class=\"form-control\" id=\"tDescripcionClinicaID\" ng-model=\" trabajoClinicaSelected.Descripcion\"> </td><td><span  ng-click=\"updateTrabajoClinica()\"><i class=\"fa fa-check\"></i></span><span  ng-click=\"cancelUpdateTrabajoClinica()\"><i class=\"fa fa-times\"></i></span></td></tr>";
        var linkFn = $compile(template);
        var content = linkFn($scope);

        $("#tablaTrabajosClinicaId").append(content);
        $("#tDescripcionClinicaID").focus();
    };
    $scope.deleteTrabajoClinica = function () {
        $scope.trabajoClinicaSelected.State = 3;
        $scope.consultorioSeleccionado = null;
        //$scope.clinicToSave.Trabajos.splice($scope.indexTrabajoClinicaSelected, 1);
        $scope.clinicToSave.Trabajos.splice($scope.indexTrabajoClinicaSelected, 0, angular.copy($scope.trabajoClinicaSelected));
        $scope.trabajoClinicaSelected = null;
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

    $scope.showNewRowTrabajoClinica = function () {

        $scope.primerTrabajo = "";
        $scope.consultorioSeleccionado = null;
        $scope.trabajoClinicaSelected = null;
        var template = "<div id = \"newTrabajoClinicaId\" style=\"padding:0px 8px\"><div style=\" float:left; width:80%\"><input type=\"text\"  class=\"form-control\" id=\"tDescripcionClinicaID\" ng-model=\" primerTrabajo\"></div> <div style=\" float:left;    padding: 8px 4px;\" ng-click=\"addTrabajoClinica()\"><i class=\"fa fa-check\"></i></div><div style=\" float:left;    padding: 8px 4px;\" ng-click=\"cancelAddTrabajoClinica()\"><i class=\"fa fa-times\"></i></div></div>";
        var linkFn = $compile(template);
        var content = linkFn($scope);

        $("#tablaTrabajosClinicaId").append(content);
        $("#tDescripcionClinicaID").focus();
    };
    $scope.addTrabajo = function () {
        if ($scope.primerTrabajo.length > 0) {
            $scope.clinicToSave.Trabajos.push({ IDClinica: -1, ID: -1, IDConsultorio: [], Descripcion: angular.copy($scope.primerTrabajo), State: 1 });
        }
    };
    function prepararNuevoTelefonoClinica() {
        $scope.telefonoClinicaTemp = { ID: -1, IDConsultorio: -1, IDClinica: -1, Telefono: "", Nombre: "", State: 1 };
    }
    $scope.cancelAddTelefonoClinica = function () {
        $("#newTelefonoClinicaId").remove();
    };
    $scope.showNewRowTelefonoClinica = function () {
        prepararNuevoTelefonoClinica();
        var template = "<tr id = \"newTelefonoClinicaId\"> <td><input type=\"text\" maxlength=\"100\" class=\"form-control\" id=\"nombreClinicaID\" ng-model=\"telefonoClinicaTemp.Nombre\"> </td><td><input type=\"text\" maxlength=\"10\" class=\"form-control\" id=\"telefonoClinicaID\" ng-model=\" telefonoClinicaTemp.Telefono\"></td><td><span ng-click=\"addNewTelefonoClinica()\"><i class=\"fa fa-check\"></i></span><span  ng-click=\"cancelAddTelefonoClinica()\"><i class=\"fa fa-times\"></i></span></td></tr>";
        var linkFn = $compile(template);
        var content = linkFn($scope);

        $("#tableTelefonosClinicaID").append(content);
        $("#nombreClinicaID").focus();
    };

});