app.controller("empresaController", function (clinicaService, $scope, $rootScope) {   
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

    $rootScope.openModalMiConsultorio = function (e) {
        e.preventDefault();
        prepararDtoConsultorio();
        clinicaService.getIntervalosTiempo().then(function (resultIntervalo) {
            $scope.intervalos = resultIntervalo;
            clinicaService.getConsultorioByID($rootScope.sessionDto.IDConsultorio).then(function (result) {
                $scope.consultorioParaGuardar = result;
                seleccionarIntervalo();
                $('#modal-mi-consultorio').modal('show');
            });
        });        
    };

    function seleccionarIntervalo() {
        var selected = $scope.intervalos.where(function (item) {
            return item.ID == $scope.consultorioParaGuardar.IDIntervalo;
        });
        $scope.intervaloSelected = selected[0];
    }

    $scope.closeModal = function (nameModal) {
        $(nameModal).modal('hide');
    };
});