app.directive('autocompleted', function () {

    return{
        restrict: 'A',
        
        link: function (scope, element, attrs) {
            console.log(scope.clinicas);
            $(element).autocomplete({
                source: scope.sourceList,
                minLength: 2,
                //select: function (event, ui) {
                //    scope.objectId = ui.item.id;
                //    scope.valueObject = ui.item.value;
                //    scope.fkReference = ui.item.fk;
                //    scope.$apply();

                //}
            });

        }
    } 

});