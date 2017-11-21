app.controller("rolesController", function (rolesService, $scope, $rootScope, loginService) {
    init();

    function init() {
        $rootScope.mostrarMenu = true;
        $scope.allRoles = [];
        $scope.subString = "";
      
        $scope.message = "";
        $scope.rolSelected = null;
        $scope.nombrerol = "";
        if (!$rootScope.sessionDto) {
            loginService.getSessionDto().then(function (result) {
                $rootScope.sessionDto = result;
                cargar_todos_los_roles();
            });
        } else {
            cargar_todos_los_roles();
        }
    };

    function cargar_todos_los_roles() {
        var roles = rolesService.getAllRols($rootScope.sessionDto.IDConsultorio).then(function (result) {
            $scope.allRoles = result;
           
        });
    }
    $scope.selectRol = function (rol) {
        $scope.rolSelected = rol;
        rolesService.getModulos($scope.rolSelected.ID).then(function (result) {
            $scope.modulosAsignados = result;
        });
    }
    $scope.openModalNewRol = function () {
        $scope.nombrerol = "";
        $scope.message = "";
        $('#new-rol').modal('show');
    }
    
    $scope.openModalConfirmDelele = function () {
        $('#confirm-delete').modal('show');
    }
    $scope.closeWarnig = function () {
        $scope.rolSelected = null;
        $('#confirm-delete').modal('hide');
    }

    $scope.EliminarRol = function () {
        rolesService.eliminarRol($scope.rolSelected.ID).then(function (result) {
            if (result.Data == 1) {
                $scope.rolSelected = null;
                $('#confirm-delete').modal('hide');
                cargar_todos_los_roles();
                toastr.success(result.Message);
            } else {
                toastr.error(result.Message);
            }
        });
    }
    $scope.crearNewRol = function () {
        rolesService.insertarRol($scope.nombrerol, $rootScope.sessionDto.IDConsultorio).then(function (result) {
            if (result.Data != 2) {
                $('#new-rol').modal('hide');
                toastr.success(result.Message);
                cargar_todos_los_roles();
            } else {
                toastr.error(result.Message);
            }

        });
    }
    $scope.modificarPermisos = function () {
        rolesService.modificarPermisos($scope.modulosAsignados, $scope.rolSelected.ID).then(function (result) {
            if (result.Data ) {
             
                toastr.success(result.Message);
                cargar_todos_los_roles();
            } else {
                toastr.error(result.Message);
            }

        });
    }
    $scope.selectNodeHead = function (tree) {
        tree.IsCollapsed = !tree.IsCollapsed;
    }
   
    $scope.formularioChecked = function (modulo,formulario, event) {
        if (event.currentTarget.checked) 
            modulo.IsChecked = true;
        seleccionarHijosComponente(formulario.Hijos, event.currentTarget.checked);
    }
    function seleccionarHijosComponente(componentes, check) {
        var cantidadComponentes = componentes.length;
        if (cantidadComponentes > 0) {
            for (var i = 0; i < cantidadComponentes; i++)
                componentes[i].IsChecked = check;
        }
    }
    $scope.seleccionarComponentePadres = function (modulo, formulario, event) {
        if (event.currentTarget.checked) {
            formulario.IsChecked = true;
            modulo.IsChecked = true;
        }
    }
    $scope.moduloChecked = function (modulo, event) {
     
        var cantidadFormularios = modulo.Hijos.length;
        if (cantidadFormularios > 0) {
            for (var i = 0; i < cantidadFormularios; i++) {
                modulo.Hijos[i].IsChecked = event.currentTarget.checked;
                seleccionarHijosComponente(modulo.Hijos[i].Hijos, event.currentTarget.checked);
            }
               
        }
     
    }
});