app.controller("fichaDetalleController", function (loginService, consultasService, comentarioService, $scope, $rootScope, $location, $window) {
    init();

    function init() {
        $rootScope.mostrarMenu = true;
        $scope.esCerrarFicha = false;
        $scope.EstadoDefecto = 0;
        if (!$rootScope.sessionDto) {
            loginService.getSessionDto().then(function (result) {
                $rootScope.sessionDto = result;
                $rootScope.fichaHistoricoNueva = { ID: result.IDHistorico };
                inicializarDatos();
            });
        } else {
            inicializarDatos();
        }


    };
    function dibujarDienteHover(cos1, cos2, sen1, sen2, diente, index, yc, canvas, fila) {
        xc = 21 + 42 * index + diente.Espacio;

        var ctx = canvas.getContext("2d");
        ctx.beginPath();
        ctx.arc(xc, yc, 18, 0, 2 * Math.PI);
        ctx.lineWidth = 2;
        ctx.strokeStyle = '#000000';
        ctx.stroke();
        var ctx2 = canvas.getContext("2d");
        ctx2.beginPath();
        ctx2.arc(xc, yc, 8, 0, 2 * Math.PI);
        ctx2.lineWidth = 2;
        ctx2.strokeStyle = '#000000';
        ctx2.stroke();
        var ctx3 = canvas.getContext("2d");
        ctx3.beginPath();
        ctx3.lineWidth = 2;
        ctx3.moveTo(xc - cos1, yc + sen1);
        ctx3.lineTo(xc - cos2, yc + sen2);
        ctx3.strokeStyle = '#000000';
        ctx3.stroke();

        ctx3.moveTo(xc + cos1, yc + sen1);
        ctx3.lineTo(xc + cos2, yc + sen2);
        ctx3.stroke();

        ctx3.moveTo(xc + cos1, yc - sen1);
        ctx3.lineTo(xc + cos2, yc - sen2);
        //triangulo
        if (fila == 0 || fila == 1) {
            ctx3.moveTo(xc + cos2, yc - sen2);
            ctx3.lineTo(xc, yc - 50);
            ctx3.lineTo(xc - cos2, yc - sen2);

        } else {
            ctx3.moveTo(xc + cos2, yc + sen2);
            ctx3.lineTo(xc, yc + 50);
            ctx3.lineTo(xc - cos2, yc + sen2);

        }



        ctx3.moveTo(xc - cos1, yc - sen1);
        ctx3.lineTo(xc - cos2, yc - sen2);

        ctx3.stroke();
        ctx.fillStyle = "#000000";
        switch (fila) {
            case 0:
                ctx.fillText(diente.Numero, xc - 6, yc - 55);
                break;
            case 1:
                ctx.fillText(diente.Numero, xc - 6, yc + 30);
                break;
            case 2:
                ctx.fillText(diente.Numero, xc - 6, yc - 23);
                break;
            case 3:
                ctx.fillText(diente.Numero, xc - 6, yc + 61);
                break;

        }

        diente.PosX = xc;
        diente.PosY = yc;
        ctx3.closePath();
        ctx2.closePath();
        ctx.closePath();

        //dibujar el triangulo


        verificarDefectos(diente.Defectos, fila);
    }

    function dibujarDiente(cos1, cos2, sen1, sen2, diente, index, yc, canvas, fila) {
        xc = 21 + 42 * index + diente.Espacio;

        var ctx = canvas.getContext("2d");
        ctx.beginPath();
        ctx.arc(xc, yc, 18, 0, 2 * Math.PI);
        ctx.lineWidth = 1;
        ctx.strokeStyle = '#000000';
        ctx.stroke();
        //dibujar cuadrado
        switch (fila) {
            case 0:
                ctx.moveTo(xc - 21, yc - 140);
                ctx.lineTo(xc + 21, yc - 140);
                ctx.lineTo(xc + 21, yc - 110);
                ctx.lineTo(xc - 21, yc - 110);
                if (index == 0)
                    ctx.lineTo(xc - 21, yc - 140);
                ctx.stroke();
                break;
            case 1:
                //  ctx.moveTo(xc - 21, yc - 200);
                ctx.moveTo(xc + 21, yc - 200);
                ctx.lineTo(xc + 21, yc - 170);
                ctx.lineTo(xc - 21, yc - 170);
                if (index == 0)
                    ctx.lineTo(xc - 21, yc - 200);
                ctx.stroke();
                break
            case 2:
                ctx.moveTo(xc - 21, canvas.height - 40);
                ctx.lineTo(xc - 21, canvas.height - 70);
                ctx.lineTo(xc + 21, canvas.height - 70);
                if (index == 9)
                    ctx.lineTo(xc + 21, canvas.height - 40);
                //if (index == 0)
                //    ctx.lineTo(xc - 21, canvas.height - 70);
                ctx.stroke();
                break;
            case 3:
                ctx.moveTo(xc - 21, canvas.height - 40);
                ctx.lineTo(xc + 21, canvas.height - 40);
                ctx.lineTo(xc + 21, canvas.height - 10);
                ctx.lineTo(xc - 21, canvas.height - 10);
                if (index == 0)
                    ctx.lineTo(xc - 21, canvas.height - 40);
                ctx.stroke();
                break;
        }

        var ctx2 = canvas.getContext("2d");
        ctx2.beginPath();
        ctx2.arc(xc, yc, 8, 0, 2 * Math.PI);
        ctx2.lineWidth = 1;
        ctx2.strokeStyle = '#000000';
        ctx2.stroke();
        var ctx3 = canvas.getContext("2d");
        ctx3.beginPath();
        ctx3.lineWidth = 1;
        /// linea 3
        ctx3.moveTo(xc - cos1, yc + sen1);
        ctx3.lineTo(xc - cos2, yc + sen2);
        ctx3.strokeStyle = '#000000';
        ctx3.stroke();
        //linea 4
        ctx3.moveTo(xc + cos1, yc + sen1);
        ctx3.lineTo(xc + cos2, yc + sen2);
        ctx3.stroke();
        //linea 1
        ctx3.moveTo(xc + cos1, yc - sen1);
        ctx3.lineTo(xc + cos2, yc - sen2);
        //triangulo
        if (fila == 0 || fila == 1) {
            ctx3.moveTo(xc + cos2, yc - sen2);
            ctx3.lineTo(xc, yc - 50);
            ctx3.lineTo(xc - cos2, yc - sen2);

        } else {
            ctx3.moveTo(xc + cos2, yc + sen2);
            ctx3.lineTo(xc, yc + 50);
            ctx3.lineTo(xc - cos2, yc + sen2);

        }


        //linea 2
        ctx3.moveTo(xc - cos1, yc - sen1);
        ctx3.lineTo(xc - cos2, yc - sen2);
        ctx3.stroke();
        ctx.fillStyle = "#000000";
        switch (fila) {
            case 0:
                ctx.fillText(diente.Numero, xc - 6, yc - 55);
                break;
            case 1:
                ctx.fillText(diente.Numero, xc - 6, yc + 30);
                break;
            case 2:
                ctx.fillText(diente.Numero, xc - 6, yc - 23);
                break;
            case 3:
                ctx.fillText(diente.Numero, xc - 6, yc + 61);
                break;

        }

        diente.PosX = xc;
        diente.PosY = yc;
        ctx3.closePath();
        ctx2.closePath();
        ctx.closePath();

        //dibujar el triangulo


        verificarDefectos(diente.Defectos, fila);
    }
    function coseno(ra) {
        return Math.cos(40) * ra;
    }
    function seno(ra) {
        return Math.sin(40) * ra;
    }
    function verificarDefectos(partes, fila) {
        angular.forEach(partes, function (p, index) {
            switch (p.Estado) {

                case 1:
                    dibujarCaries(p.PosX, p.PosY, p.PartePieza);
                    break;
                case 2:
                    dibujarCoronaTemp(p.PosX, p.PosY);
                    break;
                case 3:
                    dibujarAusente(p.PosX, p.PosY, fila);
                    break;
                case 4:
                    dibujarCoronaFija(p.PosX, p.PosY);
                    break;
            }
        });
    };
    function dibujarAusente(x, y, fila) {
        var c = document.getElementById("myCanvas");
        var ctx = c.getContext("2d");
        y = y - 15;
        ctx.beginPath();
        if (fila == 1 || fila == 0) {
            ctx.moveTo(x - 20, y - 32);
            ctx.lineTo(x + 20, y + 32);

            ctx.moveTo(x + 20, y - 32);
            ctx.lineTo(x - 20, y + 32);
        } else {

            ctx.moveTo(x - 20, (y + 30) - 32);
            ctx.lineTo(x + 20, (y + 30) + 32);

            ctx.moveTo(x + 20, (y + 30) - 32);
            ctx.lineTo(x - 20, (y + 30) + 32);
        }

        ctx.strokeStyle = '#0000FF';
        ctx.stroke();
    }
    function dibujarCaries(posX, posY, partePieza) {
        var c = document.getElementById("myCanvas");
        var ctx = c.getContext("2d");
        ctx.beginPath();
        switch (partePieza) {
            case 1:
                ctx.arc(posX, posY, 7, 0, 2 * Math.PI);
                ctx.fillStyle = '#ff0000';
                ctx.strokeStyle = '#ff0000';

                ctx.fill();
                ctx.stroke();
                break;
            case 2:
                var sangulo = 134 * (Math.PI / 180);
                var eangulo = 226 * (Math.PI / 180);
                dibujarCariesRellena(ctx, posX, posY, sangulo, eangulo)
                break;
            case 3:
                var sangulo = 231 * (Math.PI / 180);
                var eangulo = 310 * (Math.PI / 180);
                dibujarCariesRellena(ctx, posX, posY, sangulo, eangulo)
                break;
            case 4:
                var sangulo = 315 * (Math.PI / 180);
                var eangulo = 45 * (Math.PI / 180);
                dibujarCariesRellena(ctx, posX, posY, sangulo, eangulo)
                break;
            case 5:
                var sangulo = 50 * (Math.PI / 180);
                var eangulo = 130 * (Math.PI / 180);
                dibujarCariesRellena(ctx, posX, posY, sangulo, eangulo);
                break;


        }


        //ctx.closePath();
    };
    function dibujarCariesRellena(ctx, posX, posY, sangulo, eangulo) {
        ctx.strokeStyle = '#ff0000';
        //  ctx.arc(posX, posY, 18, sangulo, eangulo);
        ctx.beginPath();
        ctx.arc(posX, posY, 17, sangulo, eangulo);
        ctx.stroke();
        ctx.beginPath();
        ctx.arc(posX, posY, 16, sangulo, eangulo);
        ctx.stroke();
        ctx.beginPath();
        ctx.arc(posX, posY, 15, sangulo, eangulo);
        ctx.stroke();
        ctx.beginPath();
        ctx.arc(posX, posY, 14, sangulo, eangulo);
        ctx.stroke();
        ctx.beginPath();
        ctx.arc(posX, posY, 13, sangulo, eangulo);
        ctx.stroke();
        ctx.beginPath();
        ctx.arc(posX, posY, 12, sangulo, eangulo);
        ctx.stroke();
        ctx.beginPath();
        ctx.arc(posX, posY, 11, sangulo, eangulo);
        ctx.stroke();
        ctx.beginPath();
        ctx.arc(posX, posY, 10, sangulo, eangulo);
        ctx.stroke();
        ctx.beginPath();
        ctx.arc(posX, posY, 9, sangulo, eangulo);
        ctx.stroke();

    }
    function dibujarCoronaTemp(posX, posY) {
        var c = document.getElementById("myCanvas");
        var ctx = c.getContext("2d");
        ctx.beginPath();
        ctx.arc(posX, posY, 19, 0, 2 * Math.PI);
        ctx.strokeStyle = '#ff0000';
        ctx.stroke();
        ctx.closePath();
    };
    function dibujarCoronaFija(posX, posY) {
        var c = document.getElementById("myCanvas");
        var ctx = c.getContext("2d");
        ctx.beginPath();
        ctx.arc(posX, posY, 19, 0, 2 * Math.PI);
        ctx.strokeStyle = '#0000FF';
        ctx.stroke();
        ctx.closePath();
    };
    function executeResizeWithTime(time) {

        setTimeout(function () {

            var canvas = document.getElementById('myCanvas'),
           context = canvas.getContext('2d');

            // resize the canvas to fill browser window dynamically
            window.addEventListener('resize', resizeCanvas, false);

            function resizeCanvas() {
                var withtable = $("#tbtratamiento").width();
                canvas.width = withtable;
                canvas.height = 550;

                /**
                 * Your drawings need to be inside this function otherwise they will be reset when 
                 * you resize the browser window and the canvas goes will be cleared.
                 */
                dibujarDientes();
            }

        }, time);
    }
    $(document).on('onResize', function (e, args) {
        executeResizeWithTime(300);
    });

    angular.element($window).bind('resize', function () {
        executeResizeWithTime(300);
    });
    $(window).resize(function () {
        setTimeout(function () {

            dibujarDientes();
            $scope.$apply();
        }, 300);

    });
    $scope.limpiarOdontograma = function () {
        $scope.dientes = JSON.parse('[[{"Espacio":50,"Numero":18,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":17,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":16,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":15,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":14,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":13,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":12,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":11,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":21,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":22,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":23,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":24,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":25,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":26,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":27,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":28,"Estado":1,"Defectos":[]}],[{"Espacio":176,"Numero":55,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":54,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":53,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":52,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":51,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":61,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":62,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":63,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":64,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":65,"Estado":1,"Defectos":[]}],[{"Espacio":176,"Numero":85,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":84,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":83,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":82,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":81,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":71,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":72,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":73,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":74,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":75,"Estado":1,"Defectos":[]}],[{"Espacio":50,"Numero":48,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":47,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":46,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":45,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":44,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":43,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":42,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":41,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":31,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":32,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":33,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":34,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":35,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":36,"Estado":26,"Defectos":[]},{"Espacio":50,"Numero":37,"Estado":27,"Defectos":[]},{"Espacio":50,"Numero":38,"Estado":28,"Defectos":[]}]]');
        dibujarDientes();
    }
    function dibujarCuadrante(diametro, canvas) {
        var ctx = canvas.getContext("2d");
        ctx.beginPath();
        ctx.lineWidth = 2;
        ctx.strokeStyle = '#000000';
        ctx.moveTo(((diametro + 6) * 9) + 8, 29);
        ctx.lineTo(((diametro + 6) * 9) + 8, canvas.height - 10);
        ctx.lineWidth = 1;
        ctx.strokeStyle = '#000000';
        ctx.moveTo(((diametro + 6) * 3) + 20, 293);
        ctx.lineTo((diametro + 6) * 15, 293);
        ctx.stroke();

    }
    function dibujarDientes() {
        // var canvas = document.getElementById("myCanvas");
        var withtable = $("#tbtratamiento").width();
        var c = document.getElementById("myCanvas");
        var h = c.getContext("2d");
        h.clearRect(0, 0, c.width, c.height);
        //  
        c.width = 790;
        c.height = 550;
        //$("#myCanvas").css('max-height', withtable + 'px');
        var xc = 0;
        var yc = 90;
        var cos1 = coseno(8);
        var cos2 = coseno(18);
        var sen1 = seno(8);
        var sen2 = seno(18);
        dibujarCuadrante(36, c);
        angular.forEach($scope.dientes, function (diente, index) {
            if (index == 2)
                yc = yc - 8;
            angular.forEach(diente, function (dsub, i) {

                verificarDefectos(dsub.Defectos, index);
                dibujarDiente(cos1, cos2, sen1, sen2, dsub, i, (yc * (index + 1)) + 80, c, index);
            })


        });
        c.onmousedown = function (e) {
            var cl = c.getContext("2d");
            var rect = this.getBoundingClientRect(),
              x = e.clientX - rect.left,
              y = e.clientY - rect.top;
            // cl.clearRect(0, 0, c.width, c.height);
            cl.beginPath();
            angular.forEach($scope.dientes, function (fila, ifila) {
                angular.forEach(fila, function (d, index) {
                    var dx = x - d.PosX;
                    var dy = y - d.PosY;


                    var isInside = dx * dx + dy * dy <= 18 * 18;
                    if (isInside) {

                        switch (parseInt($scope.EstadoDefecto)) {
                            case 1:
                                var partePieza = 0;
                                if (dx * dx + dy * dy <= 8 * 8)
                                    partePieza = 1;
                                else
                                    if (dx * dx + dy * dy >= 8 * 8 && dx * dx + dy * dy <= 18 * 18)// esta afuera de circulo pequeno
                                    {
                                        var ca = (x - d.PosX);
                                        var co = (y - d.PosY);
                                        var h = Math.sqrt((ca * ca) + (co * co));
                                        var angulo = (Math.acos(Math.abs(ca) / h)) / (Math.PI / 180);
                                        if (ca < 0 && co < 0)//2 o 3
                                        {
                                            if (angulo <= 40)
                                                partePieza = 2;
                                            else
                                                partePieza = 3;
                                        } else {
                                            if (ca > 0 && co < 0)//3 o 4
                                            {
                                                if (angulo <= 40)
                                                    partePieza = 4;
                                                else
                                                    partePieza = 3;
                                            } else {
                                                if (ca > 0 && co > 0) {
                                                    if (angulo <= 40)
                                                        partePieza = 4;
                                                    else
                                                        partePieza = 5;
                                                } else
                                                    if (ca < 0 && co > 0) {

                                                        if (angulo <= 40)
                                                            partePieza = 2;
                                                        else
                                                            partePieza = 5;
                                                    }

                                            }
                                        }



                                    }
                                //   partePieza = 2;
                                d.Defectos.push({ Estado: parseInt($scope.EstadoDefecto), PosX: d.PosX, PosY: d.PosY, PartePieza: partePieza });

                                break;
                            case 2:
                                d.Defectos.push({ Estado: parseInt($scope.EstadoDefecto), PosX: d.PosX, PosY: d.PosY });

                                break;
                            case 3:
                                d.Defectos.push({ Estado: parseInt($scope.EstadoDefecto), PosX: d.PosX, PosY: d.PosY });

                                break;
                            case 4:
                                d.Defectos.push({ Estado: parseInt($scope.EstadoDefecto), PosX: d.PosX, PosY: d.PosY });

                                break;
                            case 5:
                                d.Defectos = [];

                                break;
                        }
                        dibujarDiente(cos1, cos2, sen1, sen2, d, index, d.PosY, c, ifila);
                        ///   cl.strokeStyle = '#ff0000';
                        cl.stroke();
                        cl.closePath();
                        return;
                    }
                });
            });

        }
        c.onmousemove = function (e) {
            var hover = c.getContext("2d");
            // important: correct mouse position:
            var rect = this.getBoundingClientRect(),
                x = e.clientX - rect.left,
                y = e.clientY - rect.top;

            hover.clearRect(0, 0, c.width, c.height);
            dibujarCuadrante(36, c);
            angular.forEach($scope.dientes, function (fila, ifila) {
                angular.forEach(fila, function (d, index) {

                    var dx = x - d.PosX;
                    var dy = y - d.PosY;
                    var isInside = dx * dx + dy * dy <= 18 * 18;
                    if (isInside) {
                        dibujarDiente(cos1, cos2, sen1, sen2, d, index, d.PosY, c, ifila);
                        // hover.beginPath();
                        dibujarDienteHover(cos1, cos2, sen1, sen2, d, index, d.PosY, c, ifila);
                        //hover.arc(d.PosX, d.PosY, 18, 0, 2 * Math.PI);
                        //hover.lineWidth = 2;
                        //hover.strokeStyle = '#000000';
                        //hover.stroke();
                        $('#myCanvas').css('cursor', 'pointer')
                        return;
                    } else {

                        dibujarDiente(cos1, cos2, sen1, sen2, d, index, d.PosY, c, ifila);
                        //hover.arc(d.PosX, d.PosY, 15, 0, 2 * Math.PI);
                        // hover.lineWidth = 1;
                        //hover.strokeStyle = '#2d2d2d';
                        hover.stroke();
                        $('#myCanvas').css('cursor', 'auto')
                    }
                    hover.closePath();
                });

            });



        }
    }

    function calcularAngulo2(xc, x, yc, y) {
        var absXcX = Math.abs(x - xc);
        var angulo = Math.acos(absXcX);
        return angulo;

    }

    $scope.validarOpcionesImprimir = function () {
        return ($scope.imprimirPdf && $scope.imprimirPdf == "-1")
    }
    $scope.exportarFicha = function () {
        switch ($scope.imprimirPdf) {

            case "0":
                fichaCompleta();
                break;
            case "1":
                tratamientosTrabajos();
                break;
        }
        $('#modal-seleccionar-pdf').modal("hide");
    }

    function tratamientosTrabajos() {

        var doc = new jsPDF();
        doc.setFontSize(22);
        doc.text(40, 20, 'Tratamientos y trabajos ' + $scope.historicoFichaCab.Titulo);
        doc.setFontSize(12);
        doc.text(10, 33, 'Tratamientos');

        doc.setFontSize(9);
        doc.line(20, 38, 142, 38);
         //doc.line(20, 45, 142, 45);
        doc.line(20, 38, 20, 45);
        doc.line(142, 38, 142, 45);
        doc.line(37, 38, 37, 45);
        doc.line(120, 38, 120, 45);
         doc.setFontType("bold");
        doc.text(25, 43, 'Pieza');
        doc.text(72, 43, 'Descripcion');
        doc.text(127, 43, 'Costo');
        doc.setFontSize(8);
        doc.setFontType("normal");
        for (var i = 0; i < $scope.historicoFichaCab.FichaTrabajo.length; i++) {
            doc.text(28, 49 + (i * 6), $scope.historicoFichaCab.FichaTrabajo[i].Pieza.toString());
            doc.text(42, 49 + (i * 6), $scope.historicoFichaCab.FichaTrabajo[i].Descripcion);
            doc.text(122, 49 + (i * 6), $scope.historicoFichaCab.FichaTrabajo[i].Costo.toString());
            doc.line(20, 45 + (i * 6), 20, 45 + ((i + 1) * 6));
            doc.line(37, 45 + (i * 6), 37, 45 + ((i + 1) * 6));
            doc.line(120, 45 + (i * 6), 120, 45 + ((i + 1) * 6));
            doc.line(142, 45 + (i * 6), 142, 45 + ((i + 1) * 6));
           doc.line(20, 45 + (i  * 6), 142, 45 + (i  * 6));
        }
        doc.line(20, 45 + ($scope.historicoFichaCab.FichaTrabajo.length * 6), 142, 45 + ($scope.historicoFichaCab.FichaTrabajo.length * 6));
        var y = 45 + ($scope.historicoFichaCab.FichaTrabajo.length * 6);
        doc.setFontSize(12);
        doc.text(10, y + 10, 'Trabajos realizados');

        doc.setFontSize(9);
        doc.line(20, y +17, 142, y+17);
        //doc.line(20, 45, 142, 45);
        doc.line(20, y + 17, 20, y + 24);
        doc.line(142, y + 17, 142, y + 24);
        doc.line(45, y + 17, 45, y + 24);
        doc.line(65, y + 17, 65, y + 24);
        doc.setFontType("bold");
        doc.text(29, y+22, 'Fecha');
        doc.text(50, y + 22, 'Pieza');
        doc.text(70, y + 22, 'Trabajo');
        y = y + 30;
        doc.setFontSize(8);
        doc.setFontType("normal");
        for (var i = 0; i < $scope.historicoFichaCab.FichaDetalle.length; i++) {
            doc.text(28, y + (i * 6), $scope.historicoFichaCab.FichaDetalle[i].Fecha.toString());
            doc.text(42, y + (i * 6), $scope.historicoFichaCab.FichaDetalle[i].Pieza.toString());
            doc.text(122, y + (i * 6), $scope.historicoFichaCab.FichaDetalle[i].TrabajoRealizado.toString());
            doc.line(20, (y - 6) + (i * 6), 20, (y - 6) + ((i + 1) * 6));
            doc.line(45, (y - 6) + (i * 6), 45, (y - 6) + ((i + 1) * 6));
            doc.line(65, (y - 6) + (i * 6), 65, (y - 6) + ((i + 1) * 6));
            doc.line(142, (y - 6) + (i * 6), 142, (y - 6) + ((i + 1) * 6));
            doc.line(20, (y - 6) + (i * 6), 142, (y - 6) + (i * 6));
        }
        doc.line(20, (y - 6) + ($scope.historicoFichaCab.FichaDetalle.length * 6), 142, (y - 6) + ($scope.historicoFichaCab.FichaDetalle.length * 6));

        doc.save('FichaOdontologica' + $scope.historicoFichaCab.Titulo + '.pdf');
    }
    function fichaCompleta() {


        var doc = new jsPDF();
        doc.setFontSize(22);
        doc.text(40, 20, 'Historia clinica odontologica ' + $scope.historicoFichaCab.Titulo);
        doc.setFontSize(7);
        //doc.setFontType("bold");
        doc.line(10, 30, 55, 30);
        doc.line(60, 30, 105, 30);
        doc.line(110, 30, 160, 30);
        doc.line(165, 30, 180, 30);
        doc.line(185, 30, 200, 30);

        doc.text(20, 34, 'Apellido Paterno ');
        doc.text(70, 34, 'Apellido Materno ');
        doc.text(129, 34, 'Nombres ');
        doc.text(169, 34, 'Edad ');
        doc.text(189, 34, 'Sexo ');
        //2
        doc.line(10, 40, 55, 40);
        doc.line(60, 40, 105, 40);
        doc.line(110, 40, 160, 40);
        doc.line(165, 40, 200, 40);
        doc.text(14, 44, 'Lugar y Fecha de nacimiento ');
        doc.text(75, 44, 'Ocupacion ');
        doc.text(129, 44, 'Direccion ');
        doc.text(169, 44, 'Telefono y/o celular ');

        //3
        doc.line(10, 50, 55, 50);
        doc.line(60, 50, 105, 50);
        doc.line(110, 50, 160, 50);
        doc.line(165, 50, 200, 50);
        doc.text(19, 54, 'Grado de Instruccion ');
        doc.text(74, 54, 'Estado Civl ');
        doc.text(121, 54, 'Naciones Originarias ');
        doc.text(172, 54, 'Idioma o Dialecto ');
        //cuadro
        doc.line(7, 24, 203, 24);
        doc.line(203, 24, 203, 56);
        doc.line(7, 56, 203, 56);
        doc.line(7, 24, 7, 56);
        //4
        doc.text(10, 60, 'Persona que brinda la informacion');
        doc.line(50, 60, 150, 60);
        //6
        doc.line(10, 66, 45, 66);
        doc.line(50, 66, 85, 66);
        doc.line(90, 66, 125, 66);
        doc.line(130, 66, 172, 66);
        doc.line(177, 66, 200, 66);

        doc.text(18, 70, 'Apellido Paterno ');
        doc.text(58, 70, 'Apellido Materno ');
        doc.text(105, 70, 'Nombres ');
        doc.text(147, 70, 'Direccion ');
        doc.text(178, 70, 'Telefono y/o celular ');
        //7
        doc.setFontType("bold");
        doc.text(10, 76, 'Antecedentes Patologicos Familiares: ');

        //8
        doc.text(10, 82, 'Antecedentes Patologicos Personales: ');
        doc.setFontType("normal");
        //9
        doc.text(15, 87, 'Anemia');
        if ($scope.historicoNuevo.Anemia)
            doc.text(30, 87, '( X )');
        else
            doc.text(30, 87, '(   )');
        doc.text(56, 87, 'Cardiopatias');
        if ($scope.historicoNuevo.Cardiopatias)
            doc.text(76, 87, '( X )');
        else
            doc.text(76, 87, '(   )');
        doc.text(96, 87, 'Enf. Gastricas');
        if ($scope.historicoNuevo.EnfGastricas)
            doc.text(116, 87, '( X )');
        else
            doc.text(116, 87, '(   )');
        doc.text(136, 87, 'Hepatitis');
        if ($scope.historicoNuevo.Hepatitis)
            doc.text(156, 87, '( X )');
        else
            doc.text(156, 87, '(   )');
        doc.text(176, 87, 'Tuberculosis');
        if ($scope.historicoNuevo.Tuberculosis)
            doc.text(196, 87, '( X )');
        else
            doc.text(196, 87, '(   )');

        //10
        doc.text(15, 93, 'Asma');
        if ($scope.historicoNuevo.Asma)
            doc.text(30, 93, '( X )');
        else
            doc.text(30, 93, '(   )');

        doc.text(56, 93, 'Diabetes Mel.');
        if ($scope.historicoNuevo.DiabetesMel)
            doc.text(76, 93, '( X )');
        else
            doc.text(76, 93, '(   )');
        doc.text(96, 93, 'Epilepsia');
        if ($scope.historicoNuevo.Epilepsia)
            doc.text(116, 93, '( X )');
        else
            doc.text(116, 93, '(   )');

        doc.text(136, 93, 'Hipertension');
        if ($scope.historicoNuevo.Hipertension)
            doc.text(156, 93, '( X )');
        else
            doc.text(156, 93, '(   )');

        doc.text(176, 93, 'VIH');
        if ($scope.historicoNuevo.VIH)
            doc.text(196, 93, '( X )');
        else
            doc.text(196, 93, '(   )');
        //11
        doc.text(10, 99, 'Otros: ');
        doc.text(75, 99, 'Alergias: ');
        if ($scope.historicoNuevo.Alergias == null) {
            doc.text(90, 99, 'SI  (   )');
            doc.text(110, 99, 'NO  (   )');
        } else
            if ($scope.historicoNuevo.Alergias) {
                doc.text(90, 99, 'SI  ( X )');
                doc.text(110, 99, 'NO  (   )');
            }

            else {
                doc.text(90, 99, 'SI  (   )');
                doc.text(110, 99, 'NO  ( X )');
            }
        doc.text(145, 99, 'Embarazo: ');
        if ($scope.historicoNuevo.Embarazo == null) {
            doc.text(160, 99, 'SI  (   )');
            doc.text(170, 99, '___ Semanas');
            doc.text(190, 99, 'NO  (   )');
        } else
            if ($scope.historicoNuevo.Embarazo == 0) {
                doc.text(160, 99, 'SI  (   )');
                doc.text(170, 99, '___ Semanas');
                doc.text(190, 99, 'NO  ( X )');
            } else {
                doc.text(160, 99, 'SI  ( X )');
                doc.text(170, 99, $scope.historicoNuevo.Embarazo + '  Semanas');
                doc.text(190, 99, 'NO  (   )');
            }

        //cuadro 2
        doc.line(7, 72, 203, 72);
        doc.line(203, 72, 203, 101);
        doc.line(7, 101, 203, 101);
        doc.line(7, 72, 7, 101);
        doc.line(7, 95, 203, 95);
        doc.line(7, 78, 203, 78);
        doc.line(73, 95, 73, 101);
        doc.line(143, 95, 143, 101);
        //12
        doc.text(10, 106, 'Esta en tratamiento medico?');
        doc.line(42, 106, 95, 106);
        doc.text(98, 106, 'Actualmente recibe algun medicamento?');
        doc.line(143, 106, 200, 106);

        //13
        doc.text(10, 113, 'Tuvo hemorragia despues de una extraccion dental?');
        if ($scope.historicoNuevo.HemorragiaExtDent == null) {
            doc.text(75, 113, 'SI  (   )');
            doc.text(85, 113, 'Especifique :');
            doc.text(100, 113, 'Inmediata - ');
            doc.text(115, 113, 'Mediana ');
            doc.text(150, 113, 'NO  (   )');

        } else {

            switch ($scope.historicoNuevo.HemorragiaExtDent.toString()) {
                case "0":
                    doc.text(75, 113, 'SI  (   )');
                    doc.text(85, 113, 'Especifique :');
                    doc.text(104, 113, 'Inmediata');
                    doc.text(118, 113, '-');
                    doc.text(122, 113, 'Mediana ');
                    doc.text(150, 113, 'NO  ( X )');

                    break;
                case "1":
                    doc.text(75, 113, 'SI  ( X )');
                    doc.text(85, 113, 'Especifique :');
                    doc.text(104, 113, 'Inmediata');
                    doc.text(118, 113, '-');
                    doc.text(122, 113, 'Mediana ');
                    doc.text(150, 113, 'NO  (   )');
                    doc.ellipse(109, 112, 8, 2);
                    break;
                case "2":
                    doc.text(75, 113, 'SI  ( X )');
                    doc.text(85, 113, 'Especifique :');
                    doc.text(104, 113, 'Inmediata');
                    doc.text(118, 113, '-');
                    doc.text(122, 113, 'Mediana ');
                    doc.text(150, 113, 'NO  (   )');
                    doc.ellipse(127, 112, 7, 2);
                    break;
            }


        }

        // cuadro
        doc.line(7, 109, 203, 109);
        doc.line(203, 109, 203, 115);
        doc.line(7, 115, 203, 115);
        doc.line(7, 109, 7, 115);
        //13
        doc.setFontType("bold");
        doc.text(20, 120, 'EXAMEN EXTRA ORAL');
        doc.text(120, 120, 'EXAMEN INTRA ORAL');
        doc.setFontType("normal");
        //14
        doc.text(10, 125, 'ATM: ');
        doc.text(102, 125, 'Labios: ');
        //15
        doc.text(10, 131, 'Ganglios Linfaticos: ');
        doc.text(102, 131, 'Lengua: ');
        //16

        doc.text(10, 137, 'Respirador:');
        if ($scope.historicoNuevo.Respirador == null) {
            doc.text(25, 137, 'Nasal (   )');
            doc.text(35, 137, 'Bucal (   )');
            doc.text(47, 137, 'Buconasal (   )');
        } else {
            switch ($scope.historicoNuevo.Respirador.toString()) {
                case "2":
                    doc.text(25, 137, 'Nasal (   )');
                    doc.text(41, 137, 'Bucal ( X )');
                    doc.text(58, 137, 'Buconasal (   )');
                    break;
                case "1":
                    doc.text(25, 137, 'Nasal ( X )');
                    doc.text(41, 137, 'Bucal (   )');
                    doc.text(58, 137, 'Buconasal (   )');
                    break;
                case "3":
                    doc.text(25, 137, 'Nasal (   )');
                    doc.text(41, 137, 'Bucal (   )');
                    doc.text(58, 137, 'Buconasal ( X )');
                    break;
            }
        }
        doc.text(102, 137, 'Paladar');
        //17
        doc.text(10, 143, 'Otros: ');
        doc.text(102, 143, 'Piso de la Boca: ');
        //18
        doc.setFontType("bold");
        doc.text(20, 149, 'ANTECENDENTES BUCODENTALES');
        doc.setFontType("normal");
        doc.text(102, 149, 'Mucosa Yugal: ');
        //18
        doc.text(10, 155, 'Fecha de la ultima visita al odontologo: ');
        doc.text(102, 155, 'Encias: ');
        //19
        doc.text(10, 161, 'Habitos:');
        if ($scope.historicoNuevo.isFuma == null || !$scope.historicoNuevo.isFuma) {
            doc.text(21, 161, 'Fuma (   )');

        } else {
            doc.text(21, 161, 'Fuma ( X )');
        }
        if ($scope.historicoNuevo.isBebe == null || !$scope.historicoNuevo.isBebe) {
            doc.text(35, 161, 'Bebe (   )');

        } else {
            doc.text(35, 161, 'Bebe ( X )');
        }
        doc.text(50, 161, 'Otros ');

        doc.text(102, 161, 'Utiliza protesis dental? ');
        if ($scope.historicoNuevo.isProtesisDental == null) {
            doc.text(130, 161, 'SI (   )');
            doc.text(142, 161, 'NO (   )');
        } else
            if ($scope.historicoNuevo.isProtesisDental) {
                doc.text(130, 161, 'SI ( X )');
                doc.text(142, 161, 'NO (   )');
            } else {
                doc.text(130, 161, 'SI (   )');
                doc.text(142, 161, 'NO ( X )');
            }

        //cuadro
        doc.line(7, 121, 203, 121);
        doc.line(7, 127, 203, 127);
        doc.line(7, 133, 203, 133);
        doc.line(7, 139, 203, 139);
        doc.line(7, 145, 203, 145);
        doc.line(7, 151, 203, 151);
        doc.line(7, 157, 203, 157);
        doc.line(7, 163, 203, 163);
        doc.line(7, 121, 7, 145);
        doc.line(98, 121, 98, 163);
        doc.line(203, 121, 203, 145);
        doc.line(7, 151, 7, 163);
        doc.line(203, 151, 203, 163);
        //
        doc.setFontType("bold");
        doc.text(20, 168, 'ANTECEDENTES HIGIENE ORAL ');
        doc.setFontType("normal");
        //
        doc.text(10, 174, 'Utiliza cepillo dental? ');
        if ($scope.historicoNuevo.isCepilloDental == null) {
            doc.text(38, 174, 'SI (   ) ');
            doc.text(51, 174, 'NO (   ) ');
        } else
            if ($scope.historicoNuevo.isCepilloDental) {
                doc.text(38, 174, 'SI ( X ) ');
                doc.text(51, 174, 'NO (   ) ');
            } else {
                doc.text(38, 174, 'SI (   ) ');
                doc.text(51, 174, 'NO ( X ) ');
            }
        doc.text(72, 174, 'Utiliza hilo dental? ');
        if ($scope.historicoNuevo.isHiloDental == null) {
            doc.text(96, 174, 'SI (   ) ');
            doc.text(109, 174, 'NO (   ) ');
        } else
            if ($scope.historicoNuevo.isHiloDental) {
                doc.text(96, 174, 'SI ( X ) ');
                doc.text(109, 174, 'NO (   ) ');
            } else {
                doc.text(96, 174, 'SI (   ) ');
                doc.text(109, 174, 'NO ( X ) ');
            }

        doc.text(132, 174, 'Utiliza enjuague bucal? ');
        if ($scope.historicoNuevo.isEnjuagueBucal == null) {
            doc.text(161, 174, 'SI (   ) ');
            doc.text(176, 174, 'NO (   ) ');
        } else
            if ($scope.historicoNuevo.isEnjuagueBucal) {
                doc.text(161, 174, 'SI ( X ) ');
                doc.text(176, 174, 'NO (   ) ');
            } else {
                doc.text(161, 174, 'SI (   ) ');
                doc.text(176, 174, 'NO ( X ) ');
            }
        //

        doc.text(10, 180, 'Frecuencia del cepillo dental: ');
        doc.line(45, 180, 110, 180);
        doc.text(115, 180, 'Durante el cepillado dental le sangra las encias? ');
        if ($scope.historicoNuevo.isSangreEncias == null) {
            doc.text(172, 180, 'SI (   ) ');
            doc.text(188, 180, 'NO (   ) ');
        } else
            if ($scope.historicoNuevo.isSangreEncias) {
                doc.text(172, 180, 'SI ( X ) ');
                doc.text(188, 180, 'NO (   ) ');
            } else {
                doc.text(172, 180, 'SI (   ) ');
                doc.text(188, 180, 'NO ( X ) ');
            }
        //
        doc.text(10, 186, 'HIGIENE BUCAL ');
        if ($scope.historicoNuevo.HigieneBucal == null) {
            doc.text(35, 186, 'Buena (   ) ');
            doc.text(50, 186, 'Regular (   ) ');
            doc.text(69, 186, 'Mala (   ) ');
        } else

            switch ($scope.historicoNuevo.HigieneBucal.toString()) {
                case "1":
                    doc.text(35, 186, 'Buena ( X ) ');
                    doc.text(50, 186, 'Regular (   ) ');
                    doc.text(69, 186, 'Mala (   ) ');
                    break;
                case "2":
                    doc.text(35, 186, 'Buena (   ) ');
                    doc.text(50, 186, 'Regular ( X ) ');
                    doc.text(69, 186, 'Mala (   ) ');
                    break;
                case "3":
                    doc.text(35, 186, 'Buena (   ) ');
                    doc.text(50, 186, 'Regular ( X ) ');
                    doc.text(69, 186, 'Mala (   ) ');
                    break;
            }
        //cuadro
        doc.line(7, 170, 203, 170);
        doc.line(7, 176, 203, 176);
        doc.line(7, 182, 203, 182);
        doc.line(7, 188, 85, 188);
        doc.line(7, 170, 7, 188);
        doc.line(203, 170, 203, 182);
        doc.line(65, 170, 65, 176);
        doc.line(125, 170, 125, 176);
        doc.line(85, 182, 85, 188);
        ///
        doc.text(10, 194, 'Observaciones:')
        //
        doc.setFontType("bold");
        doc.text(30, 200, 'ESTADO PERIODONTAL:')
        doc.setFontType("normal");
        ///
        //doc.line(18,219,65,217);
        for (var i = 0; i < $scope.valores.length; i++) {
            doc.text(20, 206 + (i * 6), $scope.valores[i].ID);
            doc.text(30, 206 + (i * 6), $scope.valores[i].Descripcion);
            doc.line(15, 202 + (i * 6), 65, 202 + (i * 6));
            doc.line(15, 202 + (i * 6), 15, 202 + ((i + 1) * 6));
            doc.line(65, 202 + (i * 6), 65, 202 + ((i + 1) * 6));
        }
        doc.line(15, 202 + ($scope.valores.length * 6), 65, 202 + ($scope.valores.length * 6));
        var yt = 202 + ($scope.valores.length * 6);
        //
        doc.text(90, 200, 'T.');
        doc.text(100, 200, 'P.');
        doc.text(110, 200, 'CRITERIOS DE DIAGNOSTICO');
        doc.line(87, 196, 165, 196);
        doc.line(87, 196, 87, 202);
        doc.line(97, 196, 97, 202);
        doc.line(105, 196, 105, 202);
        doc.line(165, 196, 165, 202);

        for (var i = 0; i < $scope.criterios.length; i++) {
            doc.text(90, 206 + (i * 6), $scope.criterios[i].T);
            doc.text(100, 206 + (i * 6), $scope.criterios[i].P);
            doc.text(110, 206 + (i * 6), $scope.criterios[i].Criterio);
            doc.line(87, 202 + (i * 6), 165, 202 + (i * 6));
            doc.line(87, 202 + (i * 6), 87, 208 + (i * 6));
            doc.line(97, 202 + (i * 6), 97, 208 + (i * 6));
            doc.line(105, 202 + (i * 6), 105, 208 + (i * 6));
            doc.line(165, 202 + (i * 6), 165, 208 + (i * 6));
        }
        doc.line(87, 202 + ($scope.criterios.length * 6), 165, 202 + ($scope.criterios.length * 6));
        //
        doc.text(20, yt + 6, '17/16');
        doc.text(37, yt + 6, '11');
        doc.text(52, yt + 6, '26/27');
        doc.line(15, yt + 8, 65, yt + 8);
        doc.line(15, yt + 16, 65, yt + 16);
        doc.line(15, yt + 24, 65, yt + 24);
        doc.text(20, yt + 28, '46/47');
        doc.text(37, yt + 28, '31');
        doc.text(52, yt + 28, '37/36');
        doc.line(15, yt + 8, 15, yt + 24);
        doc.line(65, yt + 8, 65, yt + 24);
        doc.line(31, yt + 8, 31, yt + 24);
        doc.line(47, yt + 8, 47, yt + 24);
        ///


        doc.line(15, yt + 34, 108, yt + 34);
        doc.line(15, yt + 42, 108, yt + 42);
        doc.line(15, yt + 34, 15, yt + 49);
        doc.line(108, yt + 34, 108, yt + 49);
        doc.line(15, yt + 49, 108, yt + 49);
        doc.line(22, yt + 34, 22, yt + 49);
        doc.text(18, yt + 39, 'c');
        doc.line(29, yt + 34, 29, yt + 49);
        doc.text(25, yt + 39, 'e');
        doc.line(36, yt + 34, 36, yt + 49);
        doc.text(32, yt + 39, 'o');
        doc.line(46, yt + 34, 46, yt + 49);
        doc.text(38, yt + 38, 'Total');
        doc.text(39, yt + 40, 'ceo');
        doc.line(53, yt + 34, 53, yt + 49);
        doc.text(49, yt + 39, 'C');
        doc.line(60, yt + 38, 60, yt + 49);
        doc.line(53, yt + 38, 67, yt + 38);
        doc.line(67, yt + 34, 67, yt + 49);
        doc.text(60, yt + 37, 'P');
        doc.text(56, yt + 41, 'P');
        doc.text(63, yt + 41, 'EI');
        doc.line(73, yt + 34, 73, yt + 49);
        doc.text(69, yt + 39, 'O');
        doc.line(84, yt + 34, 84, yt + 49);
        doc.text(76, yt + 38, 'Total');
        doc.text(76, yt + 41, 'CPO');
        doc.line(95, yt + 34, 95, yt + 49);
        doc.text(86, yt + 37, 'Total');
        doc.text(86, yt + 39, 'Piezas');
        doc.text(86, yt + 41, 'Sanas');
        doc.text(97, yt + 37, 'Total');
        doc.text(97, yt + 39, 'Piezas');
        doc.text(97, yt + 41, 'Dentarias');

        ///
        ///informacion

        doc.setFontType("italic");
        doc.setFontSize(8);
        doc.line(14, 65, $scope.historicoNuevo.ApellidoPaternoPI == null ? '' : $scope.historicoNuevo.ApellidoPaternoPI.toString());
        doc.line(54, 65, $scope.historicoNuevo.ApellidoMaternoPI == null ? '' : $scope.historicoNuevo.ApellidoMaternoPI.toString());
        doc.line(93, 65, $scope.historicoNuevo.NombresPI == null ? '' : $scope.historicoNuevo.NombresPI.toString());
        doc.line(131, 65, $scope.historicoNuevo.DireccionPI == null ? '' : $scope.historicoNuevo.DireccionPI.toString());
        doc.line(178, 65, $scope.historicoNuevo.TelefonoPI == null ? '' : $scope.historicoNuevo.TelefonoPI.toString());


        doc.text(51, 59, $scope.historicoNuevo.PersonaInformacion == null ? '' : $scope.historicoNuevo.PersonaInformacion.toString());
        doc.text(17, 99, $scope.historicoNuevo.OtrosAntP == null ? '' : $scope.historicoNuevo.OtrosAntP.toString());
        doc.text(43, 105, $scope.historicoNuevo.TratamientoMedico == null ? '' : $scope.historicoNuevo.TratamientoMedico.toString());
        doc.text(144, 105, $scope.historicoNuevo.Medicamento == null ? '' : $scope.historicoNuevo.Medicamento.toString());
        doc.text(20, 125, $scope.historicoNuevo.ATM == null ? '' : $scope.historicoNuevo.ATM.toString());
        doc.text(112, 125, $scope.historicoNuevo.Labios == null ? '' : $scope.historicoNuevo.Labios.toString());
        //15
        doc.text(57, 161, $scope.historicoNuevo.OtroHabitos == null ? '' : $scope.historicoNuevo.OtroHabitos.toString());
        doc.text(10, 131, $scope.historicoNuevo.GangliosLinf == null ? '' : $scope.historicoNuevo.GangliosLinf.toString());
        doc.text(112, 131, $scope.historicoNuevo.Lengua == null ? '' : $scope.historicoNuevo.Lengua.toString());
        doc.text(112, 137, $scope.historicoNuevo.Paladar == null ? '' : $scope.historicoNuevo.Paladar.toString());
        doc.text(20, 143, $scope.historicoNuevo.OtrosExtOral == null ? '' : $scope.historicoNuevo.OtrosExtOral.toString());
        doc.text(121, 143, $scope.historicoNuevo.PisoBoca == null ? '' : $scope.historicoNuevo.PisoBoca.toString());
        doc.text(121, 149, $scope.historicoNuevo.MucosaYugal == null ? '' : $scope.historicoNuevo.MucosaYugal.toString());
        doc.text(55, 155, $scope.historicoNuevo.FecUltVisita == null ? '' : $scope.historicoNuevo.FecUltVisita);
        doc.text(112, 155, $scope.historicoNuevo.Encias == null ? '' : $scope.historicoNuevo.Encias.toString());
        doc.text(46, 179, $scope.historicoNuevo.FrecCepDent == null ? '' : $scope.historicoNuevo.FrecCepDent.toString())
        doc.text(23, yt + 13, $scope.historicoNuevo.Sext17_16 == null ? '' : $scope.historicoNuevo.Sext17_16.toString());
        doc.text(37, yt + 13, $scope.historicoNuevo.Sext11 == null ? '' : $scope.historicoNuevo.Sext11.toString());
        doc.text(55, yt + 13, $scope.historicoNuevo.Sext26_27 == null ? '' : $scope.historicoNuevo.Sext26_27.toString());
        doc.text(23, yt + 21, $scope.historicoNuevo.Sext46_47 == null ? '' : $scope.historicoNuevo.Sext46_47.toString());
        doc.text(37, yt + 21, $scope.historicoNuevo.Sext31 == null ? '' : $scope.historicoNuevo.Sext31.toString());
        doc.text(55, yt + 21, $scope.historicoNuevo.Sext37_36 == null ? '' : $scope.historicoNuevo.Sext37_36.toString());
        doc.text(17, yt + 47, $scope.historicoNuevo.Cminuscula == null ? '' : $scope.historicoNuevo.Cminuscula.toString());
        doc.text(24, yt + 47, $scope.historicoNuevo.E == null ? '' : $scope.historicoNuevo.E.toString());
        doc.text(31, yt + 47, $scope.historicoNuevo.Ominuscula == null ? '' : $scope.historicoNuevo.Ominuscula.toString());
        doc.text(39, yt + 47, $scope.historicoNuevo.TotalCEO == null ? '' : $scope.historicoNuevo.TotalCEO.toString());
        doc.text(49, yt + 47, $scope.historicoNuevo.Cmayuscula == null ? '' : $scope.historicoNuevo.Cmayuscula.toString());
        doc.text(56, yt + 47, $scope.historicoNuevo.P == null ? '' : $scope.historicoNuevo.P.toString());
        doc.text(63, yt + 47, $scope.historicoNuevo.Ei == null ? '' : $scope.historicoNuevo.Ei.toString());
        doc.text(69, yt + 47, $scope.historicoNuevo.Omayuscula == null ? '' : $scope.historicoNuevo.Omayuscula.toString());
        doc.text(76, yt + 47, $scope.historicoNuevo.TotalCPO == null ? '' : $scope.historicoNuevo.TotalCPO.toString());
        doc.text(88, yt + 47, $scope.historicoNuevo.TotalPiezasSanas == null ? '' : $scope.historicoNuevo.TotalPiezasSanas.toString());
        doc.text(99, yt + 47, $scope.historicoNuevo.TotalPiezasDent == null ? '' : $scope.historicoNuevo.TotalPiezasDent.toString());
        doc.text(20, 29, $scope.historicoNuevo.ApellidoPaterno);
        doc.text(70, 29, $scope.historicoNuevo.ApellidoMaterno);
        doc.text(129, 29, $scope.historicoNuevo.Nombre);
        doc.text(169, 29, $scope.historicoNuevo.Edad.toString());
        doc.text(189, 29, $scope.historicoNuevo.Sexo);
        //I2
        doc.text(14, 39, $scope.historicoNuevo.FechaNacimiento);
        doc.text(75, 39, $scope.historicoNuevo.Ocupacion == null ? "" : $scope.historicoNuevo.Ocupacion);
        doc.text(129, 39, $scope.historicoNuevo.Direccion == null ? "" : $scope.historicoNuevo.Direccion);
        doc.text(169, 39, $scope.historicoNuevo.Telefono == null ? "" : $scope.historicoNuevo.Telefono);

        doc.addPage();

        var c = document.getElementById("myCanvas");
        var ctx = c.getContext("2d");
        doc.addImage(c.toDataURL("image/png"), 'PNG', 10, 10);
        doc.save('FichaOdontologica' + $scope.historicoFichaCab.Titulo + '.pdf');
    }
    function prepararNuevoHistorico() {
        $scope.historicoNuevo = {
            ID: 0,

            IdHistoricoFichaCab: "",

            FechaCreacion: "",

            ApellidoPaterno: "",

            ApellidoMaterno: "",

            Nombre: "",

            Edad: "",

            Sexo: "F",

            LugarNacimiento: "",

            FechaNacimiento: "",

            Ocupacion: "",

            Direccion: "",

            Telefono: "",

            GradoInstruccion: "I",

            EstadoCivil: "S",

            NacionesOriginarias: "",

            Idioma: "",

            PersonaInformacion: "",

            ApellidoPaternoPI: "",

            ApellidoMaternoPI: "",

            NombresPI: "",

            DireccionPI: "",

            TelefonoPI: "",

            AntPatFam: "",

            Anemia: false,

            Cardiopatias: false,

            EnfGastricas: false,

            Hepatitis: false,

            Tuberculosis: false,

            Asma: false,

            DiabetesMel: false,

            Epilepsia: false,

            Hipertension: false,

            VIH: false,

            OtrosAntP: "",

            Alergias: false,

            Embarazo: 0,

            TratamientoMedico: "",

            Medicamento: "",

            HemorragiaExtDent: 0,

            ATM: "",

            GangliosLinf: "",

            Respirador: "",

            OtrosExtOral: "",

            Labios: "",

            Lengua: "",

            Paladar: "",

            PisoBoca: "",

            MucosaYugal: "",

            Encias: "",

            isProtesisDental: "",

            FecUltVisita: "",

            isFuma: "",

            isBebe: "",

            OtroHabitos: "",

            isCepilloDental: null,

            isHiloDental: null,

            isEnjuagueBucal: null,

            FrecCepDent: "",

            isSangreEncias: null,

            HigieneBucal: null,

            Observaciones: "",

            Sext17_16: "",

            Sext11: "",

            Sext26_27: "",

            Sext46_47: "",

            Sext31: "",

            Sext37_36: "",

            Cminuscula: "",

            E: "",

            Ominuscula: "",

            TotalCEO: "",

            Cmayuscula: "",

            P: "",

            Ei: "",

            Omayuscula: "",

            TotalCPO: "",

            TotalPiezasSanas: "",

            TotalPiezasDent: "",
            FechaNacimientoString: "",
        }
    }
    //$scope.validarCamposHistoricos = function () {
    //    return ($scope.historicoNuevo && ($scope.historicoNuevo.))
    //}
    function inicializarDatos() {
        $("#fechanac").datepicker({
            dateFormat: 'dd/mm/yy',
            onSelect: function () {

                $scope.historicoNuevo.FechaNacimiento = $.datepicker.formatDate("dd/mm/yy", $(this).datepicker('getDate'));
                $scope.historicoNuevo.FechaNacimientoString = $.datepicker.formatDate("dd/mm/yy", $(this).datepicker('getDate'));
                $scope.$apply();
            }
        });
        $("#ultimafecha").datepicker({
            dateFormat: 'dd/mm/yy',
            onSelect: function () {

                $scope.historicoNuevo.FecUltVisita = $.datepicker.formatDate("dd/mm/yy", $(this).datepicker('getDate'));
                $scope.historicoNuevo.UltimaVisitaString = $.datepicker.formatDate("dd/mm/yy", $(this).datepicker('getDate'));
                $scope.$apply();
            }
        });

        $scope.siEmbarazo = 1;
        $scope.siExtraccion = 0;
        consultasService.obtenerCriterios().then(function (result) {
            $scope.criterios = result;
        });
        consultasService.obtenerValoresPeriodontal().then(function (result) {
            $scope.valores = result;
        });

        if ($rootScope.fichaHistoricoNueva)
            if ($rootScope.fichaHistoricoNueva.ID != 0) {
                consultasService.obtenerHistoricoFichaCab($rootScope.fichaHistoricoNueva.ID).then(function (result) {
                    $scope.historicoFichaCab = result;
                    $scope.historicoNuevo = result.HistoricoPaciente;
                    $scope.dientes = JSON.parse(result.Odontograma);
                    angular.forEach($scope.historicoFichaCab.FichaDetalle, function (element) {
                        element.Fecha = moment(element.Fecha).format('DD/MM/YYYY');

                    });
                    $scope.siEmbarazo = $scope.historicoNuevo.Embarazo == 0 ? 0 : 1;
                    $scope.siExtraccion = $scope.historicoNuevo.HemorragiaExtDent == 0 ? 0 : 1;
                    $scope.historicoNuevo.Alergias = $scope.historicoNuevo.Alergias ? "true" : "false";
                    $scope.historicoNuevo.isCepilloDental = $scope.historicoNuevo.isCepilloDental ? "true" : "false";
                    $scope.historicoNuevo.isHiloDental = $scope.historicoNuevo.isHiloDental ? "true" : "false";
                    $scope.historicoNuevo.isEnjuagueBucal = $scope.historicoNuevo.isEnjuagueBucal ? "true" : "false";
                    $scope.historicoNuevo.isSangreEncias = $scope.historicoNuevo.isSangreEncias ? "true" : "false";
                    $scope.historicoNuevo.isProtesisDental = $scope.historicoNuevo.isProtesisDental ? "true" : "false";
                    $scope.historicoNuevo.FechaNacimiento = moment($scope.historicoNuevo.FechaNacimiento).format('DD/MM/YYYY');
                    $scope.historicoNuevo.FecUltVisita = $scope.historicoNuevo.FecUltVisita == null ? moment(new Date()).format('DD/MM/YYYY') : moment($scope.historicoNuevo.FecUltVisita).format('DD/MM/YYYY');
                    $scope.historicoNuevo.FechaNacimientoString = $scope.historicoNuevo.FechaNacimiento;
                    $scope.historicoNuevo.UltimaVisitaString = $scope.historicoNuevo.FecUltVisita == null ? moment(new Date()).format('DD/MM/YYYY') : moment($scope.historicoNuevo.FecUltVisita).format('DD/MM/YYYY');
                    dibujarDientes();

                });

            }
            else {
                $scope.historicoFichaCab = angular.copy($rootScope.fichaHistoricoNueva);
                prepararNuevoHistorico();
                $scope.historicoFichaCab.FichaTrabajo = [];
                $scope.historicoFichaCab.FichaDetalle = [];
                $scope.dientes = JSON.parse('[[{"Espacio":50,"Numero":18,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":17,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":16,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":15,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":14,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":13,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":12,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":11,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":21,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":22,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":23,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":24,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":25,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":26,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":27,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":28,"Estado":1,"Defectos":[]}],[{"Espacio":176,"Numero":55,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":54,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":53,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":52,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":51,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":61,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":62,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":63,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":64,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":65,"Estado":1,"Defectos":[]}],[{"Espacio":176,"Numero":85,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":84,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":83,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":82,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":81,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":71,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":72,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":73,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":74,"Estado":1,"Defectos":[]},{"Espacio":176,"Numero":75,"Estado":1,"Defectos":[]}],[{"Espacio":50,"Numero":48,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":47,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":46,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":45,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":44,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":43,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":42,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":41,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":31,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":32,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":33,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":34,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":35,"Estado":1,"Defectos":[]},{"Espacio":50,"Numero":36,"Estado":26,"Defectos":[]},{"Espacio":50,"Numero":37,"Estado":27,"Defectos":[]},{"Espacio":50,"Numero":38,"Estado":28,"Defectos":[]}]]');
                dibujarDientes();
            }
    }

    $scope.selectFichaPaciente = function (fichaHistorico) {
        $scope.fichaPacienteSelected = fichaHistorico;
    }
    $scope.salirModalTrabajo = function () {
        $('#modal-nueva-ficha').modal("hide");

    }
    $scope.salirModalDetalle = function () {
        $('#modal-nueva-detalle').modal("hide");

    }
    $scope.closeModalLicencia = function () {
        $('#modal-licencia-free').modal("hide");

    }
    $scope.guardarCambiosFicha = function () {
        if ($rootScope.sessionDto.Licencia == -1 || $rootScope.sessionDto.Licencia == 0) {
            $('#modal-licencia-free').modal("show");
        } else {
            $scope.historicoFichaCab.Odontograma = JSON.stringify($scope.dientes);
            consultasService.guardarHistoricoCompleto($scope.historicoFichaCab).then(function (result) {
                if (result.Success) {
                    toastr.success(result.Message);
                    inicializarDatos();

                } else {
                    toastr.error(result.Message);
                }
            });
        }

    }

    function actualizarTratamiento() {
        consultasService.obtenerHistoricoFichaTrabajo($scope.historicoFichaCab.ID).then(function (result) {

            $scope.historicoFichaCab.FichaTrabajo = result;
        });
    }
    $scope.guardarTratamiento = function () {
        if ($rootScope.sessionDto.Licencia == -1 || $rootScope.sessionDto.Licencia == 0) {
            $('#modal-licencia-free').modal("show");
        } else {
            consultasService.guardarHistoricoFichaTrabajo($scope.historicoFichaCab.FichaTrabajo).then(function (result) {
                if (result.Success) {
                    toastr.success(result.Message);
                    actualizarTratamiento();

                } else {
                    toastr.error(result.Message);
                }
            });
        }
    }
    $scope.openModalCerrarFicha = function () {
        $scope.esCerrarFicha = true;
        if ($rootScope.sessionDto.IDCitaSeleccionada != "")
            $("#cita-atendida").modal("show");
        else {
            $scope.cerrarFicha(false);
        }

    }
    $scope.cerrarFicha = function (citaAtendida) {
        $("#cita-atendida").modal("hide");
        if ($rootScope.sessionDto.Licencia == -1 || $rootScope.sessionDto.Licencia == 0) {
            $('#modal-licencia-free').modal("show");
        } else {
            consultasService.cerrarFichaHistorico($scope.historicoFichaCab.ID, citaAtendida).then(function (result) {
                if (result.Success) {
                    toastr.success(result.Message);
                    inicializarDatos();
                    $rootScope.sessionDto.IDCitaSeleccionada = "";
                    $scope.esCerrarFicha = false;
                } else {
                    toastr.error(result.Message);
                }
            });
        }
    }
    $scope.confirmarCitaAtendida = function () {
        if ($rootScope.sessionDto.IDCitaSeleccionada != "")
            $("#cita-atendida").modal("show");
        else {
            $scope.guardarTrabajosRealizados(false);
        }
    }
    $scope.marcarAtendidaCerrarOTrabajo = function (citaAtendida) {
        if ($scope.esCerrarFicha)
            $scope.cerrarFicha(citaAtendida);
        else
            $scope.guardarTrabajosRealizados(citaAtendida);
    }
    function actualizarTrabajosRealizados() {
        consultasService.obtenerHistoricoFichaDetalle($scope.historicoFichaCab.ID).then(function (result) {

            $scope.historicoFichaCab.FichaDetalle = result;
            angular.forEach($scope.historicoFichaCab.FichaDetalle, function (element) {
                element.Fecha = moment(element.Fecha).format('DD/MM/YYYY');
                $scope.esCerrarFicha = false;
            });
        });

    }
    $scope.guardarTrabajosRealizados = function (citaatendida) {
        $("#cita-atendida").modal("hide");
        if ($rootScope.sessionDto.Licencia == -1 || $rootScope.sessionDto.Licencia == 0) {
            $('#modal-licencia-free').modal("show");
        } else {
            consultasService.guardarHistoricoFichaDetalle($scope.historicoFichaCab.FichaDetalle, citaatendida).then(function (result) {
                if (result.Success) {
                    toastr.success(result.Message);
                    actualizarTrabajosRealizados();
                    if (citaatendida) {
                        $('.modal-backdrop').remove();
                        $('body').removeClass("modal-open");
                        $location.path("/consultas");
                    }

                } else {
                    toastr.error(result.Message);
                }
            });
        }
    }
    $scope.agregarTrabajo = function () {
        $('#modal-nueva-ficha').modal('hide');
        $scope.historicoFichaCab.FichaTrabajo.push(angular.copy($scope.fichaHistoricoTrabajoNueva));
    }
    $scope.openModalCrearFichaTrabajo = function () {
        $scope.fichaTrabajoSeleccionada = null;
        prepararNuevaFichaTrabajo();
        $scope.fichaPacienteSelected = null;
        $('#modal-nueva-ficha').modal("show");
    }
    function prepararNuevaFichaTrabajo() {
        $scope.fichaHistoricoTrabajoNueva = {
            ID: 0,
            IDHistoricoFichaCab: $rootScope.fichaHistoricoNueva.ID,
            Pieza: 0,
            Descripcion: '',
            Costo: 0,
            Eliminar: false
        };
    }
    $scope.volverAHistorico = function () {
        $location.path('/historicos');
    }
    $scope.agregarDetalle = function () {
        $('#modal-nuevo-detalle').modal("hide");
        $scope.historicoFichaCab.FichaDetalle.push(angular.copy($scope.fichaHistoricoDetalleNuevo));
    }
    $scope.openModalCrearFichaDetalle = function () {
        prepararNuevaFichaDetalle();
        $scope.fichaPacienteSelected = null;
        $('#modal-nuevo-detalle').modal("show");
    }
    $scope.salirModaDetalle = function () {
        $('#modal-nuevo-detalle').modal("hide");
    }
    function prepararNuevaFichaDetalle() {
        $scope.fichaHistoricoDetalleNuevo = {
            ID: 0,
            IDHistoricoFichaCab: $rootScope.fichaHistoricoNueva.ID,
            Pieza: 0,
            TrabajoRealizado: '',
            Fecha: moment(new Date()).format('DD/MM/YYYY'),
            Eliminar: false

        };
    }
    $scope.openModalSeleccionar = function () {
        $scope.imprimirPdf = "-1";
        $('#modal-seleccionar-pdf').modal("show");

    }
    $scope.cerrarModalPS = function () {
        $('#modal-seleccionar-pdf').modal("hide");
    }
    $scope.cerrarModalTr = function () {
        $('#modal-confirmar-elimiar-tr').modal("hide");
    }
    $scope.openModalConfirmacionEliminarT = function () {
        $('#modal-confirmar-elimiar-tr').modal("show");
    }
    $scope.eliminarTrabajo = function () {
        $('#modal-confirmar-elimiar-tr').modal("hide");
        $scope.fichaTrabajoSeleccionada.Eliminar = true;
    }

    $scope.selectFichaTrabajo = function (ft) {
        $scope.fichaTrabajoSeleccionada = ft;
    }
    $scope.cerrarModalDet = function () {
        $('#modal-confirmar-eliminar-det').modal("hide");
    }
    $scope.openModalConfirmacionEliminarD = function () {
        $('#modal-confirmar-eliminar-det').modal("show");
    }
    $scope.eliminarDetalle = function () {
        $('#modal-confirmar-eliminar-det').modal("hide");
        $scope.fichaDetalleSeleccionada.Eliminar = true;
    }

    $scope.selectFichaDetalle = function (fd) {
        $scope.fichaDetalleSeleccionada = fd;
    }

    $scope.limpiar = function () {

        $scope.historicoNuevo = angular.copy($scope.historicoOriginal);

    }
    $scope.guardarHistorico = function () {
        if ($scope.siEmbarazo == 0) $scope.historicoNuevo.Embarazo = 0;
        if ($scope.siExtraccion == 0) $scope.historicoNuevo.HemorragiaExtDent = 0;
        if ($rootScope.sessionDto.Licencia == -1 || $rootScope.sessionDto.Licencia == 0) {
            $('#modal-licencia-free').modal("show");
        } else {
            consultasService.guardarHistoricoPaciente($scope.historicoNuevo).then(function (result) {
                if (result.Success) {
                    toastr.success(result.Message);
                    inicializarDatos();

                } else {
                    toastr.error(result.Message);
                }
            });
        }
    }
});