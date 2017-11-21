﻿app.directive("loader", function () {
    var permanentlyHide = false;
    return function ($scope, element, attrs) {
        element.height = 1500;
        $scope.$on("loader_show", function () {
            if (!permanentlyHide) {
               
                return element.show();
            }
            return element.hide();
        });

        return $scope.$on("loader_hide", function (e, value) {
            permanentlyHide = value;
            return element.hide();
        });
    };
});