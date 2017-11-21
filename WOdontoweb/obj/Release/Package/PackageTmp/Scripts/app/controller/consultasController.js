app.controller("consultasController", function (consultasService, pacienteService, clinicaService, $scope, $compile, $rootScope, loginService, $location) {
    init();

    function init() {
        $rootScope.mostrarMenu = true;
        $rootScope.sessionDto.IDCitaSeleccionada = "";
        $scope.diasDeSemana = ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"];
        $scope.dateSelected = moment().format('DD/MM/YYYY');
        $scope.dateSelectedString = $scope.diasDeSemana[(ObtenerFechaDesdeStrint($scope.dateSelected).getDay() % 7)] + " " + $scope.dateSelected;
        $scope.citaSeleccionada = null;
        $("#datepicker").datepicker({
            dateFormat: "dd/mm/yy",
            defaultDate: $scope.dateSelected,

            onSelect: function (date) {

                $scope.dateSelected = $.datepicker.formatDate("dd/mm/yy", $(this).datepicker('getDate'));
                var aux = ObtenerFechaDesdeStrint($scope.dateSelected);
                $scope.dateSelectedString = $scope.diasDeSemana[aux.getDay() % 7] + " " + $scope.dateSelected;
                cargarCitasDelDia();
                $scope.$apply();

            }
        });
        if (!$rootScope.sessionDto) {
            loginService.getSessionDto().then(function (result) {
                $rootScope.sessionDto = result;
                cargarConsultorio();
                $("#datepicker").datepicker("setDate", $scope.dateSelected);
            });
        } else {
            cargarConsultorio();
            $("#datepicker").datepicker("setDate", $scope.dateSelected);
        }
    };

    function ObtenerFechaDesdeStrint(dateString) {
        var dateSplit = dateString.split("/");
        return new Date(dateSplit[2], dateSplit[1] - 1, dateSplit[0]);
    }

    function cargarConsultorio() {
        if ($rootScope.consultorioActual == undefined) {
            if ($rootScope.sessionDto.IDConsultorio) {
                clinicaService.getConsultorioByID($rootScope.sessionDto.IDConsultorio).then(function (result) {
                    $rootScope.consultorioActual = result;
                    cargarCitasDelDia();
                });
            }
        } else {
            cargarCitasDelDia();
        }

    }

    function cargarCitasDelDia() {

        if ($rootScope.validarPermisoComponente('dpCalendario')) {
            if ($rootScope.sessionDto.IDConsultorio && $rootScope.consultorioActual.TiempoCita) {
                consultasService.getCitasDelDia($scope.dateSelected, $rootScope.sessionDto.IDConsultorio, $rootScope.consultorioActual.TiempoCita).then(function (result) {
                    $scope.citasDelDia = result;
                    $scope.citaSeleccionada = null;
                });
            }
        }
    }
    $scope.closeModalLicenciaCon = function () {
        $('#modal-licencia-free-con').modal("hide");

    }
    $scope.seleccionaCita = function (cita) {
        $scope.pacienteSeleccionado = null;
        if ($rootScope.sessionDto.Licencia == -1 || $rootScope.sessionDto.Licencia == 0) {
            $('#modal-licencia-free-con').modal("show");
        } else {
            $scope.citaSeleccionada = cita;
            mostrarAdvertenciasEstadoCita();
        }
    };

    function mostrarAdvertenciasEstadoCita() {
        if ($scope.citaSeleccionada.EsTarde)
            toastr.warning("La fecha y hora seleccionada ya no estan diponibles");
        else
            if ($scope.citaSeleccionada.EstaAtendida)
                toastr.warning("La cita seleccionada ya fue atendida");

    }

    $scope.showModalPacientes = function () {
        cargarPacientesEmpresa("#modal-seleccionar-cliente");
    };

    $scope.showModalPacientesCliente = function () {
        getPacientesByCliente("#modal-seleccionar-paciente");
    };

    $scope.showModalCancelarCita = function () {
        $scope.horaLibre = false;
        $scope.motivoCancelacion = "";
        $("#modal-cancelar-cita").modal("show");
    };

    $scope.mostrarHistorico = function () {
        //$scope.pacienteParaAtender = paciente;

        $("#modal-seleccionar-paciente").modal("hide");
        $('.modal-backdrop').remove();
        $('body').removeClass("modal-open");
        $location.path("/historicos");

    };

    function cargarHistoricoPaciente(modalOpen) {
        consultasService.getHistoricoPaciente($scope.pacienteParaAtender.IdPaciente, $rootScope.sessionDto.IDConsultorio).then(function (result) {
            $scope.historicosPaciente = result.select(function (his) {
                his.FechaCreacion = moment(his.FechaCreacion).format('DD/MM/YYYY');
                return his;
            });
            $("#modal-seleccionar-paciente").modal("hide");
            if (modalOpen.length > 0) {
                $(modalOpen).modal('show');
            }
        });
    }

    function cargarPacientesEmpresa(modalOpen) {

        pacienteService.obtenerClientesPorEmpresa($rootScope.sessionDto.IDConsultorio).then(function (result) {
            $scope.pacientesConsultorio = result;
            if (modalOpen.length > 0) {
                $(modalOpen).modal('show');
            }
        });
    }

    $scope.closeModal = function (modal) {
        $("#" + modal).modal("hide");
        $scope.citaSeleccionada = null;
        $scope.pacienteParaAtender = null;
        $scope.historicoPacienteSeleccionado = null;
    };

    $scope.closeModalDetalle = function () {
        $("#modal-detalle-historico").modal('hide');
        $("#modal-historico-paciente").modal('show');
        $scope.historicoPacienteSeleccionado = null;
    };

    $scope.seleccionarPaciente = function (paciente) {
        $scope.pacienteSeleccionado = paciente;
    };
    $scope.seleccionarPacienteParaAtender = function (paciente) {
        $scope.pacienteParaAtender = paciente;
        $rootScope.pacienteSeleccionado = paciente;

        $rootScope.sessionDto.IDPacienteSeleccionado = paciente.IdPaciente;
        $rootScope.sessionDto.IDCitaSeleccionada = $scope.citaSeleccionada.IdCita;
    };
    $scope.mostrarModalNuevoHistorico = function () {


        prepararNuevoHistoricoDto();

        dibujarDientes();
        $("#modal-historico-paciente").modal("hide");
        $("#modal-nuevo-historico").modal("show");
        //var $canvas = $("#myCanvas");
        //var $parent = $canvas.parent();
        //$canvas.width(700);
        //$canvas.height(150);

    };

    $scope.crearNuevoHistorico = function () {
        consultasService.aBMHistorico($scope.historicoNuevo).then(function (result) {
            if (result.Success) {
                toastr.success(result.Message);
                $("#modal-nuevo-historico").modal("hide");
                cargarHistoricoPaciente("#modal-historico-paciente");
            } else {
                toastr.error(result.Message);
            }
        });
    };

    function prepararNuevoHistoricoDto() {
        $scope.historicoNuevo = {
            Id: "",
            IdPaciente: $scope.pacienteParaAtender.IdPaciente,
            IdConsultorio: $rootScope.sessionDto.IDConsultorio,
            FechaCreacion: "",
            FechaNacimiento: "",
            Nombre: $scope.pacienteParaAtender.Nombre,
            Apellido: $scope.pacienteParaAtender.Apellido,
            TipoDocumento: "",
            NumeroDocumento: "",
            LugarNacimiento: "",
            TipoSangre: "",
            Nacionalidad: "",
            Direccion: $scope.pacienteParaAtender.Direccion,
            Odontograma: "",
            Istratamiento: false,
            DescTratamiento: "",
            IsDiabetes: false,
            EnfCardiovascular: false,
            EnfCongénita: false,
            OtraEnfermedad: "",
            AlergiaAnestecia: false,
            AlergiaPenicilina: false,
            AlergiaOtro: "",
            ProblemaOdontologico: -1,
            DescProblemaOdon: ""
        }
        /*
                $scope.historicoNuevo = {
                    IdConsultorio: $rootScope.sessionDto.IDConsultorio,
                    IdPaciente: $scope.pacienteParaAtender.IdPaciente,
                    NumeroHistorico: $scope.historicosPaciente.length + 1,
                    FechaCreacion: "",
                    EstimacionCitas: 0,
                    CitasRealizadas: 0,
                    Estado: true,
                    TituloHistorico: "",
                    EstadoABM: 1,
                    NombrePaciente: $scope.pacienteParaAtender.Nombre,
                    ApellidoPaciente: $scope.pacienteParaAtender.Apellido,
                    Direccion: $scope.pacienteParaAtender.Direccion
                };*/
    }

    $scope.seleccionarHistoricoPaciente = function (historico) {
        $scope.historicoPacienteSeleccionado = angular.copy(historico);
    }
    $scope.mostrarDetalleHistorico = function () {

        $scope.historicoDetalleSeleccionado = angular.copy($scope.historicoPacienteSeleccionado.DetalleHistorico);
        prepararNuevoHistoricoDetalleDto();
        $("#modal-detalle-historico").modal('show');
        $("#modal-historico-paciente").modal('hide');
    };

    $scope.guardarHistoricoDetalle = function () {
        consultasService.insertarHistoricoDetalle($scope.historicoDetalleNuevo).then(function (result) {
            if (result.Success) {
                toastr.success(result.Message);
                cargarCitasDelDia();
                $("#modal-detalle-historico").modal("hide");
                $scope.pacienteSeleccionado = null;
                $scope.citaSeleccionada = null;
            } else {
                toastr.error(result.Message);
            }
        });
    };
    $scope.habilitarHora = function () {
        consultasService.habilitarHora($scope.citaSeleccionada).then(function (result) {
            if (result.Success) {
                toastr.success(result.Message);
                cargarCitasDelDia();

            } else {
                toastr.error(result.Message);
            }
        });
    };
    function prepararNuevoHistoricoDetalleDto() {
        $scope.historicoDetalleNuevo = {
            IdConsultorio: $rootScope.sessionDto.IDConsultorio,
            IdPaciente: $scope.historicoPacienteSeleccionado.IdPaciente,
            NumeroHistorico: $scope.historicoPacienteSeleccionado.NumeroHistorico,
            NumeroDetalle: $scope.historicoDetalleSeleccionado.length + 1,
            FechaCreacion: "",
            TrabajoRealizado: "",
            TrabajoARealizar: "",
            IdCita: $scope.citaSeleccionada.IdCita,
            CerrarHistorico: false
        };
    }

    $scope.agendarCita = function () {
        consultasService.insertarCitaPaciente($scope.citaSeleccionada, $scope.dateSelected, $scope.pacienteSeleccionado.LoginCliente).then(function (result) {
            if (result.Success) {
                toastr.success(result.Message);
                cargarCitasDelDia();
                $("#modal-seleccionar-cliente").modal("hide");
                $scope.pacienteSeleccionado = null;
                $scope.citaSeleccionada = null;
            } else {
                toastr.error(result.Message);
            }
        });
    };

    $scope.eliminarCitaPaciente = function () {
        consultasService.eliminarCitaPaciente($scope.citaSeleccionada, !$scope.horaLibre, $scope.motivoCancelacion).then(function (result) {
            if (result.Success) {
                toastr.success(result.Message);
                cargarCitasDelDia();
                $("#modal-cancelar-cita").modal("hide");
                $scope.pacienteSeleccionado = null;
                $scope.citaSeleccionada = null;
            } else {
                toastr.error(result.Message);
            }
        });
    };

    function getPacientesByCliente(modalOpen) {

        pacienteService.obtenerPacientesPorClienteCita($scope.citaSeleccionada.LoginCliente).then(function (result) {
            $scope.pacientesClienteSeleccionado = result;
            if (modalOpen.length > 0) {
                $(modalOpen).modal('show');
            }
        });
    }


    function coseno(ra) {
        return Math.cos(40) * ra;
    }
    function seno(ra) {
        return Math.sin(40) * ra;
    }
    function dibujarDienteHover(x, y, cos1, cos2, sen1, sen2, diente, canvas) {

    }
    function dibujarDiente(cos1, cos2, sen1, sen2, diente, index, yc, canvas) {
        xc = 18 + 36 * index + diente.Espacio;

        var ctx = canvas.getContext("2d");
        ctx.beginPath();
        ctx.arc(xc, yc, 15, 0, 2 * Math.PI);
        ctx.lineWidth = 1;
        ctx.strokeStyle = '#000000';
        ctx.stroke();
        var ctx2 = canvas.getContext("2d");
        ctx2.beginPath();
        ctx2.arc(xc, yc, 6, 0, 2 * Math.PI);
        ctx2.lineWidth = 1;
        ctx2.strokeStyle = '#000000';
        ctx2.stroke();
        var ctx3 = canvas.getContext("2d");
        ctx3.beginPath();
        ctx3.lineWidth = 1;
        ctx3.moveTo(xc - cos1, yc + sen1);
        ctx3.lineTo(xc - cos2, yc + sen2);
        ctx3.strokeStyle = '#000000';
        ctx3.stroke();
        ctx3.moveTo(xc + cos1, yc + sen1);
        ctx3.lineTo(xc + cos2, yc + sen2);
        ctx3.stroke();
        ctx3.moveTo(xc + cos1, yc - sen1);
        ctx3.lineTo(xc + cos2, yc - sen2);
        ctx3.moveTo(xc - cos1, yc - sen1);
        ctx3.lineTo(xc - cos2, yc - sen2);
        ctx3.stroke();
        diente.PosX = xc;
        diente.PosY = yc;
        ctx3.closePath();
        ctx2.closePath();
        ctx.closePath();
        verificarDefectos(diente.Defectos);
    }
    function dibujarDientes() {
        var c = document.getElementById("myCanvas");

        var xc = 0;
        var yc = 30;
        var cos1 = coseno(6);
        var cos2 = coseno(15);
        var sen1 = seno(6);
        var sen2 = seno(15);
        console.log($scope.dientes);
        angular.forEach($scope.dientes, function (diente, index) {

            angular.forEach(diente, function (dsub, i) {
                console.log(sdub);
                verificarDefectos(dsub.Defectos);
                dibujarDiente(cos1, cos2, sen1, sen2, dsub, i, yc * (index + 1), c);
            })


        });
        c.onmousedown = function (e) {
            var cl = c.getContext("2d");
            var rect = this.getBoundingClientRect(),
              x = e.clientX - rect.left,
              y = e.clientY - rect.top;
            cl.beginPath();
            angular.forEach($scope.dientes, function (d, index) {
                var dx = x - d.PosX;
                var dy = y - d.PosY;
                var isInside = dx * dx + dy * dy <= 15 * 15;
                if (isInside) {
                    d.Defectos.push({ Estado: 2, PosX: x, PosY: y });
                    dibujarDiente(cos1, cos2, sen1, sen2, d, index, d.PosY, c);
                    // dibujarDiente(cos1, cos2, sen1, sen2, d, index, d.PosY, c);
                    cl.arc(x, y, 2, 0, 2 * Math.PI);

                    cl.strokeStyle = '#ff0000';
                    cl.stroke();
                    cl.closePath();
                    return;
                }
            });

        }
        c.onmousemove = function (e) {
            var hover = c.getContext("2d");
            // important: correct mouse position:
            var rect = this.getBoundingClientRect(),
                x = e.clientX - rect.left,
                y = e.clientY - rect.top;

            hover.clearRect(0, 0, c.width, c.height);
            console.log("move");
            angular.forEach($scope.dientes, function (fila, ifila) {
                angular.forEach(fila, function (d, index) {

                    var dx = x - d.PosX;
                    var dy = y - d.PosY;
                    var isInside = dx * dx + dy * dy <= 15 * 15;
                    if (isInside) {
                        dibujarDiente(cos1, cos2, sen1, sen2, d, index, d.PosY, c);
                        hover.beginPath();
                        dibujarDienteHover(d.PosX, d.PosY, 15, cos1, cos2, sen1, sen2, d, c);
                        hover.arc(d.PosX, d.PosY, 15, 0, 2 * Math.PI);
                        hover.lineWidth = 2;
                        hover.strokeStyle = '#000000';
                        hover.stroke();
                        return;
                    } else {
                        dibujarDiente(cos1, cos2, sen1, sen2, d, index, d.PosY, c);
                        //hover.arc(d.PosX, d.PosY, 15, 0, 2 * Math.PI);
                        // hover.lineWidth = 1;
                        //hover.strokeStyle = '#2d2d2d';
                        hover.stroke();
                    }
                    hover.closePath();
                });

            });



        }

    }
    function verificarDefectos(partes) {
        angular.forEach(partes, function (p, index) {
            switch (p.Estado) {

                case 2:
                    dibujarCaries(p.PosX, p.PosY);
                    break;
            }
        });
    };
    function dibujarCaries(posX, posY) {
        var c = document.getElementById("myCanvas");
        var ctx = c.getContext("2d");
        ctx.beginPath();
        ctx.arc(posX, posY, 1, 0, 2 * Math.PI);
        ctx.strokeStyle = '#ff0000';
        ctx.stroke();
        ctx.closePath();
    }
    $scope.testCanvas = function () {
        $scope.dientes = [{ uno: 1 }, { dos: 2 }];
        /* $scope.dientes = [[
             { Espacio: 0, Numero: 18, Estado: 1, Defectos: [] },
              { Espacio: 0, Numero: 17, Estado: 1, Defectos: [] },
               { Espacio: 0, Numero: 16, Estado: 1, Defectos: [] },
                { Espacio: 0, Numero: 15, Estado: 1, Defectos: [] },
     { Espacio: 0, Numero: 14, Estado: 1, Defectos: [] },
     { Espacio: 0, Numero: 13, Estado: 1, Defectos: [] },
     { Espacio: 0, Numero: 12, Estado: 1, Defectos: [] },
     { Espacio: 9, Numero: 11, Estado: 1, Defectos: [] },
     { Espacio: 9, Numero: 21, Estado: 1, Defectos: [] },
      { Espacio: 18, Numero: 22, Estado: 1, Defectos: [] },
              { Espacio: 18, Numero: 23, Estado: 1, Defectos: [] },
               { Espacio: 18, Numero: 24, Estado: 1, Defectos: [] },
                { Espacio: 18, Numero: 25, Estado: 1, Defectos: [] },
     { Espacio: 18, Numero: 14, Estado: 26, Defectos: [] },
     { Espacio: 18, Numero: 13, Estado: 27, Defectos: [] },
     { Espacio: 18, Numero: 12, Estado: 28, Defectos: [] }],
     [
           
                { Espacio: 0, Numero: 15, Estado: 1, Defectos: [] },
     { Espacio: 0, Numero: 14, Estado: 1, Defectos: [] },
     { Espacio: 0, Numero: 13, Estado: 1, Defectos: [] },
     { Espacio: 0, Numero: 12, Estado: 1, Defectos: [] },
     { Espacio: 9, Numero: 11, Estado: 1, Defectos: [] },
     { Espacio: 9, Numero: 21, Estado: 1, Defectos: [] },
                     { Espacio: 18, Numero: 25, Estado: 1, Defectos: [] },
     { Espacio: 18, Numero: 14, Estado: 26, Defectos: [] },
     { Espacio: 18, Numero: 13, Estado: 27, Defectos: [] },
     { Espacio: 18, Numero: 12, Estado: 28, Defectos: [] }]
         ];*/

        dibujarDientes();
    }

});