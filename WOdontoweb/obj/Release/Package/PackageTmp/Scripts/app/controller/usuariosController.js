app.controller("usuariosController", function (usuariosService, rolesService, $scope, $rootScope) {
    init();

    function init() {
        $scope.allUsers = [];
        $scope.subString = "";
        cargar_todos_los_usuarios();
        $scope.message = "";
        $scope.userSelected = null;
        $scope.nombrerol = "";
        $scope.rolesConsultorio = [];
        cargar_roles_empresa();
        $scope.rolSelected = null;
        prepararNuevoUsuario();
        $scope.idEmpresa = 1;
    };

    function prepararNuevoUsuario() {
        $scope.userSelected = null;
        $scope.userToSave = {
            Nombre: "",
            Password: "",
            Login: "",
            State: 1,
            IDRol: -1,
            IDEmpresa: $scope.idEmpresa,
            changepass: false
        };

        $scope.rolSelected = null;
        $("#nombreId").focus();
    }

    $scope.nuevoUsuario = function () {
        prepararNuevoUsuario();
    }

    function cargar_roles_empresa() {
        rolesService.getAllRols($rootScope.sessionDto.IDConsultorio).then(function (result) {
            $scope.rolesConsultorio = result;

        });
    }


    function cargar_todos_los_usuarios() {
        var user = usuariosService.getAllUsers($rootScope.sessionDto.IDConsultorio).then(function (result) {
            $scope.allUsers = result;

        });
    }
    $scope.selectUser = function (user) {
        $scope.userSelected = user;
        $scope.userToSave = angular.copy($scope.userSelected);
        $scope.userToSave.State = 2;
        $scope.userToSave.ConfirmPass = angular.copy($scope.userToSave.Password);
        selectRol();
    }

    function selectRol() {
        var selectRolUser = $scope.rolesConsultorio.where(function (item) {
            return item.ID == $scope.userToSave.IDRol
        });
        $scope.rolSelected = selectRolUser[0];
    }

    $scope.validadUsuario = function () {
        return $scope.userToSave == null || $scope.userToSave.Nombre.length < 4 || $scope.userToSave.Login.length < 4
            || $scope.userToSave.Password.length < 4 || $scope.userToSave.Password != $scope.userToSave.ConfirmPass
             || $scope.rolSelected == null;
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
        $scope.userSelected = null;
        $('#confirm-delete').modal('hide');
    }

    $scope.eliminarUsuario = function () {
        usuariosService.eliminarUsuario($scope.userToSave).then(function (result) {
            if (result.Data == 1) {
                $('#confirm-delete').modal('hide');
                cargar_todos_los_usuarios();
                prepararNuevoUsuario();
                toastr.success(result.Message);
            } else {
                toastr.error(result.Message);
            }
        });
    }
    $scope.guardarUsuario = function () {
        $scope.userToSave.IDRol = $scope.rolSelected.ID;
        if ($scope.userToSave.State == 1) {
            usuariosService.insertarUsuario($scope.userToSave).then(function (result) {

                if (result.Data == 1) {
                    $scope.allUsers.push(angular.copy($scope.userToSave));
                    toastr.success(result.Message);
                    prepararNuevoUsuario();
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
    }
});