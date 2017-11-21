app.directive('validate', [ function (localize) {
    return {
        require: "ngModel",
        link: function (scope, elm, attrs, ctrl) {

            var validate = $(elm).attr("validate");
            var validateRE = "";
            var name = attrs.ngModel.substr(attrs.ngModel.lastIndexOf('.') + 1);
            var message = "";
            ctrl.$parsers.unshift(function (viewValue) {
                switch (validate) {
                    case 'text':
                        message = "Solo se permiten letras"; // "Solo se permiten letras";
                        validateRE = /^[A-Za-z\sÑñ]*$/;
                        break;
                    case 'text-we':
                        message = "Solo se permiten letras"; // "Solo se permiten letras";
                        validateRE = /^[A-Za-z\s]*$/;
                        break;
                    case 'text-number':
                        message = "Solo se permite letras y numeros"; //"Solo se permite letras y numeros";
                        validateRE = /^[A-Za-z\s0-9]*$/;
                        break;
                    case 'number':
                        message = "No es un número válido";
                        validateRE = /^[0-9]*$/;
                        break;
                    case 'periodontal':
                        message = "Solo se permite valores: 0, 1, 2, 3, 4 o X";
                        validateRE = /^[01234Xx]$/;
                        break;
                    case 'telephone-number':
                        message = "No es un número telefono válido";
                        validateRE = /^[-\s0-9]*$/;
                        break;
                    case 'real':
                        message = "No es un número válido";
                        validateRE = /^[+-]?\d+([,.]\d+)?$/;
                        break;
                    case 'email':
                        message = "No es una direccion de correo válida";
                        validateRE = /[\w-\.]{3,}@([\w-]{2,}\.)*([\w-]{2,}\.)[\w-]{2,4}/;
                        break;
                    case 'user':
                        message = "Usuario no válido";
                        validateRE = /^[a-z\d_]{4,20}$/i;
                        break;
                    case 'password':
                        message = "Contraseña no válida";
                        validateRE = /(?=^.{6,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$/;
                        break;
                    case 'fecha':
                        message = "Fecha no válida";
                        validateRE = /^([0][1-9]|[12][0-9]|3[01])(\/|-)([0][1-9]|[1][0-2])\2(\d{4})$/;
                        break;
                    case 'nameformula':
                        message = "Nombre no valido";
                        validateRE = /^[A-z\\\_][\w\.]?.{0,62}$/;
                        break;
                    case 'url':
                        message = "URL no válida";
                        validateRE = /^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)( [a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$/;
                        break;
                    case 'decimal':
                        message = "No es un número válido";
                        validateRE = /^[0-9]*\,?[0-9]*$/;
                        break;
                    default:
                        validateRE = /./;
                }

                var ngShow = "<span class='error' id='" + name + "'>&nbsp;" + message + "</span>";

                $(elm).focusout(function () {
                    $("#" + name).fadeOut();
                });

                if ($(elm).attr("required") && $(elm).val() === '') {
                    var textTranslated = "El campo es obligatorio";
                    message = textTranslated; 
                    if (!$("#" + name).length) {
                        $(elm).after(ngShow);
                    }
                    $("#" + name).text(message);
                    $("#" + name).fadeIn();

                } else
                    if (validateRE.test(viewValue) || $(elm).val() === '') {
                        ctrl.$setValidity('validate', true);
                        if ($("#" + name).length) {
                            $("#" + name).fadeOut();
                        }
                    }
                    else {
                        ctrl.$setValidity('validate', false);
                        if (!$("#" + name).length) {
                            $(elm).after(ngShow);
                        } else {
                            $("#" + name).text(message);
                            $("#" + name).fadeIn();
                        }
                        return undefined;
                    }
                return viewValue;
            });


        }
    };
}]);

app.directive('rmtValidate', function () {
    return {
        require: "ngModel",
        link: function (scope, elm, attrs, ctrl) {
            var type = attrs.rmtValidate,
                name = "error" + attrs.ngModel.substr(attrs.ngModel.lastIndexOf('.') + 1),
                message = "";
            ctrl.$parsers.unshift(function (viewValue) {
                var validatorMethod,
                    isValidInput = true,
                    value = $(elm).val(),
                    errorTooltip = $("#" + name);
                switch (type) {
                    case 'real':
                        message = "No es un número válido";
                        validatorMethod = scope.isValidNumber;
                        break;
                    case 'date':
                        message = "No es una fecha válida";
                        validatorMethod = scope.isValidDate;
                        break;
                    default:
                        return viewValue;
                }

                var ngShow = "<span class='error' id='" + name + "'>&nbsp;" + message + "</span>";

                if ($(elm).attr("required") && value === '') {
                    message = "El campo es obligatorio";
                    if (!errorTooltip.length) {
                        $(elm).after(ngShow);
                    }
                    $("#" + name).text(message);
                    $("#" + name).fadeIn();
                    isValidInput = false;
                } else
                    if (value === '' || validatorMethod(viewValue)) {
                        isValidInput = true;
                        if (errorTooltip.length) {
                            $("#" + name).fadeOut();
                        }
                    }
                    else {
                        isValidInput = false;
                        if (!errorTooltip.length) {
                            $(elm).after(ngShow);
                        }
                        $("#" + name).text(message);
                        $("#" + name).fadeIn();
                    }
                ctrl.$setValidity('rmtValidate', isValidInput);
                scope.isValidParamValue = isValidInput;
                return isValidInput ? viewValue : undefined;
            });


        }
    };
});

app.directive('validatename', function () {
    return {
        require: "ngModel",
        link: function (scope, elm, attrs, ctrl) {
            var name = attrs.ngModel.substr(attrs.ngModel.lastIndexOf('.') + 1);
            var message = "";
            ctrl.$parsers.unshift(function (viewValue) {
                var reg = /^[A-z\\\_][\w\.]?.{0,62}$/;
                var ngShow = "<span class='error' id='" + name + "' data-i18n='_INCORRECTNAMEABEL'>&nbsp;</span>";
                $(elm).focusout(function () {
                    $("#" + name).fadeOut();
                });

                if (reg.test(viewValue) || $(elm).val() === '') {
                   ctrl.$setValidity('validate', true);
                    if ($("#" + name).length) {
                        $("#" + name).fadeOut();
                    }
                }
                else {
                    ctrl.$setValidity('validate', false);
                    if (!$("#" + name).length) {
                        $(elm).after(ngShow);
                    } else {
                        $("#" + name).text(message);
                        $("#" + name).fadeIn();
                    }
                    return undefined;
                }
                return viewValue;
            });


        }
    };
});

app.directive('validateLocalNumber', ["localize", function (localize) {
    return {
        require: "ngModel",
        link: function (scope, elm, attrs, ctrl) {

            var validate = $(elm).attr("validate-local-number");
            var validateRE = "";
            var name = attrs.ngModel.substr(attrs.ngModel.lastIndexOf('.') + 1);
            var message = "";
            var showMessage = true;
           ctrl.$parsers.push(function (inputValue) {
                showMessage = true;
                if (inputValue == undefined) return '';
                var transformedInput = inputValue.replace(/[^0-9\.,]/g, '');
                if (transformedInput != inputValue) {
                    ctrl.$setViewValue(transformedInput);
                    ctrl.$render();
                    showMessage = false;
                }
                
                var symbol = (0.001).toLocaleString().charAt(1);

                if (symbol != "") {
                    if (inputValue.indexOf(',') > 0 && symbol != ',') {
                        ctrl.$setViewValue(inputValue.replaceAll(',', symbol));
                        ctrl.$render();
                    }
                    if (inputValue.indexOf('.') > 0 && symbol != '.') {
                        ctrl.$setViewValue(inputValue.replaceAll('.', symbol));
                        ctrl.$render();
                    }
                }

                switch (validate) {
                    case 'real':
                        message = "No es un número válido!";
                        validateRE = /^[+-]?\d+([.,]\d+)?$/;
                        break;
                    default:
                        validateRE = /./;
                }



                var ngShow = "<span class='error' id='" + name + "'>&nbsp;" + message + "</span>";

                $(elm).focusout(function () {
                    $("#" + name).fadeOut();
                });

                if ($(elm).attr("required") && $(elm).val() === '') {
                    var textTranslated = localize.getLocalizedString("_FIELDREQUIREDMSN");
                    message = textTranslated;
                    if (!$("#" + name).length) {
                        $(elm).after(ngShow);
                    }
                    $("#" + name).text(message);
                    $("#" + name).fadeIn();

                } else
                    if (validateRE.test(inputValue) || $(elm).val() === '') {
                        ctrl.$setValidity('validate', true);
                        if ($("#" + name).length) {
                            $("#" + name).fadeOut();
                        }
                    }
                    else {
                        if (showMessage && inputValue) {

                            ctrl.$setValidity('validate', false);
                            if (!$("#" + name).length) {
                                $(elm).after(ngShow);
                            } else {
                                $("#" + name).text(message);
                                $("#" + name).fadeIn();
                            }

                        }
                    }
                

                return transformedInput;
            });
            
            


        }
    };
}]);
app.directive('requiredField', [ function () {
    return {
        restrict: 'E',
        template: '<span >{{textoRequiered}}</span><sup class="fa fa-asterisk" style="font-size:8px; top: -0.2cm !important; color: red"></sup>',
        scope: {
        },
       // templateUrl: "/Scripts/app/partials/requieredField.html",
        link: function (scope, element, attrs) {
            scope.textoRequiered = attrs.texto;
        }
    };
}]);