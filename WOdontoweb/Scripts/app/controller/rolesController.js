app.controller("rolesController", function (rolesService, $scope,$rootScope) {
    init();

    function init() {
        $scope.allRoles = [];
        $scope.subString = "";
        cargar_todos_los_roles();
        $scope.message = "";
        $scope.rolSelected = null;
        $scope.nombrerol = "";
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
});